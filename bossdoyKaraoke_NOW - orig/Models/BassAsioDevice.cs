using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Models.VocalEffects;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.Misc;
using Un4seen.BassAsio;

namespace bossdoyKaraoke_NOW.Models
{
    class BassAsioDevice : Mixer
    {
        private static Control thisControl;
        private static int m_mixerChannel = 0;
        private static int m_inputChannel;
        private static int m_outputChannel;
        private static double samplerate = 0;
        private static ASIOPROC _asioProc;
        private static BASS_ASIO_DEVICEINFO devinfo;
        private static BassAsioHandler m_asioInputHandlers;
        private static BassAsioHandler m_asioOutputHandlers;
        private DSP_PeakLevelMeter m_plmInput;
        private DSP_PeakLevelMeter m_plmOutput;
        public static float m_micVolume = 0.05f;
        public static int StreamInputVlc;
        private static int m_streamInputMic;
        private static int m_input_selectedDev = 0;
        private static int m_output_selectedDev = 0;
        private static int[] m_stream = new int[4];
        private static List<object> m_asioInputChannels;
        private static List<object> m_asioOutputChannels;
        private static int m_vlcAsioInputChannel;

        private static bool m_bassinit;
        private static int m_asiodevice;
        private static double m_samplerate;
        private static string f = @"D:\tanie\karaokeNow Resources\LdVocal_01.wav";

        public static void BassDispose()
        {
            Bass.BASS_StreamFree(m_streamInputMic);
            Bass.BASS_StreamFree(StreamInputVlc);
            Bass.BASS_Free();
        }

        public static void Start()
        {        
            BassAsio.BASS_ASIO_SetDevice(m_asiodevice);
            BassAsio.BASS_ASIO_Start(0);
        }

        public static void ReStart()
        {
            Stop();
            SetDevice(m_inputChannel, m_outputChannel);
            Start();
        }

        public static void CreateInputs(int CountOfInputs)
        {
            m_asioInputHandlers = new BassAsioHandler(true, m_asiodevice, CountOfInputs, 2, BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT, m_samplerate);
            m_asioInputHandlers.UseInput = false;
            m_asioInputHandlers.SetFullDuplex(0, BASSFlag.BASS_STREAM_DECODE, false);
            m_asioInputHandlers.BypassFullDuplex = false;
            m_asioInputHandlers.Volume = PlayerControl.MicVolumeScroll.Value * 0.01f;
            var error = Bass.BASS_ErrorGetCode();
            Console.WriteLine("CreateIn: {0}", error);
        }

        public static void CreateOutput(int CountOfOutputs)
        {
           /* Player.Mixer = MixerStreamCreate((int)samplerate);
            if (Player.Mixer == 0)
            {
                var error = Bass.BASS_ErrorGetCode();
                MessageBox.Show(error.ToString(), "Could not create mixer!");
                Bass.BASS_Free();
                return;
            }*/

           // m_mixerChannel = Player.Mixer;
            m_asioOutputHandlers = new BassAsioHandler(m_asiodevice, CountOfOutputs, m_mixerChannel);
        }

        public static void Connect()
        {
            Bass.BASS_ChannelSetPosition(m_asioInputHandlers.OutputChannel, 0.0);
           // BassMix.BASS_Mixer_StreamAddChannel(m_asioOutputHandlers.OutputChannel, m_asioInputHandlers.OutputChannel, BASSFlag.BASS_MIXER_DOWNMIX | BASSFlag.BASS_STREAM_AUTOFREE);          
             var error = Bass.BASS_ErrorGetCode();
             Console.WriteLine("Start: {0}", error);
        }

        public static void Stop()
        {
            BassAsio.BASS_ASIO_Stop();
            BassAsio.BASS_ASIO_Free();
        }

        public static void SetDevice(int inputChannel, int outputChannel)
        {

          //  Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0);
         //   Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_FLOATDSP, true);
           // BassAsio.BASS_ASIO_Init(0, BASSASIOInit.BASS_ASIO_DEFAULT);
         //   Bass.BASS_Init(0, (int)samplerate, 0, IntPtr.Zero);

         //   m_asioInputHandlers = new BassAsioHandler();
         //   m_asioOutputHandlers = new BassAsioHandler();
            devinfo = new BASS_ASIO_DEVICEINFO();
            m_asioInputChannels = new List<object>();
            m_asioOutputChannels = new List<object>();
            thisControl = PlayerControl.MainFormControl;
            m_inputChannel = inputChannel;
            m_outputChannel = outputChannel;

         /*   for (int n = 0; BassAsio.BASS_ASIO_GetDeviceInfo(n, devinfo); n++)
            {
                if (devinfo.name.Contains("ASIO4ALL"))
                {
                    if (!BassAsio.BASS_ASIO_IsStarted())
                        BassAsio.BASS_ASIO_Init(n, BASSASIOInit.BASS_ASIO_DEFAULT);

                    samplerate = BassAsio.BASS_ASIO_GetRate();
                    m_asiodevice = n;

                    BASS_ASIO_INFO asioinfo = BassAsio.BASS_ASIO_GetInfo();
                    if (asioinfo != null)
                    {
                        // assuming stereo input
                        for (int i = 0; i < asioinfo.inputs; i += 2)
                        {
                            BASS_ASIO_CHANNELINFO chanInfo = BassAsio.BASS_ASIO_ChannelGetInfo(true, i);
                            if (chanInfo != null)
                                GetAsioInputChannels.Add(chanInfo);

                            if (chanInfo.name.Contains("VB-Audio Point 1"))
                                GetVlcAsioInputChannel = i;
                        }

                        for (int o = 0; o < asioinfo.outputs; o += 2)
                        {
                            BASS_ASIO_CHANNELINFO chanInfo = BassAsio.BASS_ASIO_ChannelGetInfo(false, o);
                            if (chanInfo != null)
                                GetAsioOutputChannels.Add(chanInfo);
                        }
                    }
                }

            }

            CreateInputs(m_inputChannel);
            CreateOutput(m_outputChannel);
            Connect();
            int ggg = 0;*/


              for (int n = 0; BassAsio.BASS_ASIO_GetDeviceInfo(n, devinfo); n++)
              {
                  if (devinfo.name.Contains("ASIO4ALL"))
                  {
                      if (!BassAsio.BASS_ASIO_IsStarted())
                          BassAsio.BASS_ASIO_Init(n, BASSASIOInit.BASS_ASIO_DEFAULT);
                      samplerate = BassAsio.BASS_ASIO_GetRate();
                      m_asiodevice = n;

                      BASS_ASIO_INFO asioinfo = BassAsio.BASS_ASIO_GetInfo();
                      if (asioinfo != null)
                      {
                          // assuming stereo input
                          for (int i = 0;i < asioinfo.inputs; i += 2)
                          {
                              BASS_ASIO_CHANNELINFO chanInfo = BassAsio.BASS_ASIO_ChannelGetInfo(true, i);
                              if (chanInfo != null)
                                  GetAsioInputChannels.Add(chanInfo);

                              if (chanInfo.name.Contains("VB-Audio Point 1"))
                                  GetVlcAsioInputChannel = i;
                          }

                          for (int o = 0; o < asioinfo.outputs; o += 2)
                          {
                              BASS_ASIO_CHANNELINFO chanInfo = BassAsio.BASS_ASIO_ChannelGetInfo(false, o);
                              if (chanInfo != null)
                                  GetAsioOutputChannels.Add(chanInfo);
                          }

                      }

                      BassAsio.BASS_ASIO_ChannelSetVolume(true, m_inputChannel, PlayerControl.MicVolumeScroll.Value * 0.01f);
                      BassAsio.BASS_ASIO_ChannelSetVolume(true, m_inputChannel + 1, PlayerControl.MicVolumeScroll.Value * 0.01f);

                      _asioProc = new ASIOPROC(AsioCallback);
                  }

              }

            if (!m_bassinit)
            {

                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0);
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_FLOATDSP, true);
                Bass.BASS_Init(0, (int)samplerate, 0, IntPtr.Zero);

                Player.Mixer = MixerStreamCreate((int)samplerate);

                if (Player.Mixer == 0)
                {
                    var error = Bass.BASS_ErrorGetCode();
                    MessageBox.Show(error.ToString(), "Could not create mixer!");
                    Bass.BASS_Free();
                    return;
                }

                m_mixerChannel = Player.Mixer;

                m_streamInputMic = Bass.BASS_StreamCreatePush((int)samplerate, 2, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT, IntPtr.Zero);
                StreamInputVlc = Bass.BASS_StreamCreatePush((int)samplerate, 2, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT, IntPtr.Zero);

                //m_streamInputMic = Bass.BASS_StreamCreateFile(f, 0L, 0L, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_SAMPLE_LOOP);
                m_stream[0] = BassMix.BASS_Split_StreamCreate(m_streamInputMic, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null);//_instream;
                m_stream[1] = BassMix.BASS_Split_StreamCreate(m_streamInputMic, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null);// _instream;
                m_stream[2] = BassMix.BASS_Split_StreamCreate(m_streamInputMic, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null);//_instream;
                m_stream[3] = BassMix.BASS_Split_StreamCreate(m_streamInputMic, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null);//_instream;


                //Group 1 VST Effects ============================================================
                Channel1Fx.SetEffects(m_stream[0]);


                //Group 2 VST Effects ============================================================
                Channel2Fx.SetEffects(m_stream[1]);


                //Group 3 VST Effects ============================================================
                Channel3Fx.SetEffects(m_stream[2]);


                //Group 4 VST Effects ============================================================
                Channel4Fx.SetEffects(m_stream[3]);

                BassMix.BASS_Mixer_StreamAddChannel(m_mixerChannel, StreamInputVlc, BASSFlag.BASS_MIXER_DOWNMIX | BASSFlag.BASS_STREAM_AUTOFREE);
                BassMix.BASS_Mixer_StreamAddChannel(m_mixerChannel, m_stream[0], BASSFlag.BASS_MIXER_DOWNMIX | BASSFlag.BASS_STREAM_AUTOFREE);
                BassMix.BASS_Mixer_StreamAddChannel(m_mixerChannel, m_stream[1], BASSFlag.BASS_MIXER_DOWNMIX | BASSFlag.BASS_STREAM_AUTOFREE);
                BassMix.BASS_Mixer_StreamAddChannel(m_mixerChannel, m_stream[2], BASSFlag.BASS_MIXER_DOWNMIX | BASSFlag.BASS_STREAM_AUTOFREE);
                BassMix.BASS_Mixer_StreamAddChannel(m_mixerChannel, m_stream[3], BASSFlag.BASS_MIXER_DOWNMIX | BASSFlag.BASS_STREAM_AUTOFREE);
                BASS_CHANNELINFO info = Bass.BASS_ChannelGetInfo(m_mixerChannel);

                m_bassinit = true;
            }

              //BassAsio.BASS_ASIO_ControlPanel();
              BassAsio.BASS_ASIO_ChannelEnable(true, GetVlcAsioInputChannel, _asioProc, new IntPtr(StreamInputVlc));
              BassAsio.BASS_ASIO_ChannelJoin(true, GetVlcAsioInputChannel + 1, GetVlcAsioInputChannel);
              BassAsio.BASS_ASIO_ChannelSetFormat(true, GetVlcAsioInputChannel, BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT);
              BassAsio.BASS_ASIO_ChannelSetRate(true, GetVlcAsioInputChannel, samplerate);
              BassAsio.BASS_ASIO_SetRate(samplerate);

              BassAsio.BASS_ASIO_ChannelEnable(true, m_inputChannel, _asioProc, new IntPtr(m_streamInputMic));
              BassAsio.BASS_ASIO_ChannelJoin(true, m_inputChannel + 1, m_inputChannel);
              BassAsio.BASS_ASIO_ChannelSetFormat(true, m_inputChannel, BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT);
              BassAsio.BASS_ASIO_ChannelSetRate(true, m_inputChannel, samplerate);
              BassAsio.BASS_ASIO_SetRate(samplerate);

              BassAsio.BASS_ASIO_ChannelEnable(false, m_outputChannel, _asioProc, new IntPtr(m_mixerChannel));
              BassAsio.BASS_ASIO_ChannelJoin(false, m_outputChannel + 1, m_outputChannel);
              BassAsio.BASS_ASIO_ChannelSetFormat(false, m_outputChannel, BASSASIOFormat.BASS_ASIO_FORMAT_FLOAT);
              BassAsio.BASS_ASIO_ChannelSetRate(false, m_outputChannel, samplerate);
              BassAsio.BASS_ASIO_SetRate(samplerate);
        }

        private static int AsioCallback(bool input, int channel, IntPtr buffer, int length, IntPtr user)
        {
            try
            {
                int sData;

                if (input)
                {
                    Bass.BASS_StreamPutData(user.ToInt32(), buffer, length);
                }
                else
                {
                    sData = Bass.BASS_ChannelGetData(user.ToInt32(), buffer, length);
                    return sData;
                }
            }
            catch (AccessViolationException)
            { }
            return 0;
        }

        private static void GetAsioDevices()
        {
          /*  BASS_ASIO_DEVICEINFO devinfo = new BASS_ASIO_DEVICEINFO();
            m_asioInputChannels = new List<object>();
            m_asioOutputChannels = new List<object>();

            for (int i = 0; BassAsio.BASS_ASIO_GetDeviceInfo(i, devinfo); i++)
            {
                if (devinfo.name.Contains("ASIO4ALL"))
                {
                    if (!BassAsio.BASS_ASIO_IsStarted())
                        BassAsio.BASS_ASIO_Init(i, BASSASIOInit.BASS_ASIO_DEFAULT);

                    BASS_ASIO_INFO info = BassAsio.BASS_ASIO_GetInfo();
                    if (info != null)
                    {
                        // assuming stereo input
                        for (int n = 0; n < info.inputs; n += 2)
                        {
                            BASS_ASIO_CHANNELINFO chanInfo = BassAsio.BASS_ASIO_ChannelGetInfo(true, n);
                            if (chanInfo != null)
                                GetAsioInputChannels.Add(chanInfo);

                            if (chanInfo.name.Contains("VB-Audio Point 1"))
                                GetVlcAsioInputChannel = n;
                        }

                        for (int n = 0; n < info.outputs; n += 2)
                        {
                            BASS_ASIO_CHANNELINFO chanInfo = BassAsio.BASS_ASIO_ChannelGetInfo(false, n);
                            if (chanInfo != null)
                                GetAsioOutputChannels.Add(chanInfo);
                        }

                    }
                }
            }*/
        }

        public static void MicVolumeTrack()
        {
            if (thisControl.InvokeRequired)
            {
                thisControl.Invoke(new Action(() =>
                {
                    MicVolumeTrack();
                }));
                return;
            }

            Mic_Volume = PlayerControl.MicVolumeScroll.Value * 0.01f;
        }

        private static float Mic_Volume
        {
            get
            {
                return m_micVolume;
            }
            set
            {
                m_micVolume = value;
                BassAsio.BASS_ASIO_ChannelSetVolume(true, m_inputChannel, value);
                BassAsio.BASS_ASIO_ChannelSetVolume(true, m_inputChannel + 1, value);
            }
        }

        public static List<object> GetAsioInputChannels 
        {
            get { return m_asioInputChannels;  }
            set { m_asioInputChannels = value; }
        }

        public static List<object> GetAsioOutputChannels
        {
            get { return m_asioOutputChannels; }
            set { m_asioOutputChannels = value; }
        }

        public static int GetVlcAsioInputChannel
        {
            get { return m_vlcAsioInputChannel; }
            set { m_vlcAsioInputChannel = value; }
        }

        public static int asioChannel
        {
            get
            {
                return m_outputChannel;
            }
        }

        public static int inputDevice {
            get
            {
                return m_input_selectedDev;
            }
            set
            {
                m_input_selectedDev = value;
            }
        }
        public static int outputDevice
        {
            get
            {
                return m_output_selectedDev;
            }
            set
            {
                m_output_selectedDev = value;
            }
        }
    }
}
