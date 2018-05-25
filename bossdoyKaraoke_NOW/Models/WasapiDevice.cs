using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.BassWasapi;
using Un4seen.Bass.AddOn.Vst;
using System.ComponentModel;
using System.Drawing;
using bossdoyKaraoke_NOW.Models.VocalEffects;

namespace bossdoyKaraoke_NOW.Models
{
    class WasapiDevice : Mixer
    {

        public static bool BassInit { get; private set; }
        public static int InputDevice { get; private set; }
        public static int OutputDevice { get; private set; }
        public static string InputDeviceName { get; private set; }
        public static string OutputDeviceName { get; private set; }
        public static BASS_WASAPI_DEVICEINFO InputDeviceInfo { get; private set; }
        public static BASS_WASAPI_DEVICEINFO OutputDeviceInfo { get; private set; }
        public static int Mixer { get; private set; } //for testing purpose only


        private static int[] _stream = new int[4];
        private static Control thisControl;
        public static float MicVolume = 0.05f;
        public static float Reverb = 0.05f;
        private static int _instream;
        private static int _outstream;
        private static bool m_isWasapiInit;
        private static WASAPIPROC _wasapiInProc;
        private static WASAPIPROC _wasapiOutProc;

        public static void Start()
        {
            // Start WASAPI
            BassWasapi.BASS_WASAPI_SetDevice(InputDevice);
            BassWasapi.BASS_WASAPI_Start();
            BassWasapi.BASS_WASAPI_SetDevice(OutputDevice);
            BassWasapi.BASS_WASAPI_Start();
        }

        public static void Stop()
        {
            // Stop WASAPI
            BassWasapi.BASS_WASAPI_SetDevice(InputDevice);
            BassWasapi.BASS_WASAPI_Stop(false);
            BassWasapi.BASS_WASAPI_SetDevice(OutputDevice);
            BassWasapi.BASS_WASAPI_Stop(false);
        }

        public static void SetDevice(int inputDevice, int outputDevice)
        {
            thisControl = PlayerControl.MainFormControl;

            //Get some info about their selected input device
            var inputdeviceinfo = BassWasapi.BASS_WASAPI_GetDeviceInfo(inputDevice);
            InputDevice = inputDevice;
            InputDeviceInfo = inputdeviceinfo;

            //Get some info about their selected ouput device
            var outputdeviceinfo = BassWasapi.BASS_WASAPI_GetDeviceInfo(outputDevice);
            OutputDevice = outputDevice;
            OutputDeviceInfo = outputdeviceinfo;


            if (!m_isWasapiInit)
            {

                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 0);
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_FLOATDSP, true);
                Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);

                Player.Mixer = MixerStreamCreate(outputdeviceinfo.mixfreq);

                if (Player.Mixer == 0)
                {
                    var error = Bass.BASS_ErrorGetCode();
                    MessageBox.Show(error.ToString(), "Could not create mixer!");
                    Bass.BASS_Free();
                    return;
                }

                _outstream = Player.Mixer;

                _instream = Bass.BASS_StreamCreatePush(inputdeviceinfo.mixfreq, inputdeviceinfo.mixchans, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, IntPtr.Zero);
                // create a stream to receive the data
                // string fn = @"D:\tanie\karaokeNow Resources\LdVocal_01.wav";
                // _instream = Bass.BASS_StreamCreateFile(fn, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_LOOP);

                //_stream[0] = BassMix.BASS_Split_StreamCreate(_instream, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null);//_instream;
                //_stream[1] = BassMix.BASS_Split_StreamCreate(_instream, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null);// _instream;
                //_stream[2] = BassMix.BASS_Split_StreamCreate(_instream, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null);//_instream;
                //_stream[3] = BassMix.BASS_Split_StreamCreate(_instream, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE, null);//_instream;


                //  _outstream = BassMix.BASS_Mixer_StreamCreate(outputdeviceinfo.mixfreq, outputdeviceinfo.mixchans, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE);
                //  Player.Mixer = _outstream;

                //Group 1 VST Effects ============================================================
                Channel1Fx.SetEffects(_instream);


                //Group 2 VST Effects ============================================================
               // Channel2Fx.SetEffects(_stream[1]);


                //Group 3 VST Effects ============================================================
                //Channel3Fx.SetEffects(_stream[2]);


                //Group 4 VST Effects ============================================================
                //Channel4Fx.SetEffects(_stream[3]);

                BassMix.BASS_Mixer_StreamAddChannel(_outstream, _instream, 0);
                // BassMix.BASS_Mixer_StreamAddChannel(_outstream, _stream[1], 0);
                //BassMix.BASS_Mixer_StreamAddChannel(_outstream, _stream[2], 0);
                // BassMix.BASS_Mixer_StreamAddChannel(_outstream, _stream[3], 0);

                m_isWasapiInit = true;
            }

            _wasapiInProc = new WASAPIPROC(InputProc);
            GC.KeepAlive(_wasapiInProc);


            _wasapiOutProc = new WASAPIPROC(OutputProc);
            GC.KeepAlive(_wasapiOutProc);

            //Set the input device so subsequent calls are on it
            BassWasapi.BASS_WASAPI_SetDevice(InputDevice);
            // Initialize BASS WASAPI input
            BASS_WASAPI_INFO input = new BASS_WASAPI_INFO();
            BassWasapi.BASS_WASAPI_GetInfo(input);

            BassWasapi.BASS_WASAPI_Init(inputDevice, inputdeviceinfo.mixfreq, inputdeviceinfo.mixchans,
                             BASSWASAPIInit.BASS_WASAPI_EVENT | BASSWASAPIInit.BASS_WASAPI_BUFFER | BASSWASAPIInit.BASS_WASAPI_SHARED, input.buflen, 0, _wasapiInProc, IntPtr.Zero);

            //Set the output device so subsequent calls are on it
            BassWasapi.BASS_WASAPI_SetDevice(OutputDevice);
            BASS_WASAPI_INFO output = new BASS_WASAPI_INFO();
            BassWasapi.BASS_WASAPI_GetInfo(output);

            // Initialize BASS WASAPI output
            BassWasapi.BASS_WASAPI_Init(outputDevice, outputdeviceinfo.mixfreq, outputdeviceinfo.mixchans,
                            BASSWASAPIInit.BASS_WASAPI_EVENT | BASSWASAPIInit.BASS_WASAPI_SHARED, output.buflen, 0, _wasapiOutProc, IntPtr.Zero);

        }

        public static int GetOutputDefaultDevice()
        {
            BASS_WASAPI_DEVICEINFO[] wasapiDevices = BassWasapi.BASS_WASAPI_GetDeviceInfos();
            int outputItem = 0;
            for (int i = 0; i < wasapiDevices.Length; i++)
            {
                BASS_WASAPI_DEVICEINFO info = wasapiDevices[i];

                if (!info.IsInput && info.IsDefault)
                {
                    outputItem = i;
                    Player.DefaultDeviceLongName = info.name;
                    break;
                }
            }

            return outputItem;
        }


        public static int GetInputDefaultDevice()
        {
            BASS_WASAPI_DEVICEINFO[] wasapiDevices = BassWasapi.BASS_WASAPI_GetDeviceInfos();
            int inputItem = 0;
            for (int i = 0; i < wasapiDevices.Length; i++)
            {
                BASS_WASAPI_DEVICEINFO info = wasapiDevices[i];

                if (info.IsInput && info.IsDefault)
                {
                    inputItem = i;
                    break;
                }
            }

            return inputItem;
        }

        private static int OutputProc(IntPtr buffer, int length, IntPtr user)
        {
            return Bass.BASS_ChannelGetData(_outstream, buffer, length);
        }

        private static int InputProc(IntPtr buffer, int length, IntPtr user)
        {
            return Convert.ToInt32(Bass.BASS_StreamPutData(_instream, buffer, length) != -1);
        }
    }
}
