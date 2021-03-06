﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using static bossdoyKaraoke_NOW.Enums.PlayerState;

namespace bossdoyKaraoke_NOW.Models
{

    class Player : Mixer
    {
        private float m_playerVolume = 0.05f;
        public FxTempo Tempo = null;
        public int Channel = 0;
        public TAG_INFO Tags = null;
        public long TrackLength = 0L;
        public SYNCPROC TrackSync = null;
        public int NextTrackSync = 0;
        public long NextTrackPos = 0L;

        private static ASIOPROC _asioProc;
        private static BASS_ASIO_CHANNELINFO chaninfo;
        private static int chan = 0;
        private static int m_defaultdevice = 0;
        private static string m_defaultdevicelongname;
        private static int m_mixerChannel = 0;
        private bool m_mute;
        public static float m_micVolume = 0.05f;

        public PlayState PlayState { get; private set; }

        public Player() {

          //  m_equalizer = new Equalizer();
        }

        public void CreateStream()
        {
            if (!File.Exists(Tags.filename)) { throw new FileNotFoundException(Tags.filename); }

            Tempo = new FxTempo();
            this.Channel = Bass.BASS_StreamCreateFile(Tags.filename, 0L, 0L, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_STREAM_PRESCAN);
            Tempo.Channel = this.Channel;
            this.Channel = Tempo.TempoCreate();

            if (this.Channel != 0)
            {
                StreamAddChannel(this.Channel, TrackSync);
                TrackLength = Bass.BASS_ChannelGetLength(Channel);
            }

            this.PlayState = PlayState.Stopped;

        }

        public static void BassInitialize()
        {
            if (IsBassInitialized) { return; }

            IsBassInitialized = true;
            IsAsioInitialized = false;
            IsWasapiInitialized = false;


            BassNet.Registration("tanie_calacar@yahoo.com", "2X183372334322");
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 200);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 20);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_FLOATDSP, true);

            BASS_DEVICEINFO info = new BASS_DEVICEINFO();
            for (int n = 0; Bass.BASS_GetDeviceInfo(n, info); n++)
            {

              //  Console.WriteLine(info.ToString());

                if (info.IsEnabled && info.IsDefault)
                {
                    Console.WriteLine(info.ToString());
                    m_defaultdevicelongname = info.name;
                    Bass.BASS_SetDevice(n);
                    m_defaultdevice = n;
                }

                if (!Bass.BASS_Init(n, 44100, BASSInit.BASS_DEVICE_DEFAULT, PlayerControl.MainFormControl.Handle))
                {
                    var error = Bass.BASS_ErrorGetCode();
                    MessageBox.Show(error.ToString(), "Bass_Init error!");
                   // return;
                }
            }

            // already create a mixer
            //m_mixerChannel = BassMix.BASS_Mixer_StreamCreate(44100, 2, BASSFlag.BASS_SAMPLE_FLOAT);

            m_mixerChannel = MixerStreamCreate(44100);

            if (m_mixerChannel == 0)
            {
                var error = Bass.BASS_ErrorGetCode();
                MessageBox.Show(error.ToString(), "Could not create mixer!");
                Bass.BASS_Free();
                return;
            }

        }

        public static void AsioInitialize()
        {
            if (IsAsioInitialized) { return; }

            BassNet.Registration("tanie_calacar@yahoo.com", "2X183372334322");
            IsAsioInitialized = true;
            IsBassInitialized = false;
            IsWasapiInitialized = false;
            BassAsioDevice.SetDevice(0, 0);
            BassAsioDevice.Start();
        }

        public static void WasapiInitialize() {

            if (IsWasapiInitialized) { return; }

            BassNet.Registration("tanie_calacar@yahoo.com", "2X183372334322");
            IsWasapiInitialized = true;
            IsBassInitialized = false;
            IsAsioInitialized = false;
            WasapiDevice.SetDevice(WasapiDevice.GetInputDefaultDevice(), WasapiDevice.GetOutputDefaultDevice());
            WasapiDevice.Start();
        }

        public static int BassVlcStreamHandle { get; set;}
        public static int ChannelFreq { get; set; }
        public static int Channels { get; set; }
        public static int DefaultDevice { get { return m_defaultdevice; } set { m_defaultdevice = value; } }
        public static string DefaultDeviceLongName { get { return m_defaultdevicelongname; } set { m_defaultdevicelongname = value; } }
        public static int Mixer
        {
            get
            {
                return m_mixerChannel;
            }
            set
            {
                m_mixerChannel = value;

            }
        }

        public static bool IsBassInitialized { get; set; }
        public static bool IsWasapiInitialized { get; set; }
        public static bool IsAsioInitialized { get; set; }

        public void Dispose()
        {
            if (this.Channel != 0)
            {
                Bass.BASS_StreamFree(this.Channel);
                Bass.BASS_Free();
            }else
                Bass.BASS_Free();
        }


        public void Play()
        {
            if (this.PlayState != PlayState.Playing)
            {
                this.PlayState = PlayState.Playing;
                BassMix.BASS_Mixer_ChannelPlay(this.Channel);
            }
        }

        public void Pause()
        {
            if (this.PlayState == PlayState.Playing)
            {
                this.PlayState = PlayState.Paused;
                BassMix.BASS_Mixer_ChannelPause(this.Channel);
            }
        }

        public void Stop()
        {
            if (this.PlayState == PlayState.Playing)
            {
                this.PlayState = PlayState.Stopped;
                Bass.BASS_StreamFree(this.Channel);


            }
        }

        public void Mute()
        {
            if (this.PlayState == PlayState.Playing)
            {
                if (!m_mute)
                {
                    Bass.BASS_ChannelSetAttribute(this.Channel, BASSAttribute.BASS_ATTRIB_VOL, 0f);
                    m_mute = true;
                }
                else
                {
                    Bass.BASS_ChannelSetAttribute(this.Channel, BASSAttribute.BASS_ATTRIB_VOL, m_playerVolume);
                    m_mute = false;
                }
            }
        }

        public float Volume
        {
            get
            {
                return m_playerVolume;
            }
            set
            {
                m_playerVolume = value;
                Bass.BASS_ChannelSetAttribute(this.Channel, BASSAttribute.BASS_ATTRIB_VOL, value);
            }
        }

        public static void MicVolumeTrack()
        {
            if (PlayerControl.MainFormControl.InvokeRequired)
            {
                PlayerControl.MainFormControl.Invoke(new Action(() =>
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
                if (Player.IsAsioInitialized)
                {
                    BassAsio.BASS_ASIO_ChannelSetVolume(true, BassAsioDevice.asioInputChannel, value);
                    BassAsio.BASS_ASIO_ChannelSetVolume(true, BassAsioDevice.asioInputChannel + 1, value);
                }
                else if(IsWasapiInitialized)
                {
                    BassWasapi.BASS_WASAPI_SetVolume(BASSWASAPIVolume.BASS_WASAPI_VOL_DEVICE | BASSWASAPIVolume.BASS_WASAPI_CURVE_WINDOWS, value);
                }
            }
        }

    }
}
