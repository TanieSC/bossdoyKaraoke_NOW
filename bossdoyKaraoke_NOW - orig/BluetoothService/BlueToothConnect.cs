using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Models;
using bossdoyKaraoke_NOW.Nlog;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using static bossdoyKaraoke_NOW.Enums.RemoveVocal;

namespace bossdoyKaraoke_NOW.BluetoothService
{
    public class BlueToothConnect
    {
        BluetoothListener m_btListener;
       // BluetoothClient remoteDevice;
        Stream m_peerStream;
        private bool m_listening = true;
        private int m_parseResult;

        string m_filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\karaokeNow\\JsonString\\";

        // Constant that indicate command from devices
        private const int EXIT_CMD = -1;
        private const int KEY_Up = 1;
        private const int KEY_Down = 2;
        private const int PlayPause = 3;
        private const int NextTrack = 4;
        private const int VolumeMuteUnMute = 5;
        private const int IncMusicTempo = 6;
        private const int DecMusicTempo = 7;
        private const int IncMusicKey = 8;
        private const int DecMusicKey = 9;
        private const int RemoveVocalNone = 10;
        private const int RemoveVocalRight = 11;
        private const int RemoveVocalLeft = 12;
        private const int NowPlaying = 13;
        private const int FileNotFound = 14;
        private const int FileFound = 15;
        private const int RemoveVocal = 16;


        private const int UpdateSongDataBase = 97;
        private const int SongList = 98;
        private const int AddToQueue = 99;

        private string commandToProcess;

        public void Start() {
            Thread t = new Thread(new ThreadStart(StartBlueTooth));
            t.Start();
        }

        public void StartBlueTooth()
        {
            try
            {
                if (BluetoothRadio.IsSupported)
                {
                   
                    if (BluetoothRadio.PrimaryRadio.Mode.Equals(RadioMode.PowerOff))
                    {
                        MessageBox.Show("Bluetooth divice is not enabled.");
                    }
                    else
                    {
                        BluetoothRadio.PrimaryRadio.Mode = RadioMode.Discoverable;
                        m_btListener = new BluetoothListener(new Guid("{418b27b0-b144-11e6-9598-0800200c9a66}"));
                        m_btListener.ServiceName = "KaraokeNow Bluetooth Service";
                        m_btListener.Authenticate = false;
                        m_btListener.Start(10);
                        m_btListener.BeginAcceptBluetoothClient(new AsyncCallback(AcceptConnection), m_btListener);
                    }
                }
                else
                {
                    MessageBox.Show("Bluetooth divice not found.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "StartBlueTooth", ex.LineNumber(), "BlueToothConnect");

            }
        }

        private async void AcceptConnection(IAsyncResult result)
        {
            if (result.IsCompleted)
            {
                m_btListener = (BluetoothListener)result.AsyncState;
                m_btListener.BeginAcceptBluetoothClient(new AsyncCallback(AcceptConnection), m_btListener);
                BluetoothClient remoteDevice = m_btListener.EndAcceptBluetoothClient(result);
                m_peerStream = remoteDevice.GetStream();

                ClientName = remoteDevice.RemoteMachineName;

                //keep connection open
                while (m_listening)
                {
                    try
                    {
                        string command = null;
                        byte[] buffer = new byte[1024];
                        int received = 0;
                        received = m_peerStream.Read(buffer, 0, buffer.Length);

                        if (received > 0)
                        {
                            command = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                            Console.WriteLine(command);

                            if (!TryToParse(command))
                            {
                                commandToProcess = command;
                                await ProcessCommand(AddToQueue);
                            }
                            else
                            {

                                if (m_parseResult == EXIT_CMD)
                                {
                                    //connection lost
                                   // m_peerStream.Close();
                                    remoteDevice.Close();
                                    break;
                                }
                                else
                                    await ProcessCommand(m_parseResult);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Logger.LogFile(ex.Message, "", "AcceptConnection", ex.LineNumber(), "BlueToothConnect");
                    }
                }
            }
        }

        private async Task ProcessCommand(int command)
        {
            var task = Task.Run(() =>
            {
                switch (command)
                {
                    case KEY_Up:
                        // bool s = await Send();
                        PlayerControl.VolumePlus();
                        break;
                    case KEY_Down:
                        PlayerControl.VolumeMinus();
                        break;
                    case PlayPause:
                        PlayerControl.PlayPause();
                        break;
                    case NextTrack:
                        PlayerControl.PlayNext();
                        break;
                    case VolumeMuteUnMute:
                        PlayerControl.VolumeControl();
                        break;
                    case IncMusicTempo:
                        PlayerControl.TempoPlus();
                        break;
                    case DecMusicTempo:
                        PlayerControl.TempoMinus();
                        break;
                    case IncMusicKey:
                        PlayerControl.KeyPlus();
                        break;
                    case DecMusicKey:
                        PlayerControl.KeyMinus();
                        break;
                    case AddToQueue:
                        if (!PlayerControl.AddRemoteSongToQueue(commandToProcess))
                            SendMessage(FileNotFound.ToString());
                        else
                            SendMessage(FileFound.ToString());

                        break;
                    case SongList:

                        break;
                    case UpdateSongDataBase:
                        break;
                    case NowPlaying:
                        string nowPlaying = PlayerControl.GetNowPlaying();
                       // Console.WriteLine(nowPlaying);
                        SendMessage(nowPlaying);
                        break;
                    case RemoveVocalNone:
                        PlayerControl.ChannelSelected = ChannelSelected.None;
                        PlayerControl.RemoveVocalLeftOrRight();
                        break;
                    case RemoveVocalRight:
                        PlayerControl.ChannelSelected = ChannelSelected.Right;
                        PlayerControl.RemoveVocalLeftOrRight();
                        break;
                    case RemoveVocalLeft:
                        PlayerControl.ChannelSelected = ChannelSelected.Left;
                        PlayerControl.RemoveVocalLeftOrRight();
                        break;
                    case RemoveVocal:
                        PlayerControl.RemoveVocalLeftOrRight();
                        break;
                    default:
                        break;
                }

            });
            await task;
        }

        private void SendMessage(string msg)
        {
            try
            {
                if (m_peerStream != null)
                {
                    UTF8Encoding encoder = new UTF8Encoding();
                    m_peerStream.Write(encoder.GetBytes(msg), 0, encoder.GetBytes(msg).Length);
                    m_peerStream.Flush();
                }
            }
            catch (Exception) {

            }
        }

        private void SendMessage(int msg)
        {
            try
            {
                if (m_peerStream != null)
                {
                    byte[] bytes = BitConverter.GetBytes(msg);
                    m_peerStream.Write(bytes, 0, bytes.Length);
                    m_peerStream.Flush();
                    UTF8Encoding encoder = new UTF8Encoding();
                }
            }
            catch (Exception)
            {

            }
        }

        private bool TryToParse(string stringToParse)
        {
            try
            {
                return int.TryParse(stringToParse, out m_parseResult);
            }
            catch (Exception ex)
            {
                return false;
            }
           // return false;
        }

        public void RemoveVocalLeftOrRight(ChannelSelected whatChannel) {
            switch (whatChannel) {
                case ChannelSelected.None:
                    SendMessage(RemoveVocalNone.ToString());
                    break;
                case ChannelSelected.Right:
                    SendMessage(RemoveVocalRight.ToString());
                    break;
                case ChannelSelected.Left:
                    SendMessage(RemoveVocalLeft.ToString());
                    break;
            }
        }


        public void UpdateSongsDataBase() {

            if (PlayerControl.MainFormControl.InvokeRequired)
            {
                PlayerControl.MainFormControl.BeginInvoke(new MethodInvoker(delegate () {
                    UpdateSongsDataBase();
                    return;
                }));
            }
          //  var dataTojason = new ;
            List<List<ListViewItem>> data = PlayerControl.UpdateSongsDatabase();
            List<TrackInfo> listTrackInfo = new List<TrackInfo>();
            foreach (List<ListViewItem> item in data)
            {
                listTrackInfo.AddRange(item.Select(s =>
                {
                    TrackInfo trackInfo = new TrackInfo();
                    trackInfo.ID = s.SubItems[0].Text.ToString();
                    trackInfo.Name = s.SubItems[1].Text.ToString();
                    trackInfo.Artist = s.SubItems[2].Text.ToString();
                    trackInfo.Duration = s.SubItems[3].Text.ToString();
                    trackInfo.FilePath = s.SubItems[4].Text.ToString();
                    return trackInfo;
                }));
            } 
        }

        public byte[] ToBinary(object objToXml)
        {

            using (var ms = new MemoryStream())
            {
                
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                binaryFormatter.Serialize(ms, objToXml);

                return ms.ToArray();
            }
        }

       /* private async Task<bool> Send()
        {
            // for not block the UI it will run in a different thread
            var task = Task.Run(() =>
            {
                try
                {
                    // if all is ok to send
                    if (remoteDevice.Connected && peerStream != null)
                    {
                        byte[] buffer = readByteArrayFromFile(@"C:\ProgramData\karaokeNow\JsonString\filename.json");//(@"D:\Eclipse\EXE\KaraFunDB.db");
                        byte[] comp = Compress(@"C:\ProgramData\karaokeNow\JsonString\filename.json");
                       // byte[] compD = CompressData();
                       peerStream.Write(comp, 0, comp.Length);
                       // Console.WriteLine(buffer.Length + " : " + comp.Length);
                        return true;
                    }


                }
                catch (Exception ex)
                {
                    Logger.LogFile(ex.Message, "", "Send", ex.LineNumber(), "BlueToothConnect");

                }

                return false;
            });
            return await task;
        }*/

        public static byte[] readByteArrayFromFile(string fileName)
        {
            try
            {
                if (System.IO.File.Exists(fileName))
                {

                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    long length = fs.Length;
                    int len = (int)length;
                    byte[] buff = new byte[len];
                    br.Read(buff, 0, len);
                    return buff;
                }

            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "readByteArrayFromFile", ex.LineNumber(), "BlueToothConnect");

            }
            return null;
        }

        public  byte[] Compress(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using (MemoryStream MS = new MemoryStream())
            {

                byte[] buffer = new byte[fs.Length];
                int byteRead = 0;

                ZipOutputStream zipOutputStream = new ZipOutputStream(MS);
                zipOutputStream.SetLevel(9); //Set the compression level(0-9)
                ZipEntry entry = new ZipEntry("zip");//Create a file that needs to be compressed
                zipOutputStream.PutNextEntry(entry);//put the entry in zip

                //Writes the data into file in memory stream for compression 
                while ((byteRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                    zipOutputStream.Write(buffer, 0, byteRead);

                zipOutputStream.IsStreamOwner = false;
                fs.Close();
                zipOutputStream.Close();
                MS.Position = 0;

                return MS.ToArray();
            }

        }

       /* public byte[] CompressData()
        {

            using (MemoryStream MS1 = ToJson())
            {

                byte[] buffer = new byte[MS1.Length];
                int byteRead = 0;


                MemoryStream MS2 = new MemoryStream();
                ZipOutputStream zipOutputStream = new ZipOutputStream(MS2);
                zipOutputStream.SetLevel(9); //Set the compression level(0-9)
                ZipEntry entry = new ZipEntry("");//Create a file that is needs to be compressed
                zipOutputStream.PutNextEntry(entry);//put the entry in zip

                //Writes the data into file in memory stream for compression 
                while ((byteRead = MS1.Read(buffer, 0, buffer.Length)) > 0)
                    zipOutputStream.Write(buffer, 0, byteRead);

                zipOutputStream.IsStreamOwner = false;
                MS1.Close();
                zipOutputStream.Close();
                MS2.Position = 0;

                return MS2.ToArray();
            }

        }*/


        public string ClientName { get; private set; }

    }
}
