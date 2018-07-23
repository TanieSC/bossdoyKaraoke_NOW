using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bossdoyKaraoke_NOW.Adb
{
    public class AdbClient : IDisposable
    {
        string m_filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\karaokeNow\songs";
        string apppath = AppDomain.CurrentDomain.BaseDirectory + @"mobile_app\karaokeNow.apk";
        System.Net.Sockets.TcpClient clientSocket = new TcpClient();


        private UserControl mUsbControl;
        private DataReceivedEventArgs dreArgs;
        private Stream outStream;
        private List<string> devices;
      //  private const int BUFFER_SIZE = 32768;


        public static AdbClient Instance { get; set; } = new AdbClient();

        public AdbClient()
        {
           // mUsbControl = new UserControl();
        }


        public bool AdbStart()
        {
            int status = StartADBCommannds("start-server", null, null);
            if (status == 0)
               status = StartADBCommannds("forward tcp:38300 tcp:38300", null, null);

            return status != 0 ? false : true;
        }

        public bool CheckForSongUpdate(string date)
        {

            var directory =  new DirectoryInfo(m_filePath);
            DateTime from_date = DateTime.Parse(date);
            var files = directory.EnumerateFiles("*.bkN", SearchOption.AllDirectories).Where(file => file.LastWriteTime > from_date).ToList();

            return (files.Count() >= 1 ? true : false);

        }

        public int InstallApp() {
            int status = StartADBCommannds("install -r " + apppath, null, null);
            return status;
        }

        public List<adbDevices> AdbGetDevices()
        {

            devices = new List<string>();

            StartADBCommannds("devices -l", null, devices);

            return devices.Select(d =>
            {
                d.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                return adbDevices.CreateDeviceList(d);
            }).Where(d => d != null).ToList();
        }

        public async Task<bool> SendData()
        {
            bool ret = false;

            try
            {
                await clientSocket.ConnectAsync("127.0.0.1", 38300);
                ret = clientSocket.Connected;
            }
            catch (SocketException se)
            {

            }
            catch (ObjectDisposedException odse)
            {
                clientSocket.Close();
                clientSocket = new TcpClient();
                await clientSocket.ConnectAsync("127.0.0.1", 38300);
                ret = clientSocket.Connected;
            }

            if (ret)
            {

                NetworkStream serverStream = clientSocket.GetStream();
                byte[] inStream = new byte[clientSocket.ReceiveBufferSize];
                serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream);

                List<string> songs = new List<string>();
                songs = Directory.EnumerateFiles(m_filePath, "*.bkN", SearchOption.AllDirectories).ToList();

                for (int i = 0; i < songs.Count; i++)
                {
                    using (var fileStream = new FileStream(songs[i], FileMode.Open, FileAccess.Read))
                    {
                        try
                        {
                            byte[] outStream = StreamToByteArray(fileStream);
                            serverStream.Write(outStream, 0, outStream.Length);
                            serverStream.Flush();
                        }
                        catch (IOException ioe)
                        {
                            serverStream.Dispose();
                            clientSocket.Close();
                            break; //no active android server host running
                        }
                    }
                }

                serverStream.Dispose();
                serverStream.Close();
                clientSocket.Close();
                StartADBCommannds("forward --remove tcp:38300", null, null);
            }


            return ret;
        }


        public void msg(TextBox textbox, string msg)
        {
            textbox.Text = textbox.Text + Environment.NewLine + " >> " + msg;
        }

        public int StartADBCommannds(string arg, List<string> errorOutput, List<string> standardOutput)
        {

            int status = ExecuteCommand(arg, errorOutput, standardOutput);

            if (status == 0)
            {
                return status;
            }

            foreach (var adbProcess in Process.GetProcessesByName("adb"))
            {
                try
                {
                    adbProcess.Kill();
                }
                catch (Win32Exception)
                {

                }
                catch (InvalidOperationException)
                {

                }
            }

            status = ExecuteCommand(arg, errorOutput, standardOutput);

            return status;
        }

        public int ExecuteCommand(string arg, List<string> errorOutput, List<string> standardOutput)
        {

            int status;

            string adbPath = AppDomain.CurrentDomain.BaseDirectory + "adb.exe";

            ProcessStartInfo psi = new ProcessStartInfo(adbPath, arg)
            {
                CreateNoWindow = true,

#if !NETSTANDARD1_3
                WindowStyle = ProcessWindowStyle.Hidden,
#endif

                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using (Process process = Process.Start(psi))
            {
                var ErrorData = process.StandardError.ReadToEnd();
                var OutputData = process.StandardOutput.ReadToEnd();

                if (errorOutput != null)
                {
                    errorOutput.AddRange(ErrorData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                }

                if (standardOutput != null)
                {
                    standardOutput.AddRange(OutputData.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                }

                if (!process.WaitForExit(5000))
                {
                    process.Kill();
                }

                //process.WaitForExit();

                status = process.ExitCode;
            }

            //  if (status != 0)
            //  {
            //      throw new Exception($"The adb process returned error code {status} when running command {arg}");
            //  }

            return status;
        }



        static byte[] StreamToByteArray(Stream inputStream)

        {
            if (!inputStream.CanRead)
            {
                throw new ArgumentException();
            }

            // This is optional
            if (inputStream.CanSeek)
            {
                inputStream.Seek(0, SeekOrigin.Begin);
            }

            byte[] output = new byte[inputStream.Length];
            int bytesRead = inputStream.Read(output, 0, output.Length);

            return output;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    clientSocket.Close();
                    mUsbControl.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AdbClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion


        /* public void Dispose()
         {
             clientSocket.Close();
             mUsbControl.Dispose();
         }

         ~AdbClient()
         {
             this.Dispose();
         }*/

    }
}
