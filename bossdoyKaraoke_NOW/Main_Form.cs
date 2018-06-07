using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.BluetoothService;
using bossdoyKaraoke_NOW.CDG;
using bossdoyKaraoke_NOW.Enums;
using bossdoyKaraoke_NOW.Misc;
using bossdoyKaraoke_NOW.Models;
using bossdoyKaraoke_NOW.Nlog;
using bossdoyKaraoke_NOW.Properties;
using Microsoft.Win32;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.AddOn.Tags;
using Un4seen.Bass.Misc;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using static bossdoyKaraoke_NOW.Enums.DefaultAudio;
using static bossdoyKaraoke_NOW.Enums.PlayerState;
using static bossdoyKaraoke_NOW.Enums.RemoveVocal;
using static bossdoyKaraoke_NOW.Enums.Songs;
using static bossdoyKaraoke_NOW.Enums.TreviewNode;

namespace bossdoyKaraoke_NOW
{
    public class Main_Form
    {

        static string m_filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\karaokeNow\";
        public static INIFileHelper iniFileHelper = new INIFileHelper(m_filePath + @"\appConfig.ini");

        string m_favoritesPath = m_filePath + @"favorites\";
        string m_songsPath = m_filePath + @"songs\";

       
        List<BackgroundWorker> m_bgws = new List<BackgroundWorker>();

        private BlueToothConnect bt;
        private GraphicUtilDX m_gdx;
        private Control m_targetControl;

        private VlcPlayer m_vlc;
        private Bitmap m_bgVideo;
        private bool m_IsPlayingVideo;
        private bool m_IsPlayingCdg;

        /// <summary>
        /// 
        /// </summary>
        private CDGFile m_CDGFile;
        private bool m_Paused;
        private bool m_Play;
        private long m_FrameCount = 0;
        private bool m_Stop;
        private bool m_Mute;
        private float m_Volume = 0f;
        private string m_MP3FileName;
        private string m_TempDir;

        /// <summary>
        /// 
        /// </summary>
        private int m_sortColumn = -1;
        private FolderBrowserDialog m_fbd;
        private List<List<ListViewItem>> m_songsArr = new List<List<ListViewItem>>();
        private List<List<ListViewItem>> m_favoritesArr = new List<List<ListViewItem>>();
        private ListViewItem m_songItems;
        private List<ListViewItem> m_songQueue = new List<ListViewItem>();
        private List<ListViewItem> m_tempFavorites = new List<ListViewItem>();
        private TreeView m_treeview;
        private RootNode m_rootNode;
        private int m_selected_treenode;
        private double m_songTotalDuration = 0D;
        private bool m_IsSearchingListView;
        private bool m_isFromFavorites;
        private bool m_isSongQueueSelected;

        /// <summary>
        /// 
        /// </summary>
        private double m_duration = 0d;
        private string m_MediaFileName;
        private int m_volumeCounter;
        private SYNCPROC m_mixerStallSync;
        private DSP_PeakLevelMeter m_plmOutput;
        private Player m_track = null;
        private Player m_currentTrack = null;
        private Player m_previousTrack = null;
        private BASS_BFX_MIX m_duplicateChannel;
        private int m_fxMix = 0;
        private bool m_isMainFormFullscreen;
        private bool m_isFullScreenPlayer;
        public static FullScreenPlayer fullScreenPlayer = new FullScreenPlayer();
        private Preferences m_prefs = new Preferences();
        private Equalizer m_equalizer;

        private Control m_thisControl;
        private Timer m_timerUpdate;
        private Timer m_timerVolume;
        private Timer m_timerVideo;
        private Timer m_reserveNotification;
        private string targetPath = "";
        private static HashSet<string> m_extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".cdg", ".mp4", ".flv" };
        private string m_extPattern = HashSetExtensionsToString(m_extensions);

        /// <summary>
        /// Initialize Timer, Bass, Vlc and Graphics
        /// </summary>
        /// <param name="form"></param>
        /// <param name="playerWindow"></param>
        public void Initialize(Control playerWindow)
        {
    
            if (!AppSettings.Get<bool>("IsDefaultInit"))
            {
                AppSettings.Set("IsDefaultInit", "true");
                AppSettings.Set("DefaultAudioOutput", PlayerControl.DefaultAudioOutput.ToString());
                AppSettings.Set("IsAsioAutoRestart", "false");
                AppSettings.SetFxDefaultSettings("DEFAudioEQBand");
                AppSettings.SetFxDefaultSettings("DEFCH1EQ1band");
                AppSettings.SetFxDefaultSettings("DEFCH1Comp");
                AppSettings.SetFxDefaultSettings("DEFCH1EQ4band");
                AppSettings.SetFxDefaultSettings("DEFCH1EQ4bandPhone");
                AppSettings.SetFxDefaultSettings("DEFCH1DeEsser");
                //AppSettings.SetFxDefaultSettings("DEFCH1EQ1band", "CH1", "CH2");
               // AppSettings.SetFxDefaultSettings("DEFCH1Comp", "CH1", "CH2");
               // AppSettings.SetFxDefaultSettings("DEFCH1EQ4band", "CH1", "CH2");
              //  AppSettings.SetFxDefaultSettings("DEFCH1DeEsser", "CH1", "CH2"); 
            }

            PlayerControl.DefaultAudioOutput = AppSettings.Get<DefaultAudioOutput>("DefaultAudioOutput");

            bt = new BlueToothConnect();
            bt.StartBlueTooth();

            //UpdateRemoteDeviceSong();

            if (!File.Exists(iniFileHelper.FilePath))
            {
                iniFileHelper.Write("Video", "Video Path", string.Empty);
                /*iniFileHelper.Write("Equalizer", "Band0", "0.0");
                iniFileHelper.Write("Equalizer", "Band1", "0.0");
                iniFileHelper.Write("Equalizer", "Band2", "0.0");
                iniFileHelper.Write("Equalizer", "Band3", "0.0");
                iniFileHelper.Write("Equalizer", "Band4", "0.0");
                iniFileHelper.Write("Equalizer", "Band5", "0.0");
                iniFileHelper.Write("Equalizer", "Band6", "0.0");
                iniFileHelper.Write("Equalizer", "Band7", "0.0");
                iniFileHelper.Write("Equalizer", "Band8", "0.0");
                iniFileHelper.Write("Equalizer", "Band9", "0.0");*/
            }
             
            PlayerControl.ChannelSelected = ChannelSelected.Right;

            m_thisControl = PlayerControl.MainFormControl;
            m_targetControl = playerWindow;

            /* if (Un4seen.Bass.Utils.Is64Bit)
                 targetPath = Path.Combine(Application.StartupPath, "x64");
             else
                 targetPath = Path.Combine(Application.StartupPath, "x86");*/
            InitTimer();

            m_equalizer = new Equalizer();

            if (PlayerControl.DefaultAudioOutput == DefaultAudioOutput.Bass)
                InitBass();
            if (PlayerControl.DefaultAudioOutput == DefaultAudioOutput.Wasapi)
                InitWasapi();
            if (PlayerControl.DefaultAudioOutput == DefaultAudioOutput.Asio)
                InitAsio();

            InitVlc();
            InitGraphics();

        }

        /// <summary>
        /// Close application
        /// </summary>
        public void FormClosed()
        {
            m_timerUpdate.Stop();
            // close bass and bassasio
            if (m_track != null)
                m_track.Dispose();

            if (m_currentTrack != null)
                m_currentTrack.Dispose();

            if (m_previousTrack != null)
                m_previousTrack.Dispose();

            WasapiDevice.Stop();
            // if (!BassAsio.BASS_ASIO_Stop())
            BassAsioDevice.Stop();
            // if (!Bass.BASS_Free())
            Bass.BASS_Free();

            m_gdx.Dispose(true);

        }

        /// <summary>
        /// Resize the main menu video screen
        /// </summary>
        public void ResizeScreen1()
        {
            m_gdx.ResizeScreen1();
        }

        /// <summary>
        /// Resize the full video screen 
        /// </summary>
        public void ResizeScreen2() {
            m_gdx.ResizeScreen2();
        }

        /// <summary>
        /// Initialize Bass audio playback
        /// </summary>
        private void InitBass() {
            try
            {
                Player.BassInitialize();

                if (Player.IsBassInitialized)
                {
                    m_mixerStallSync = new SYNCPROC(OnMixerStall);
                    Bass.BASS_ChannelSetSync(Player.Mixer, BASSSync.BASS_SYNC_STALL, 0L, m_mixerStallSync, IntPtr.Zero);

                    m_timerUpdate.Start();
                    Bass.BASS_ChannelPlay(Player.Mixer, false);
                }
                else return;
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "InitBass", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Initialize Wasapi playback
        /// </summary>
        private void InitWasapi()
        {
            try
            {

                Player.WasapiInitialize();

                if (Player.IsWasapiInitialized)
                {
                    m_mixerStallSync = new SYNCPROC(OnMixerStall);
                    Bass.BASS_ChannelSetSync(Player.Mixer, BASSSync.BASS_SYNC_STALL, 0L, m_mixerStallSync, IntPtr.Zero);

                    m_timerUpdate.Start();
                   // Bass.BASS_ChannelPlay(Player.Mixer, false);
                }
                else return;
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "InitWasapi", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Initialiize BassAsio playback
        /// </summary>
        private void InitAsio() {
            try
            {
                Player.AsioInitialize();

                if (Player.IsAsioInitialized)
                {
                    m_mixerStallSync = new SYNCPROC(OnMixerStall);
                    Bass.BASS_ChannelSetSync(Player.Mixer, BASSSync.BASS_SYNC_STALL, 0L, m_mixerStallSync, IntPtr.Zero);

                    m_timerUpdate.Start();
                }
                else return;
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "InitAsio", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Initialize Vlc for video palyback 
        /// </summary>
        private void InitVlc() {

            m_vlc = new VlcPlayer();
            m_vlc.Volume(PlayerControl.VolumeScroll.Value);
        }

        /// <summary>
        /// Initailize the draw utility and assign main menu handle
        /// </summary>
        /// <param name="target"></param>
        private void InitGraphics() {

            m_gdx = new GraphicUtilDX();

            m_gdx.Initialize(m_targetControl, fullScreenPlayer.panelPlayer);

            m_timerVideo.Start();

        }

        /// <summary>
        /// Initailize the timer for playing audio video and cdg
        /// </summary>
        private void InitTimer()
        {

            m_timerUpdate = new Timer();
            m_timerVolume = new Timer();
            m_timerVideo = new Timer();
            m_reserveNotification = new Timer();

            m_timerUpdate.Tick += TimerUpdate_Tick;
            m_timerVolume.Tick += TimerVolume_Tick;
            m_timerVideo.Tick += TimerVideo_Tick;
            m_reserveNotification.Tick += TimerReserveNotification_Tick;

            m_timerUpdate.Enabled = false;
            m_timerUpdate.Interval = 50;
            m_timerVolume.Enabled = false;
            m_timerVolume.Interval = 100;
            m_timerVideo.Enabled = false;
            m_timerVideo.Interval = 50;
            m_reserveNotification.Enabled = false;
            m_reserveNotification.Interval = 1000;
        }

        /// <summary>
        /// Initailize the default value for main player window
        /// </summary>
        private void InitControls()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.BeginInvoke(new Action(() =>
                {
                    InitControls();
                }));
                return;
            }

            try
            {
                if (m_IsPlayingCdg)
                {
                    PlayerControl.KeyTempoPanel.Enabled = true;
                    PlayerControl.KeyValue.Text = m_currentTrack.Tempo.Key.ToString();
                    PlayerControl.TempoValue.Text = m_currentTrack.Tempo.Tempo.ToString() + "%";
                }
                if (m_IsPlayingVideo)
                {
                    PlayerControl.KeyTempoPanel.Enabled = false;
                    PlayerControl.KeyValue.Text = "0";
                    PlayerControl.TempoValue.Text = "0%";
                }

                if (!m_IsPlayingVideo && !m_IsPlayingCdg)
                {
                    // PlayerControl.FunctionPanel.Enabled = false;
                    PlayerControl.PlayPauseButton.Enabled = false;
                    PlayerControl.NextButton.Enabled = false;
                    PlayerControl.KeyValue.Text = "0";
                    PlayerControl.TempoValue.Text = "0%";
                }
                else
                {
                    // PlayerControl.FunctionPanel.Enabled = true;
                    PlayerControl.PlayPauseButton.Enabled = true;
                    PlayerControl.NextButton.Enabled = true;
                }

            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "InitControls", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Orange square color reserve notification on the side of main menu video screen will be drawn to notify if reserve is successful
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerReserveNotification_Tick(object sender, EventArgs e)
        {
            try
            {
                if (m_reserveNotification.Interval >= 1000)
                {
                    PlayerControl.IsAddToReserve = false;
                    m_reserveNotification.Stop();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "TimerReserveNotification_Tick", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Vlc timer for rendering video to screen  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerVideo_Tick(object sender, EventArgs e)
        {
            try
            {
                if (m_vlc.Video != null)
                {
                    m_bgVideo = new Bitmap(m_vlc.Video);
                    m_gdx.DoRender(m_bgVideo, null, m_songQueue.Count.ToString(), false);
                    m_bgVideo.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "TimerVideo_Tick", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Vlc timer for fading Volume on next track
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerVolume_Tick(object sender, EventArgs e)
        {
            try
            {
                m_volumeCounter--;
                if (m_IsPlayingVideo)
                    m_vlc.Volume(m_volumeCounter);

                if (m_volumeCounter <= 0)
                {
                    m_timerVolume.Stop();
                    m_vlc.Volume(PlayerControl.VolumeScroll.Value);
                    playNextTrack();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "TimerVolume_Tick", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Timer to render and update UI video, elapse time, remaining time and DBlevel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                double dbLevelL = 0.0;
                double dbLevelR = 0.0;

                RMS(out dbLevelL, out dbLevelR);

                // Raise the level with factor 1.5 so that the VUMeter shows more activity
                dbLevelL += Math.Abs(dbLevelL * 0.5);
                dbLevelR += Math.Abs(dbLevelR * 0.5);

                if ((int)dbLevelL < -25)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_off;
                }
                else if ((int)dbLevelL < -20)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_20;
                }
                else if ((int)dbLevelL < -17)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_17;
                }
                else if ((int)dbLevelL < -15)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_15;
                }
                else if ((int)dbLevelL < -12)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_12;
                }
                else if ((int)dbLevelL < -10)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_10;
                }
                else if ((int)dbLevelL < -8)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_8;
                }
                else if ((int)dbLevelL < -6)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_6;
                }
                else if ((int)dbLevelL < -3)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_3;
                }
                else if ((int)dbLevelL < 0)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_0;
                }
                else if ((int)dbLevelL < 1)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_plus1;
                }
                else if ((int)dbLevelL < 2)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_plus2;
                }
                else if ((int)dbLevelL < 3)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_plus3;
                }
                else if ((int)dbLevelL < 4)
                {
                    PlayerControl.LeftChannelLevelDb.Image = Resources.vu_v_plus4;
                }


                PlayerControl.LeftChannelLevelDb.Invalidate();


                if ((int)dbLevelR < -25)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_off;
                }
                else if ((int)dbLevelR < -20)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_20;
                }
                else if ((int)dbLevelR < -17)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_17;
                }
                else if ((int)dbLevelR < -15)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_15;
                }
                else if ((int)dbLevelR < -12)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_12;
                }
                else if ((int)dbLevelR < -10)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_10;
                }
                else if ((int)dbLevelR < -8)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_8;
                }
                else if ((int)dbLevelR < -6)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_6;
                }
                else if ((int)dbLevelR < -3)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_3;
                }
                else if ((int)dbLevelR < 0)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_0;
                }
                else if ((int)dbLevelR < 1)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_plus1;
                }
                else if ((int)dbLevelR < 2)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_plus2;
                }
                else if ((int)dbLevelR < 3)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_plus3;
                }
                else if ((int)dbLevelR < 4)
                {
                    PlayerControl.RightChannelLevelDb.Image = Resources.vu_v_plus4;
                }

                PlayerControl.RightChannelLevelDb.Invalidate();

                if (m_IsPlayingCdg && !m_IsPlayingVideo)
                {
                    m_bgVideo = new Bitmap(m_vlc.Video);
                    m_gdx.DoRender(m_bgVideo, (Bitmap)m_CDGFile.RGBImage, m_songQueue.Count.ToString(), true);
                    m_bgVideo.Dispose();
                    // int level = Bass.BASS_ChannelGetLevel(Player.Mixer);              

                    if (m_currentTrack != null)
                    {
                        long pos = BassMix.BASS_Mixer_ChannelGetPosition(m_currentTrack.Channel);
                        double time = Bass.BASS_ChannelBytes2Seconds(m_currentTrack.Channel, pos);
                        long renderAtPosition = Convert.ToInt64(time * 1000);

                        m_CDGFile.renderAtPosition(renderAtPosition);

                        PlayerControl.Time.Text = Un4seen.Bass.Utils.FixTimespan(time, "HHMMSS");
                        PlayerControl.TimeRemain.Text = Un4seen.Bass.Utils.FixTimespan(Bass.BASS_ChannelBytes2Seconds(m_currentTrack.Channel, m_currentTrack.TrackLength - pos), "HHMMSS");
                        DrawPlayerPosition(pos, 0, m_currentTrack.TrackLength);
                    }
                }
                if (!m_IsPlayingCdg && m_IsPlayingVideo)
                {
                    if (m_vlc.Time == "00:00:00")
                        PlayerControl.Time.Text = "00:00:00";
                    else
                    {
                        double time = Convert.ToDouble(m_vlc.Time);
                        double timeRemain = Convert.ToDouble(m_vlc.TimeDuration);
                        PlayerControl.Time.Text = TimeSpan.FromMilliseconds(time).ToString().Substring(0, 8);
                        PlayerControl.TimeRemain.Text = TimeSpan.FromMilliseconds(timeRemain - time).ToString().Substring(0, 8);

                        m_bgVideo = new Bitmap(m_vlc.Video);
                        m_gdx.DoRender(m_bgVideo, null, m_songQueue.Count.ToString(), true);
                        m_bgVideo.Dispose();
                        DrawPlayerPosition(0, m_vlc.PlayerPosition);
                    }

                    if (m_vlc.MediaEnded)
                    {
                        if (m_songQueue.Count > 0)
                        {

                            SetMediaInfo();

                            if (MediaExtension == ".mp3")
                                addToBassMixer();

                            playNextTrack();
                        }
                        else
                        {
                            SetMediaInfo();

                            m_timerUpdate.Stop();

                            LoadDefaultVideoBG();
                            m_timerVideo.Start();

                            m_IsPlayingCdg = false;
                            m_IsPlayingVideo = false;

                            return;
                        }
                    }

                }
                GetNextTrack();
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "TimerUpdate_Tick", ex.LineNumber(), this.m_thisControl.Name);

            }
        }


        /// <summary>
        /// Get the dblevel per channel
        /// </summary>
        /// <param name="dbLevelL">Left channel</param>
        /// <param name="dbLevelR">Right channel</param>
        private void RMS(out double dbLevelL, out double dbLevelR)
        {

            int peakL = 0;
            int peakR = 0;
            double dbLeft = 0.0;
            double dbRight = 0.0;

            int level = 0;

            if (Player.IsAsioInitialized)
            {
                float fpeakL = BassAsio.BASS_ASIO_ChannelGetLevel(false, BassAsioDevice.asioOuputChannel);
                float fpeakR = BassAsio.BASS_ASIO_ChannelGetLevel(false, BassAsioDevice.asioOuputChannel + 1);
                dbLeft = 20.0 * Math.Log10(fpeakL);
                dbRight = 20.0 * Math.Log10(fpeakR);
            }

            else if (Player.IsWasapiInitialized)
            {
                level = BassWasapi.BASS_WASAPI_GetLevel();
            }
            else
                level = Bass.BASS_ChannelGetLevel(Player.Mixer);


            if (Player.IsBassInitialized || Player.IsWasapiInitialized)
            {
                peakL = Un4seen.Bass.Utils.LowWord32(level); // the left level
                peakR = Un4seen.Bass.Utils.HighWord32(level); // the right level

                dbLeft = Un4seen.Bass.Utils.LevelToDB(peakL, 65535);
                dbRight = Un4seen.Bass.Utils.LevelToDB(peakR, 65535);
            }

            dbLevelL = dbLeft;
            dbLevelR = dbRight;
        }

        /// <summary>
        /// Set audio output device
        /// </summary>
        public void SetAudioOutputDevice()
        {
            m_vlc.SetAudioOutputDevice();
        }

        /// <summary>
        /// Increase Key on audio truck 
        /// </summary>
        public void keyPlusbtn()
        {
            try
            {
                if (PlayerControl.KeyTempoPanel.Enabled)
                {
                    if (m_currentTrack.Tempo.Key != 6)
                    {
                        m_currentTrack.Tempo.Key = m_currentTrack.Tempo.Key + 1f;
                        InitControls();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "keyPlusbtn", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Decrease Key on audio truck
        /// </summary>
        public void keyMinusBtn()
        {
            try
            {
                if (PlayerControl.KeyTempoPanel.Enabled)
                {
                    if (m_currentTrack.Tempo.Key != -6)
                    {
                        m_currentTrack.Tempo.Key = m_currentTrack.Tempo.Key - 1f;
                        InitControls();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "keyMinusBtn", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Increase Tempo on audio truck
        /// </summary>
        public void tempoPlusBtn()
        {
            try
            {
                if (PlayerControl.KeyTempoPanel.Enabled)
                {
                    if (m_currentTrack.Tempo.Tempo != 50)
                    {
                        m_currentTrack.Tempo.Tempo = m_currentTrack.Tempo.Tempo + 5f;
                        InitControls();

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "tempoPlusBtn", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Decrease Tempo on audio truck
        /// </summary>
        public void tempoMinusBtn()
        {
            try
            {
                if (PlayerControl.KeyTempoPanel.Enabled)
                {
                    if (m_currentTrack.Tempo.Tempo != -50)
                    {
                        m_currentTrack.Tempo.Tempo = m_currentTrack.Tempo.Tempo - 5f;
                        InitControls();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "tempoMinusBtn", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Update Equlizer Gain
        /// </summary>
        /// <param name="band">The band number tp update gain</param>
        /// <param name="gain">The gain value</param>
        public void UpdateEQ(int band, float gain)
        {
            
            if (m_IsPlayingCdg)
                m_equalizer.UpdateEQBass(band, gain);
            //  m_currentTrack.UpdateEQ(band, gain);

            if (m_IsPlayingVideo)
                m_vlc.UpdateEQ(band, gain);
        }

        /// <summary>
        /// Mute and Unmute button
        /// </summary>
        public void volumeBtn()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    volumeBtn();
                }));
                return;
            }

            try
            {

                if (!m_Mute)
                {
                    m_Volume = PlayerControl.VolumeScroll.Value * 0.01f;

                    if (m_currentTrack != null)
                        m_currentTrack.Mute();

                    if (m_IsPlayingVideo)
                    {
                        m_vlc.Mute();
                        m_vlc.Volume(PlayerControl.VolumeScroll.Value);
                    }
                    PlayerControl.VolumeButton.BackgroundImage = Resources.ic_volume_off_black_24dp_1x;
                    m_Mute = true;

                }
                else
                {
                    if (m_currentTrack != null)
                        m_currentTrack.Mute();

                    if (m_IsPlayingVideo)
                        m_vlc.Mute();

                    PlayerControl.VolumeButton.BackgroundImage = Resources.ic_volume_up_black_24dp_1x;
                    m_Mute = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "volumeBtn", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Remove left/right vocals on audio track with seperate vocal track  
        /// </summary>
        public void RemoveVocalLeftOrRight()
        {
            switch (PlayerControl.ChannelSelected)
            {
                case ChannelSelected.None: // Center no vocal removed
                    Bass.BASS_ChannelRemoveFX(Player.Mixer, m_fxMix);
                    PlayerControl.RemoveVocal.BackgroundImage = Resources.ic_record_voice_over_L_R_black_24dp_1x;
                    PlayerControl.ChannelSelected = ChannelSelected.Right;

                    bt.RemoveVocalLeftOrRight(ChannelSelected.None);
                    break;
                case ChannelSelected.Right: // Remove Right Vocal
                    Bass.BASS_ChannelRemoveFX(Player.Mixer, m_fxMix);
                    m_duplicateChannel = new BASS_BFX_MIX(BASSFXChan.BASS_BFX_CHAN1, BASSFXChan.BASS_BFX_CHAN1);
                    m_fxMix = Bass.BASS_ChannelSetFX(Player.Mixer, BASSFXType.BASS_FX_BFX_MIX, 0);
                    Bass.BASS_FXSetParameters(m_fxMix, m_duplicateChannel);
                    PlayerControl.RemoveVocal.BackgroundImage = Resources.ic_record_voice_over_R_black_24dp_1x;
                    PlayerControl.ChannelSelected = ChannelSelected.Left;

                    bt.RemoveVocalLeftOrRight(ChannelSelected.Right);
                    break;
                case ChannelSelected.Left: // Remove Left Vocal 
                    Bass.BASS_ChannelRemoveFX(Player.Mixer, m_fxMix);
                    m_duplicateChannel = new BASS_BFX_MIX(BASSFXChan.BASS_BFX_CHAN2, BASSFXChan.BASS_BFX_CHAN2);
                    m_fxMix = Bass.BASS_ChannelSetFX(Player.Mixer, BASSFXType.BASS_FX_BFX_MIX, 0);
                    Bass.BASS_FXSetParameters(m_fxMix, m_duplicateChannel);
                    PlayerControl.RemoveVocal.BackgroundImage = Resources.ic_record_voice_over_L_black_24dp_1x;
                    PlayerControl.ChannelSelected = ChannelSelected.None;

                    bt.RemoveVocalLeftOrRight(ChannelSelected.Left);
                    break;
            }

            Console.WriteLine(PlayerControl.ChannelSelected);

        }

        /// <summary>
        /// Increase/ decrease volume  
        /// </summary>
        public void volumeTrack()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    volumeTrack();
                }));
                return;
            }

            try
            {
                m_Volume = PlayerControl.VolumeScroll.Value * 0.01f;
                PlayerControl.VolumeLevel = PlayerControl.VolumeScroll.Value.ToString();

                if (m_currentTrack != null)
                    m_currentTrack.Volume = m_Volume;

                if (m_IsPlayingVideo)
                    m_vlc.Volume(PlayerControl.VolumeScroll.Value);

                if (m_Volume > 0f)
                {
                    PlayerControl.VolumeButton.BackgroundImage = Resources.ic_volume_up_black_24dp_1x;
                    m_Mute = false;
                }
                else
                {
                    PlayerControl.VolumeButton.BackgroundImage = Resources.ic_volume_off_black_24dp_1x;
                    m_Mute = true;
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "volumeTrack", ex.LineNumber(), this.m_thisControl.Name);

            }

        }

        /// <summary>
        /// Increase volume using android application
        /// </summary>
        public void VolumePlus()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    VolumePlus();
                }));
                return;
            }
            try
            {
                if (PlayerControl.VolumeScroll.Value != PlayerControl.VolumeScroll.Maximum)
                    PlayerControl.VolumeScroll.Value += 1;
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "VolumePlus", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Decrease volume using android application
        /// </summary>
        public void VolumeMinus()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    VolumeMinus();
                }));
                return;
            }

            try
            {
                if (PlayerControl.VolumeScroll.Value != PlayerControl.VolumeScroll.Minimum)
                    PlayerControl.VolumeScroll.Value -= 1;
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "VolumeMinus", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Load available files from ProgramData\karaokeNow to IU 
        /// </summary>
        /// <param name="treeview"></param>
        public void LoadTreviewMenu(TreeView treeview)
        {

            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    LoadTreviewMenu(treeview);
                }));
                return;
            }

            DirectoryInfo directoryInfo;
            DirectorySecurity directorySecurity;
            AccessRule accessRule;
            SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null);

            try
            {
                directoryInfo = Directory.CreateDirectory(m_filePath); //Karaoke now main diretory in "ProgramData"

                if (Directory.Exists(m_filePath))
                {
                    directoryInfo = Directory.CreateDirectory(m_filePath);
                    bool modified;
                    directorySecurity = directoryInfo.GetAccessControl();
                    accessRule = new FileSystemAccessRule(
                            securityIdentifier,
                            FileSystemRights.FullControl,
                            AccessControlType.Allow);
                    directorySecurity.ModifyAccessRule(AccessControlModification.Add, accessRule, out modified);
                    directoryInfo.SetAccessControl(directorySecurity);
                }

                Directory.CreateDirectory(m_favoritesPath);
                Directory.CreateDirectory(m_songsPath);

                this.m_treeview = new TreeView();
                this.m_treeview = treeview;
                PlayerControl.SongListView.FullRowSelect = false;
                List<string> songs;
                string fileName;

                foreach (var node in Collect(this.m_treeview.Nodes))
                {
                    //RootNode rootNode;
                    setSEARCHDIRorTEXTState(SearchAndLoad.SEARCH_TEXTFILE);
                    if (Enum.TryParse(node.Name.ToUpper(), out m_rootNode))
                    {
                        switch (m_rootNode)
                        {
                            case RootNode.SONG_QUEUE:
                                songs = new List<string>();
                                songs = Directory.EnumerateFiles(m_filePath, "*.que", SearchOption.AllDirectories).ToList();
                                if (songs.Count() > 0)
                                {
                                    m_isFromFavorites = false;
                                    startWorker(songs[0]);

                                    setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_FROM_FILE_TO_QUEUE);
                                    startWorker();
                                }
                                break;
                            case RootNode.MY_FAVORITES:
                                songs = new List<string>();
                                songs = Directory.EnumerateFiles(m_favoritesPath, "*.fav", SearchOption.AllDirectories).OrderByDescending(file => new FileInfo(file).CreationTime).ToList();
                                if (songs.Count > 0)
                                {
                                    for (int i = 0; i < songs.Count; i++)
                                    {
                                        fileName = Path.GetFileName(songs[i]).Replace(".fav", "");
                                        node.Nodes.Insert(i, fileName, fileName);
                                        startWorker(songs[i]);
                                        m_favoritesArr.Add(PlayerControl.AllSongs);
                                    }
                                }
                                break;
                            case RootNode.MY_COMPUTER:
                                songs = new List<string>();
                                songs = Directory.EnumerateFiles(m_songsPath, "*.bkN", SearchOption.AllDirectories).OrderByDescending(file => new FileInfo(file).CreationTime).ToList();
                                if (songs.Count > 0)
                                {
                                    for (int i = 0; i < songs.Count; i++)
                                    {
                                        fileName = Path.GetFileName(songs[i]).Replace(".bkN", "");
                                        node.Nodes.Insert(i, fileName, fileName);
                                        startWorker(songs[i]);
                                        m_songsArr.Add(PlayerControl.AllSongs);
                                    }

                                    this.m_treeview.SelectedNode = node.FirstNode;
                                    PlayerControl.SongListView.VirtualListSize = m_songsArr[node.FirstNode.Index].Count;
                                }
                                else
                                    PlayerControl.SongListView.VirtualListSize = 0;

                                break;
                        }
                    }
                }

                PlayerControl.SongListView.FullRowSelect = true;
                PlayerControl.SongListView.Refresh();
                m_IsSearchingListView = true;
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "LoadTreviewMenu", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Set or select a song to play next
        /// </summary>
        public void SetAsNext()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    SetAsNext();
                }));
                return;
            }

            try
            {
                if (m_songQueue.Count > 1)
                {
                    this.m_thisControl.Execute(delegate
                    {
                        ListView.SelectedIndexCollection col = PlayerControl.SongListView.SelectedIndices;
                        foreach (int i in col)
                        {
                            m_songItems = new ListViewItem();
                            m_songItems = m_songQueue[i];
                            m_songQueue.RemoveAt(i);
                            m_songQueue.Insert(0, m_songItems);
                        }
                        setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_QUEUE_SONGS);
                        startWorker();
                    });

                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "SetAsNext", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        // =========================================================================================
        /// <summary>
        /// Load songs from Favorites treeview to Song Queue
        /// </summary>
        public void AddFavoritesSongsToQueue()
        {

            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    AddFavoritesSongsToQueue();
                }));
                return;
            }

            try
            {
                setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_FROM_FILE_TO_QUEUE);
                startWorker();

                setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_QUEUE_SONGS);
                startWorker();

                this.m_treeview.SelectedNode = this.m_treeview.Nodes[0];
                this.m_treeview.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "AddFavoritesSongsToQueue", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Save all previously played songs to Favorites
        /// </summary>
        public void AddAllPlayedSongsToFavorite()
        {
            CreateFavoriteFile(this.m_treeview.Nodes[1].Nodes[m_selected_treenode].Name, true);
        }

        public void RenameFavoriteFile(NodeLabelEditEventArgs e) {
            try
            {
                if (e.Label != null)
                {
                    if (e.Label.Length > 0)
                    {
                        if (e.Label.IndexOfAny(new char[] { '@', '.', ',', '!' }) == -1)
                        {
                            // Stop editing without canceling the label change.
                            e.Node.EndEdit(false);
                            RenameFavoriteFile(e.Label, e.Node.Name);
                        }
                        else
                        {
                            /* Cancel the label edit action, inform the user, and 
                               place the node in edit mode again. */
                            e.CancelEdit = true;
                            MessageBox.Show("Invalid filename.\n" +
                               "The invalid characters are: '@', '.', ', ', '!'",
                               "Favorites");
                            e.Node.BeginEdit();
                        }
                    }
                    else
                    {
                        /* Cancel the label edit action, inform the user, and 
                           place the node in edit mode again. */
                        e.CancelEdit = true;
                        MessageBox.Show("Invalid filename.\nThe filename cannot be blank",
                           "Favorites");
                        e.Node.BeginEdit();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "RenameFavoriteFile", ex.LineNumber(), this.m_thisControl.Name);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void RemoveFileFromTreeView()
        {
            try
            {
                string sDir = "";
                bool isFileExist = false;
                if (Enum.TryParse(this.m_treeview.SelectedNode.Parent.Name.ToUpper(), out m_rootNode))
                {
                    switch (m_rootNode)
                    {
                        case RootNode.MY_FAVORITES:
                            sDir = m_favoritesPath + this.m_treeview.SelectedNode.Text + ".fav";
                            isFileExist = File.Exists(sDir);
                            if (isFileExist) m_favoritesArr.Remove(m_favoritesArr[m_selected_treenode]);
                            break;
                        case RootNode.MY_COMPUTER:
                            sDir = m_songsPath + this.m_treeview.SelectedNode.Text + ".bkN";
                            isFileExist = File.Exists(sDir);
                            if (isFileExist) m_songsArr.Remove(m_songsArr[m_selected_treenode]);
                            break;
                    }

                    if (isFileExist)
                    {
                        File.Delete(sDir);
                        this.m_treeview.SelectedNode.Remove();
                        this.m_treeview.SelectedNode = this.m_treeview.SelectedNode.Parent;
                        PlayerControl.SongListView.VirtualListSize = 0;
                        PlayerControl.SongListView.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "RemoveFileFromTreeview", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Remove selected song from Song Queue
        /// </summary>
        public void RemoveSongFromQueue()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    RemoveSongFromQueue();
                }));
                return;
            }

            try
            {
                if (m_songQueue.Count > 0)
                {

                    ListView.SelectedIndexCollection col = PlayerControl.SongListView.SelectedIndices;
                    foreach (int i in col)
                    {
                        m_songItems = new ListViewItem();
                        m_songItems = m_songQueue[i];
                        m_duration = Convert.ToDouble(m_songItems.SubItems[3].Text);
                        m_songQueue.RemoveAt(i);
                        updateSongQueueDuration("minus");
                    }
                    setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_QUEUE_SONGS);
                    startWorker();

                   // setSEARCHDIRorTEXTState(SearchAndLoad.WRITE_TO_QUEUE_LIST);
                   // startWorker();
                    WriteToQueueList();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "RemoveSongFromQueue", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Clear Song Queue
        /// </summary>
        public void EmptyQueue()
        {
            try
            {
                if (m_songQueue.Count > 0)
                {
                    m_songQueue.RemoveRange(0, m_songQueue.Count);
                    PlayerControl.SongListView.VirtualListSize = 0;
                    PlayerControl.SongListView.Refresh();
                    m_songTotalDuration = 0;
                    m_duration = 0;
                    updateSongQueueDuration("minus");

                    //setSEARCHDIRorTEXTState(SearchAndLoad.WRITE_TO_QUEUE_LIST);
                    //startWorker();
                    WriteToQueueList();
                }

                if (m_tempFavorites.Count > 0)
                {
                    m_tempFavorites.RemoveRange(0, m_tempFavorites.Count);
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "EmptyQueue", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Shuffle songs
        /// </summary>
        public void ShuffleSongs() {
            setSEARCHDIRorTEXTState(SearchAndLoad.SHUFFLE_SONGS);
            startWorker();
        }

        /// <summary>
        /// Sort songs
        /// </summary>
        /// <param name="column"></param>
        public void SortSongs(int column)
        {
            setSEARCHDIRorTEXTState(SearchAndLoad.SORT_SONGS);
            startWorker(column.ToString());
        }

        /// <summary>
        /// Add songs to Song Queue using android application
        /// </summary>
        public bool AddRemoteSongToQueue(string song)
        {    
             return AddToQueueList(song);
        }

        /// <summary>
        ///  Play/Add selected songs to Song Queue
        /// </summary>
        public void on_SongListView_Item_Selected()
        {

            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    on_SongListView_Item_Selected();
                }));
                return;
            }

            try
            {
                ListView.SelectedIndexCollection col = PlayerControl.SongListView.SelectedIndices;

                foreach (int i in col)
                {
                   // TrackInfo trackInfo = null;
                    string filePath = PlayerControl.SongListView.Items[i].SubItems[4].Text.ToString();
                    if(!AddToQueueList(filePath, i))
                        MessageBox.Show("Cannot find " + Path.GetFileName(filePath) + " file to play.");
                    /*   string MediaFileName = GetExtPatern(filePath);

                       if (File.Exists(MediaFileName))
                       {
                           if (MediaFileName.EndsWith(".mp3"))
                           {
                               trackInfo = new TrackInfo(MediaFileName);
                           }
                           else
                           {
                               trackInfo = new TrackInfo();
                               m_vlc.GetDuration(MediaFileName);
                               trackInfo.Duration = GetVlcTimeOrDuration(Convert.ToDouble(m_vlc.GetTimeDuration)).ToString();
                           }


                           MediaExtension = Path.GetExtension(MediaFileName).ToLower();
                           trackInfo.ID = PlayerControl.SongListView.Items[i].SubItems[0].Text.ToString();
                           trackInfo.Name = PlayerControl.SongListView.Items[i].SubItems[1].Text.ToString();
                           trackInfo.Artist = PlayerControl.SongListView.Items[i].SubItems[2].Text.ToString();
                           trackInfo.FilePath = filePath;

                           m_songItems = new ListViewItem();
                           m_songItems = trackInfo.toListViewItem();
                           addToQueue();
                           WriteToQueueList(); //This sholud be running on background

                           AddToReserveNotification();
                       }
                       else
                       {
                           MessageBox.Show("Cannot find " + Path.GetFileName(MediaFileName) + " file to play.");
                           return;

                       }*/
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "on_SongListView_Item_Selected", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Show the content of the treeview selected node 
        /// </summary>
        /// <param name="e"></param>
        public void treeView1_AfterSelect(TreeViewEventArgs e)
        {
            try
            {
                string sDir = "";
                bool isDataExist = false;
                // RootNode rootNode;
                if (Enum.TryParse(e.Node.Name.ToUpper(), out m_rootNode))
                {
                    switch (m_rootNode)
                    {
                        case RootNode.SONG_QUEUE:
                            m_isSongQueueSelected = true;
                            setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_QUEUE_SONGS);
                            startWorker();
                            break;
                        case RootNode.MY_FAVORITES:
                            m_isSongQueueSelected = false;
                            break;
                        case RootNode.ADD_FAVORITES:
                            m_isSongQueueSelected = false;
                            this.m_treeview.LabelEdit = true;
                            int childcount = this.m_treeview.SelectedNode.Parent.Nodes.Count;
                            this.m_treeview.SelectedNode.Parent.Nodes.Insert(0, "Favorites " + childcount, "Favorites " + childcount);
                            this.m_treeview.SelectedNode.Parent.Nodes[0].BeginEdit();

                            CreateFavoriteFile(this.m_treeview.SelectedNode.Parent.Nodes[0].Name);

                            if (this.m_treeview.SelectedNode.Name.ToUpper() == RootNode.ADD_FAVORITES.ToString())
                            {
                                this.m_treeview.SelectedNode = this.m_treeview.SelectedNode.Parent;
                            }
                            break;
                        case RootNode.MY_COMPUTER:
                            m_isSongQueueSelected = false;
                            break;
                        case RootNode.ADD_FOLDER:
                            m_isSongQueueSelected = false;
                            m_fbd = new FolderBrowserDialog();
                            setSEARCHDIRorTEXTState(SearchAndLoad.SEARCH_DIRECTORY);
                            if (m_fbd.ShowDialog() == DialogResult.OK)
                            {
                                startWorker(m_fbd.SelectedPath);
                                string[] filePath = new string[] { m_fbd.SelectedPath };
                                string folderName = Path.GetFileName(m_fbd.SelectedPath);
                                this.m_treeview.SelectedNode.Parent.Nodes.Insert(0, folderName, folderName);

                            }

                            if (this.m_treeview.SelectedNode.Name.ToUpper() == RootNode.ADD_FOLDER.ToString())
                            {
                                this.m_treeview.SelectedNode = this.m_treeview.SelectedNode.Parent;
                            }
                            break;
                    }
                }

                if (e.Node.Parent != null && e.Node.Parent.GetType() == typeof(TreeNode))
                {
                    if (this.m_treeview.SelectedNode.Level != 0 && this.m_treeview.SelectedNode.Name.ToUpper() != RootNode.ADD_FOLDER.ToString() && this.m_treeview.SelectedNode.Name.ToUpper() != RootNode.ADD_FAVORITES.ToString())
                    {
                        m_isSongQueueSelected = false;
                        m_selected_treenode = this.m_treeview.SelectedNode.Index;

                        if (Enum.TryParse(e.Node.Parent.Name.ToUpper(), out m_rootNode))
                        {
                            switch (m_rootNode)
                            {
                                case RootNode.MY_FAVORITES:
                                    setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_FAVORITES);
                                    sDir = m_favoritesPath + this.m_treeview.SelectedNode.Text + ".fav";
                                    isDataExist = File.Exists(sDir);
                                    m_isFromFavorites = true;
                                    break;
                                case RootNode.MY_COMPUTER:
                                    setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_ADDED_SONGS);
                                    sDir = m_songsPath + this.m_treeview.SelectedNode.Text + ".bkN";
                                    string songsPath = File.ReadLines(sDir).First();
                                    isDataExist = Directory.Exists(songsPath);
                                    break;
                            }
                        }

                        this.m_thisControl.ExecuteAsync(delegate
                        {
                            if (!isDataExist)
                            {
                                PlayerControl.SongListView.FullRowSelect = false;
                                PlayerControl.SongListView.BackColor = SystemColors.Control;
                                PlayerControl.SongListView.ForeColor = SystemColors.ControlDark;
                            }
                            else
                            {
                                PlayerControl.SongListView.FullRowSelect = true;
                                PlayerControl.SongListView.BackColor = SystemColors.Window;
                                PlayerControl.SongListView.ForeColor = SystemColors.WindowText;
                            }
                        });

                        startWorker(sDir);
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "treeView1_AfterSelect", ex.LineNumber(), this.m_thisControl.Name);

            }

        }

        /// <summary>
        /// Show/display the full screen window
        /// </summary>
        public void ShowCDGWindow()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    ShowCDGWindow();
                }));
                return;
            }

            try
            {
                Screen[] zScreen = Screen.AllScreens;
                int secondary = 1;

                if (!zScreen[0].Primary)
                    secondary = 0;

                if (fullScreenPlayer == null || fullScreenPlayer.IsDisposed)
                {
                    fullScreenPlayer = new FullScreenPlayer();
                    m_gdx.InitScreen2(fullScreenPlayer.panelPlayer);
                }

                int c = zScreen.Count();

                if (zScreen.Count() > 1)
                {
                    if (!m_isFullScreenPlayer)
                    {
                        fullScreenPlayer.StartPosition = FormStartPosition.Manual;
                        fullScreenPlayer.Bounds = zScreen[secondary].Bounds;
                        fullScreenPlayer.Show();
                        fullScreenPlayer.Activate();
                        m_isFullScreenPlayer = true;
                    }
                    else {
                        HideCDGWindow();
                        m_isFullScreenPlayer = false;
                    }
                }
                else
                {
                    if (!m_isMainFormFullscreen)
                    {
                        PlayerControl.SongListView.Columns[0].Width = 1;
                        PlayerControl.SongListView.Columns[1].Width = 200;
                        PlayerControl.splitContainer.Panel2.Controls["panel6"].Controls["panelPlayer"].Visible = false;
                        PlayerControl.splitContainer.Panel2.Controls["panel6"].Controls.Add(PlayerControl.splitContainer.Panel1.Controls["panel2"]);
                        PlayerControl.splitContainer.Panel2.Controls["panel6"].Controls["panel2"].Height = 300;
                        PlayerControl.splitContainer.Panel2.Controls["panel6"].Controls["panel2"].Dock = DockStyle.Top;
                        PlayerControl.splitContainer.Panel1.Controls.Remove(PlayerControl.splitContainer.Panel1.Controls["panel2"]);
                        PlayerControl.MainFormControl.WindowState = FormWindowState.Maximized;
                        PlayerControl.MainFormControl.Refresh();
                       
                        PlayerControl.splitContainer.SplitterDistance = 1100;
                        PlayerControl.splitContainer.IsSplitterFixed = true;
                        fullScreenPlayer.TopLevel = false;
                        PlayerControl.splitContainer.Panel1.Controls.Add(fullScreenPlayer);
                        fullScreenPlayer.Show();
                        fullScreenPlayer.Activate();

                        m_isMainFormFullscreen = true;
                    }
                    else
                    {
                        fullScreenPlayer.Close();
                        PlayerControl.SongListView.Columns[0].Width = 74;
                        PlayerControl.SongListView.Columns[1].Width = 234;
                        PlayerControl.splitContainer.SplitterDistance = 250;
                        PlayerControl.splitContainer.IsSplitterFixed = false;                       
                        PlayerControl.splitContainer.Panel1.Controls.Add(PlayerControl.splitContainer.Panel2.Controls["panel6"].Controls["panel2"]);
                        PlayerControl.splitContainer.Panel1.Controls["panel2"].Dock = DockStyle.Fill;
                        PlayerControl.splitContainer.Panel2.Controls["panel6"].Controls["panelPlayer"].Visible = true;
                        PlayerControl.splitContainer.Panel2.Controls["panel6"].Controls.Remove(PlayerControl.splitContainer.Panel2.Controls["panel6"].Controls["panel2"]);
                        PlayerControl.MainFormControl.WindowState = FormWindowState.Normal;
                        PlayerControl.MainFormControl.Refresh();

                        m_isMainFormFullscreen = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "ShowCDGWindow", ex.LineNumber(), this.m_thisControl.Name);

            }

        }
        /// <summary>
        /// Hide/close the full screen window 
        /// </summary>
        public void HideCDGWindow()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    HideCDGWindow();
                }));
                return;
            }
            else
                fullScreenPlayer.Close();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sDir"></param>
        public bool GetVideoBG(string sDir)
        {
            return m_vlc.GetVideoBG(sDir);
        }

        /// <summary>
        /// Set the preview video on Preferences form 
        /// </summary>
        public void SetDefaultVideoBG(IntPtr handle)
        {
            m_vlc.SetDefaultVideoBG(handle);
        }

        /// <summary>
        /// 
        /// </summary>
        public void ViewNextPreviewVideoBG()
        {
            m_vlc.ViewNextPreviewVideoBG();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ViewPreviousPreviewVideoBG()
        {
            m_vlc.ViewPreviousPreviewVideoBG();

        }

        /// <summary>
        /// Stop playing the preview video on Preferences form 
        /// </summary>
        public void StopPreviewVideoBG()
        {
            m_vlc.StopPreviewVideoBG();
        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadDefaultVideoBG()
        {
            m_vlc.LoadDefaultVideoBG();
        }

        /// <summary>
        ///  Show the Preference window
        /// </summary>
        public void LoadPreferecesForm()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    LoadPreferecesForm();
                }));
                return;
            }

            try
            {
                if (m_prefs == null || m_prefs.IsDisposed)
                {
                    m_prefs = new Preferences();
                }

                PlayerControl.PrefsForm = m_prefs;

                m_prefs.Show();
                m_prefs.Activate();
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "LoadPreferecesForm", ex.LineNumber(), this.m_thisControl.Name);

            }

        }

        /// <summary>
        /// Play/puase playback 
        /// </summary>
        public void PlayPause()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    PlayPause();
                }));
                return;
            }

            try
            {
                if (m_currentTrack != null)
                {
                    if (m_currentTrack.PlayState == PlayState.Playing)
                    {
                        m_currentTrack.Pause();
                        m_Paused = true;
                    }
                    else
                    {
                        m_currentTrack.Play();
                        m_Paused = false;
                    }
                }

                if (m_IsPlayingVideo)
                {
                    m_Paused = m_vlc.PlayPause();
                }

                if (m_Paused)
                    PlayerControl.PlayPauseButton.BackgroundImage = Resources.ic_play_arrow_black_36dp_1x;
                else
                    PlayerControl.PlayPauseButton.BackgroundImage = Resources.ic_pause_black_36dp_1x;
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "PlayPause", ex.LineNumber(), this.m_thisControl.Name);

            }
        }


        public void CheckForSongUpdate()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public void UpdateRemoteDeviceSong()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.FileName = "./adb.exe ";
            //startInfo.Arguments = "forward tcp:38300 tcp:38300";
            startInfo.Arguments = "status-window";
            process.StartInfo = startInfo;
            process.Start();
            string output1 = process.StandardOutput.ReadToEnd();
            Console.WriteLine("output1 " + output1);
            process.WaitForExit();


            /*startInfo.FileName = "adb.exe";
startInfo.Arguments = "-s " + textBox1.Text + " shell dumpsys battery";
process.StartInfo = startInfo;
process.Start();
string output = process.StandardOutput.ReadToEnd();
richTextBox1.Text = output;
process.WaitForExit();*/
        }


        // Private functions ======================================================================================================    


        /// <summary>
        /// Add songs to Song Queue list 
        /// </summary>
        /// <param name="song"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool AddToQueueList(string song, int item = -1) {
            try
            {
                TrackInfo trackInfo = null;
                string MediaFileName = GetExtPatern(song);

                if (File.Exists(MediaFileName))
                {
                    if (MediaFileName.EndsWith(".mp3"))
                    {
                        trackInfo = new TrackInfo(MediaFileName);
                    }
                    else
                    {
                        trackInfo = new TrackInfo();
                        m_vlc.GetDuration(MediaFileName);
                        trackInfo.Duration = GetVlcTimeOrDuration(Convert.ToDouble(m_vlc.GetTimeDuration)).ToString();
                    }


                    MediaExtension = Path.GetExtension(MediaFileName).ToLower();
                    if (item != -1)
                    {
                        trackInfo.ID = PlayerControl.SongListView.Items[item].SubItems[0].Text.ToString();
                        trackInfo.Name = PlayerControl.SongListView.Items[item].SubItems[1].Text.ToString();
                        trackInfo.Artist = PlayerControl.SongListView.Items[item].SubItems[2].Text.ToString();
                        trackInfo.FilePath = song;
                    }
                    else
                    {
                        string songInfo = song.Substring(song.LastIndexOf("\\") + 1).Trim();
                        trackInfo.ID = "0";
                        trackInfo.Name = songInfo.Substring(songInfo.LastIndexOf(" - ") + 1).Trim();
                        trackInfo.Artist = songInfo;
                        trackInfo.FilePath = song;
                    }

                    m_songItems = new ListViewItem();
                    m_songItems = trackInfo.toListViewItem();
                    addToQueue();

                    //setSEARCHDIRorTEXTState(SearchAndLoad.WRITE_TO_QUEUE_LIST);
                    //startWorker();
                    WriteToQueueList(); //This sholud be running on background

                    AddToReserveNotification();

                    return true;
                }
                else
                {
                    //AddToReserveNotification();
                   // MessageBox.Show("Cannot find " + Path.GetFileName(MediaFileName) + " file to play.");
                    return false;

                }

            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "AddToQueueList", ex.LineNumber(), this.m_thisControl.Name);

            }

            return false;
        }

        /// <summary>
        /// Load Favorites to Song Queue
        /// </summary>
        /// 
        private void LoadFromFileToQueue()
        {
            //int count = m_tempFavorites == null? 0 : m_tempFavorites.Count;
            try
            {
                if (m_isFromFavorites)
                {
                    if (m_favoritesArr[m_selected_treenode].Count > 0)
                    {
                        int count = 0;
                        m_favoritesArr[m_selected_treenode].Select(s =>
                        {
                            return SelectFromListViewItems(s, count++);

                        }).ToList();
                    }
                }
                else
                {
                    if (PlayerControl.AllSongs.Count() > 0)
                    {
                        int count = 0;
                        PlayerControl.AllSongs.Select(s =>
                        {
                            return SelectFromListViewItems(s, count++);

                        }).ToList();
                    }
                }

                WriteToQueueList();

                m_isFromFavorites = false;
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "LoadFromFileToQueue", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Add selected file to Song Queue list
        /// </summary>
        /// <param name="item"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private ListViewItem SelectFromListViewItems(ListViewItem item, int count)
        {
            string MediaFileName = GetExtPatern(item.SubItems[4].Text);
            MediaExtension = Path.GetExtension(MediaFileName).ToLower();
            ListViewItem items = trackInfo(item.SubItems[4].Text, count).toListViewItem();

            try
            {
                TrackInfo info = null;
                if (MediaFileName.EndsWith(".mp3"))
                {
                    info = new TrackInfo(MediaFileName);
                    items.SubItems[3].Text = info.Duration;
                    items.Tag = info.Tags;
                }
                else
                {
                    info = new TrackInfo();
                    m_vlc.GetDuration(MediaFileName);
                    items.SubItems[3].Text = GetVlcTimeOrDuration(Convert.ToDouble(m_vlc.GetTimeDuration)).ToString();
                }

                m_songItems = new ListViewItem();
                m_songItems = items;
                addToQueue();
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "SelectFromListViewItems", ex.LineNumber(), this.m_thisControl.Name);

            }
            return items;
        }


        /// <summary>
        /// Wright selected files to SonQueueList.que text file on ProgramData\karaokeNow 
        /// </summary>
        private void WriteToQueueList()
        {
            try
            {
                string file = m_filePath + "SonQueueList.que";

                var items = m_songQueue.OfType<ListViewItem>().Select(i => i.SubItems[4].Text).ToList();
                string[] filaPathArray = items.ToArray<string>();
                Directory.CreateDirectory(m_filePath);

                File.WriteAllLines(file, filaPathArray);
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "WriteToQueueList", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Wright selected files to Favorites.fav text file on ProgramData\karaokeNow
        /// </summary>
        /// <param name="filename">name of the text file</param>
        /// <param name="addFromPlayedSongs">true if adding previously played songs</param>
        private void CreateFavoriteFile(string filename, bool addFromPlayedSongs = false)
        {
            try
            {
                string fname = filename + ".fav";
                string[] filaPathArray = new string[0] { };
                if (addFromPlayedSongs)
                {
                    var items = m_tempFavorites.OfType<ListViewItem>().Select(i => i.SubItems[4].Text).ToList();
                    filaPathArray = items.ToArray<string>();
                    m_favoritesArr[m_selected_treenode].AddRange(m_tempFavorites);
                }
                else
                    m_favoritesArr.Insert(0, new List<ListViewItem>());

                Directory.CreateDirectory(m_favoritesPath);
                File.WriteAllLines(m_favoritesPath + fname, filaPathArray);

                setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_FAVORITES);
                startWorker();
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "CreateFavoriteFile", ex.LineNumber(), this.m_thisControl.Name);

            }

        }

        /// <summary>
        /// Rename Favorites.fav text file to what ever you like
        /// </summary>
        /// <param name="destinationFilename">new file name</param>
        /// <param name="sourceFilename">old file name</param>
        private void RenameFavoriteFile(string destinationFilename, string sourceFilename)
        {
            try
            {
                string destfileName = m_favoritesPath + destinationFilename + ".fav";
                string srcfileName = m_favoritesPath + sourceFilename + ".fav";

                if (File.Exists(destfileName))
                {
                    var result = MessageBox.Show("The \"" + destinationFilename + "\" filename already exist!\nSelect another filename.");
                }
                else
                {
                    File.Move(srcfileName, destfileName);
                    this.m_treeview.SelectedNode.Name = destinationFilename;
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "RenameFavoriteFile", ex.LineNumber(), this.m_thisControl.Name);

            }

        }


        /// <summary>
        /// Play/Add songs to Song Queue
        /// </summary>
        private void addToQueue()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    addToQueue();
                }));
                return;
            }

            try
            {
                lock (m_songQueue)
                {
                    if (PlayerControl.SetAsNextSong && m_songQueue.Count > 1)
                    {
                        m_songQueue.Insert(0, m_songItems);
                        m_tempFavorites.Insert(0, m_songItems);
                        updateSongQueueDuration("add");
                        PlayerControl.SetAsNextSong = false;
                    }
                    else
                    {
                        m_songQueue.Add(m_songItems);
                        m_tempFavorites.Add(m_songItems);
                        updateSongQueueDuration("add");
                    }
                }

                if (m_currentTrack == null && MediaExtension == ".mp3" && !m_IsPlayingVideo && !m_IsPlayingCdg)
                {
                    m_IsPlayingCdg = true;
                    m_IsPlayingVideo = false;
                    addToBassMixer();
                    playNextTrack();
                }
                else if (m_currentTrack == null && MediaExtension != ".mp3" && !m_IsPlayingCdg && !m_IsPlayingVideo)
                {

                    m_IsPlayingVideo = true;
                    m_IsPlayingCdg = false;
                    SetMediaInfo();
                    playNextTrack();
                }

                if (m_timerVideo.Enabled) m_timerVideo.Stop();
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "addToQueue", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Get the total playing duration of all songs on Song Queue
        /// </summary>
        /// <param name="plusMinusduration">add/minus duration</param>
        private void updateSongQueueDuration(string plusMinusduration)
        {
            try
            {
                string totalDuration = "";
                int count = m_songQueue.Count;

                switch (plusMinusduration.ToLower())
                {
                    case "add":
                        if (MediaExtension != ".mp3")
                            m_duration = GetVlcTimeOrDuration(Convert.ToDouble(m_vlc.GetTimeDuration));
                        else
                            m_duration = Convert.ToDouble(m_songItems.SubItems[3].Text);

                        m_songTotalDuration += m_duration;
                        totalDuration = String.Format("{0}", Un4seen.Bass.Utils.FixTimespan(m_songTotalDuration, "HHMMSS"));
                        break;
                    case "minus":
                        m_songTotalDuration -= m_duration;
                        totalDuration = String.Format("{0}", Un4seen.Bass.Utils.FixTimespan(m_songTotalDuration, "HHMMSS"));
                        break;
                }


                if (m_songTotalDuration < 1)
                {
                    m_treeview.Nodes[0].Text = "Song Queue (Empty)";
                }
                else
                {
                    if (count < 1)
                        m_treeview.Nodes[0].Text = "Song Queue (Empty)";
                    if (count == 1)
                        m_treeview.Nodes[0].Text = "Song Queue (" + count + " Song - " + totalDuration + ")";
                    else
                        m_treeview.Nodes[0].Text = "Song Queue (" + count + " Songs - " + totalDuration + ")";
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "updateSongQueueDuration", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Add the selected song to BassMix
        /// </summary>
        private void addToBassMixer()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    addToBassMixer();
                }));
                return;
            }

            try
            {
                lock (m_songQueue)
                {
                    if (m_songQueue.Count > 0)
                    {
                        SetMediaInfo();
                        m_track = new Player();
                        m_track.Tags = m_songQueue[0].Tag as TAG_INFO;

                        m_track.TrackSync = new SYNCPROC(OnTrackSync);
                        m_track.CreateStream();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "addToBassMixer", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Display reserve notification on UI main video window 
        /// </summary>
        private void AddToReserveNotification()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.BeginInvoke(new MethodInvoker(delegate () { AddToReserveNotification(); }));
                return;
            }
            try
            {
                if (m_songQueue.Count > 0)
                {
                    PlayerControl.IsAddToReserve = true;
                    m_reserveNotification.Start();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "AddToReserveNotification", ex.LineNumber(), this.m_thisControl.Name);

            }

        }

        /// <summary>
        /// Get the next tack to play and display on top of the full screen window for 30 sec.
        /// </summary>
        private void GetNextTrack() {
            try
            {
                if (m_songQueue.Count > 0)
                {
                    string showNextTrack = PlayerControl.TimeRemain.Text;
                    int minute = Convert.ToInt32(showNextTrack.Substring(3, 2));
                    int second = Convert.ToInt32(showNextTrack.Substring(6, 2));

                    if (minute <= 0 && second < 30)
                    {
                        string nextSong = m_songQueue[0].SubItems[1].Text + "( " + m_songQueue[0].SubItems[2].Text + " )";
                        PlayerControl.GetNextSong = nextSong;
                    }
                    else
                        PlayerControl.GetNextSong = "";
                }
                else
                    PlayerControl.GetNextSong = "";
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "GetNextTrack", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Next button = Play the next track if Song Queue is not empty
        /// </summary>
        public void PlayNext() {

            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.BeginInvoke(new Action(() =>
                {
                    PlayNext();
                }));
                return;
            }

            try {
                if (m_songQueue.Count > 0)
                {
                    SetMediaInfo();

                    if (m_currentTrack != null)
                        Bass.BASS_ChannelSlideAttribute(m_currentTrack.Channel, BASSAttribute.BASS_ATTRIB_VOL, -1f, 2000);

                    if (MediaExtension == ".mp3")
                    {
                        addToBassMixer();
                        VolumeSlideAttribute();

                    }
                    else
                    {
                        VolumeSlideAttribute();
                    }

                    if (m_previousTrack != null)
                        Bass.BASS_StreamFree(m_previousTrack.Channel);
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "PlayNext", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Not  in use at the moment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Plm_Output_Notification(object sender, EventArgs e)
        {
            if (PlayerControl.MainFormControl.InvokeRequired)
            {
                PlayerControl.MainFormControl.BeginInvoke(new MethodInvoker(delegate () { Plm_Output_Notification(sender, e); }));
                return;
            }
        }

        /// <summary>
        ///  Set media info and display the Title, Artist and time duration to UI
        /// </summary>
        private void SetMediaInfo()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.BeginInvoke(new Action(() =>
                {
                    SetMediaInfo();
                }));
                return;
            }

            try
            {
                if (m_songQueue.Count > 0)
                {
                    PlayerControl.SongTitle.Text = m_songQueue[0].SubItems[1].Text;
                    PlayerControl.SongArtist.Text = m_songQueue[0].SubItems[2].Text;
                    string duration = m_songQueue[0].SubItems[3].Text;
                    m_MediaFileName = m_songQueue[0].SubItems[4].Text;
                    if (duration == "")
                    {
                        m_vlc.GetDuration(m_MediaFileName);
                        m_duration = Convert.ToDouble(m_vlc.TimeDuration);
                    }
                    else
                        m_duration = Convert.ToDouble(m_songQueue[0].SubItems[3].Text);

                    MediaExtension = Path.GetExtension(GetExtPatern(m_MediaFileName));
                }
                else
                {
                    PlayerControl.SongTitle.Text = "";
                    PlayerControl.SongArtist.Text = "";
                    PlayerControl.Time.Text = "00:00:00";
                    PlayerControl.TimeRemain.Text = "00:00:00";
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "SetMediaInfo", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Play the next track if Song Queue is not empty
        /// </summary>
        private void playNextTrack()
        {

           /* if (this.thisControl.InvokeRequired)
            {
                this.thisControl.BeginInvoke(new Action(() =>
                {
                    playNextTrack();
                }));
                return;
            }*/
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.BeginInvoke(new MethodInvoker(delegate () { playNextTrack(); }));
                return;
            }
            try
            {
                lock (m_songQueue)
                {
                    if (Player.IsAsioInitialized && AppSettings.Get<bool>("IsAsioAutoRestart") )
                        BassAsioDevice.ReStart();

                    m_targetControl.Invalidate();

                    if (fullScreenPlayer.panelPlayer != null)
                        fullScreenPlayer.panelPlayer.Invalidate();

                    // if (m_plmOutput != null)
                    //  m_plmOutput.Notification -= new EventHandler(Plm_Output_Notification);

                    if (m_songQueue.Count > 0)
                    {
                        if (PlayerControl.ChannelSelected != ChannelSelected.Right)
                        {
                            PlayerControl.ChannelSelected = ChannelSelected.None;
                            RemoveVocalLeftOrRight();
                        }

                        PlayerControl.PlayPauseButton.BackgroundImage = Resources.ic_pause_black_36dp_1x;

                        if (MediaExtension == ".mp3")
                        {
                            m_track.Tags = m_songQueue[0].Tag as TAG_INFO;

                            m_Play = true;
                            m_Paused = false;
                            m_Stop = false;
                            m_FrameCount = 0;
                            m_Volume = PlayerControl.VolumeScroll.Value * 0.01f;
                            m_CDGFile = new CDGFile(m_MediaFileName);

                            m_previousTrack = m_currentTrack;
                            m_currentTrack = m_track as Player;

                            //   m_plmOutput = new DSP_PeakLevelMeter(m_currentTrack.Channel, 0);
                            //   m_plmOutput.Notification += new EventHandler(Plm_Output_Notification);

                            m_songQueue.RemoveAt(0);

                            m_IsPlayingCdg = m_vlc.PlayCDG();

                            if (!m_timerUpdate.Enabled)
                            {
                                m_timerUpdate.Start();
                            }

                            m_IsPlayingVideo = false;

                            InitControls();

                            //m_equalizer.Init(m_currentTrack.Channel);
                          
                            m_currentTrack.Volume = m_Volume;

                            m_currentTrack.Play();

                            if (m_previousTrack != null)
                            {
                                m_duration = Convert.ToDouble(m_previousTrack.Tags.duration);
                                updateSongQueueDuration("minus");
                            }
                            else
                                updateSongQueueDuration("minus");
                        }
                        else
                        {
                            m_songQueue.RemoveAt(0);
                            //m_duration = GetVlcTimeOrDuration(m_duration);
                            updateSongQueueDuration("minus");
                            if (m_timerVideo.Enabled)
                            {
                                m_timerVideo.Stop();
                            }


                            //m_equalizer.Init(-1);
                            m_IsPlayingVideo = m_vlc.PlayVideo(m_MediaFileName);

                            /* if (Player.IsAsioInitialized)
                             {
                                 m_plmOutput = new DSP_PeakLevelMeter(BassAsioDevice.StreamInputVlc, 0);
                                 m_plmOutput.Notification += new EventHandler(Plm_Output_Notification);
                             }*/
                            m_vlc.Volume(PlayerControl.VolumeScroll.Value);
                            InitControls();
                            if (!m_timerUpdate.Enabled) m_timerUpdate.Start();
                            m_IsPlayingCdg = false;
                        }

                        // setSEARCHDIRorTEXTState(SearchAndLoad.WRITE_TO_QUEUE_LIST);
                        // startWorker();

                        WriteToQueueList();

                        if (m_isSongQueueSelected)
                        {
                            setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_QUEUE_SONGS);
                            startWorker();

                        }

                    }
                    else
                    {
                        m_IsPlayingCdg = false;
                        m_IsPlayingVideo = false;
                        m_timerUpdate.Stop();
                        m_timerVideo.Start();

                        m_track = null;
                        m_currentTrack = null;
                        m_previousTrack = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "playNextTrack", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Fade volume before starting the next truck
        /// </summary>
        private void VolumeSlideAttribute()
        {

            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.BeginInvoke(new Action(() =>
                {
                    VolumeSlideAttribute();
                }));
                return;
            }
            try
            {
                int interval = 2000 / PlayerControl.VolumeScroll.Value;
                m_volumeCounter = PlayerControl.VolumeScroll.Value;
                m_timerVolume.Interval = interval;
                m_timerVolume.Start();
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "VolumeSlideAttribute", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Get the song duration being played using vlc (e.g mp4 files)
        /// </summary>
        /// <param name="timeOrDuration">Time duration of the file</param>
        /// <returns></returns>
        private double GetVlcTimeOrDuration(double timeOrDuration)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(timeOrDuration);
            return t.TotalSeconds;
        }

        /// <summary>
        /// Not in use at moment
        /// </summary>
        private void PreProcessFiles()
        {
            try
            {
                string CDGFileName = "";
                // if (Regex.IsMatch(tbFileName.Text, "\\.zip$"))
                // {
                //     string myTempDir = Path.GetTempPath() + Path.GetRandomFileName();
                //     Directory.CreateDirectory(myTempDir);
                //     mTempDir = myTempDir;
                //     myCDGFileName = Unzip.UnzipMP3GFiles(tbFileName.Text, myTempDir);
                //     goto PairUpFiles;
                // }
                // else 
                if (Regex.IsMatch(m_MediaFileName, "\\.cdg$"))
                {
                    CDGFileName = m_MediaFileName;
                    goto PairUpFiles;
                }

                PairUpFiles:
                string MP3FileName = Regex.Replace(CDGFileName, "\\.cdg$", ".mp3");
                if (File.Exists(MP3FileName))
                {
                    m_MP3FileName = MP3FileName;
                    m_MediaFileName = CDGFileName;
                    m_TempDir = "";
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "PreProcessFiles", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// Not in use at moment
        /// </summary>
        private void CleanUp()
        {
            if (!string.IsNullOrEmpty(m_TempDir))
            {
                try
                {
                    Directory.Delete(m_TempDir, true);
                }
                catch (Exception ex)
                {
                }
            }
            m_TempDir = "";
        }

        /// <summary>
        /// Add songs to Song Queue
        /// </summary>
        /// <returns></returns>
        private List<ListViewItem> LoadQueueSongs()
        {
            PlayerControl.AllSongs = new List<ListViewItem>();
            try
            {
                PlayerControl.AllSongs.AddRange(m_songQueue);
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "LoadQueueSongs", ex.LineNumber(), this.m_thisControl.Name);

            }
            return PlayerControl.AllSongs;
        }

        /// <summary>
        /// Get the cdg file and replace cdg to mp3
        /// </summary>
        /// <param name="fileName">the filename to replace</param>
        /// <returns></returns>
        private string GetExtPatern(string fileName)
        {
            string extPattern = ".cdg$";
            Regex regX = new Regex(extPattern, RegexOptions.IgnoreCase);
            string MediaFileName = regX.Replace(fileName, ".mp3");

            return MediaFileName;
        }

        /// <summary>
        /// Search song on listview and select to add to Song Queue
        /// </summary>
        /// <param name="searchString">Song tilte/ artist</param>
        /// <returns></returns>
        private List<ListViewItem> SearchListView(string searchString)
        {
            PlayerControl.AllSongs = new List<ListViewItem>();
            try
            {
                if (searchString != "")

                    for (int i = 0; i < m_songsArr.Count; i++)
                    {
                        PlayerControl.AllSongs.AddRange(m_songsArr[i].Where(s => s.SubItems[1].Text.ToLower().Contains(searchString.ToLower()) || s.SubItems[2].Text.ToLower().Contains(searchString.ToLower())).ToList());
                    }
                else
                    PlayerControl.AllSongs.AddRange(m_songsArr[m_selected_treenode]);
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "SearchListView", ex.LineNumber(), this.m_thisControl.Name);

            }
            return PlayerControl.AllSongs;
        }

        /// <summary>
        /// Select added file from treeview and load content to litsview. (e.g Song Queue, Favorites, My computer )
        /// </summary>
        /// <param name="sDir">selected file from treeview</param>
        /// <returns>Returns the list of song available</returns>
        private List<ListViewItem> TextSearchSongs(string sDir)
        {
            int count = 0;

            PlayerControl.AllSongs = new List<ListViewItem>();
            //extensions.Contains(Path.GetExtension(s))
            try
            {
                PlayerControl.AllSongs.AddRange(File.ReadAllLines(sDir).Where(s => m_extensions.Contains(Path.GetExtension(s))) // s.EndsWith(".cdg") || s.EndsWith(".CDG") || s.EndsWith(".mp4") || s.EndsWith(".MP4"))
                    .Select(s =>
                    {
                        return trackInfo(s, count++).toListViewItem();

                    }).ToList());
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "TextSearchSongs", ex.LineNumber(), this.m_thisControl.Name);

            }
            return PlayerControl.AllSongs;
        }

        /// <summary>
        /// Select Folder that contains mp3cdg and mp4 from directory to add it to listview and to ProgramData\karaokeNow\songs\filename.bkn
        /// </summary>
        /// <param name="sDir"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        private List<ListViewItem> DirSearchSongs(string sDir)
        {           
            int count = 0;
            PlayerControl.AllSongs = new List<ListViewItem>();
            try
            {
                PlayerControl.AllSongs = Directory.EnumerateFiles(sDir, "*.*", SearchOption.AllDirectories)
                  .Where(s => m_extensions.Contains(Path.GetExtension(s))).Select(s =>
                  {

                      return trackInfo(s, count++).toListViewItem();

                  }).ToList();

                m_songsArr.Insert(0, PlayerControl.AllSongs);
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "DirSearchSongs", ex.LineNumber(), this.m_thisControl.Name);

            }
            return PlayerControl.AllSongs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<ListViewItem> Shuffle()
        {
            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    Shuffle();

                }));
                return PlayerControl.AllSongs;
            }


            string nodeName = "";
            Random rnd = new Random();
            PlayerControl.AllSongs = new List<ListViewItem>();
            try
            {
                nodeName = this.m_treeview.SelectedNode.Name;
                foreach (var node in Collect(this.m_treeview.Nodes))
                {
                    if (nodeName == node.Name)
                    {
                        if (nodeName == node.Text)
                        {//its ceated dynamically
                            if (m_songsArr[m_selected_treenode].Count > 0)
                            {
                                m_songsArr[m_selected_treenode] = new List<ListViewItem>(m_songsArr[m_selected_treenode].OrderBy(x => rnd.Next()).ToList());
                                PlayerControl.AllSongs.AddRange(m_songsArr[m_selected_treenode]);
                            }
                        }
                        else
                        { //created manually
                            if (m_songQueue.Count > 1 && node.Index == 0) //Song Queue
                            {
                                m_songQueue = new List<ListViewItem>(m_songQueue.OrderBy(x => rnd.Next()).ToList());
                                PlayerControl.AllSongs.AddRange(m_songQueue);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "Shuffle", ex.LineNumber(), this.m_thisControl.Name);

            }

            return PlayerControl.AllSongs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private List<ListViewItem> SortList(int column) {

            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.Invoke(new Action(() =>
                {
                    SortList(column);

                }));
                return PlayerControl.AllSongs;
            }

            string nodeName = "";
            PlayerControl.AllSongs = new List<ListViewItem>();

            try
            {
                nodeName = this.m_treeview.SelectedNode.Name;
                foreach (var node in Collect(this.m_treeview.Nodes))
                {
                    if (nodeName == node.Name)
                    {
                        if (nodeName == node.Text)
                        {//its ceated dynamically
                            if (m_songsArr[m_selected_treenode].Count > 0)
                            {
                                if (m_sortColumn != column)
                                {
                                    m_songsArr[m_selected_treenode] = new List<ListViewItem>(m_songsArr[m_selected_treenode].OrderBy(s => s.SubItems[column].Text).ToList());
                                    m_sortColumn = column;
                                }
                                else
                                {
                                    m_songsArr[m_selected_treenode] = new List<ListViewItem>(m_songsArr[m_selected_treenode].OrderByDescending(s => s.SubItems[column].Text).ToList());
                                    m_sortColumn = -1;
                                }

                                PlayerControl.AllSongs.AddRange(m_songsArr[m_selected_treenode]);
                            }
                        }
                        else
                        { //created manually
                            if (m_songQueue.Count > 1 && node.Index == 0) //Song Queue
                            {
                                if (m_sortColumn != column)
                                {
                                    m_songQueue = new List<ListViewItem>(m_songQueue.OrderBy(s => s.SubItems[column].Text).ToList());
                                    m_sortColumn = column;
                                }
                                else
                                {
                                    m_songQueue = new List<ListViewItem>(m_songQueue.OrderByDescending(s => s.SubItems[column].Text).ToList());
                                    m_sortColumn = -1;
                                }

                                PlayerControl.AllSongs.AddRange(m_songQueue);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "SortList", ex.LineNumber(), this.m_thisControl.Name);

            }
            return PlayerControl.AllSongs;
        }

        /// <summary>
        /// Background worker to process task on background to avoid UI Freeze
        /// </summary>
        /// <param name="arg">function to process</param>
        public void startWorker(object arg = null)
        {
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += DoWork;
            bgw.RunWorkerCompleted += Workers_Completed;
            m_bgws.Add(bgw);
            bgw.RunWorkerAsync(arg);

            while (bgw.IsBusy)
            {
                Application.DoEvents();
                if (bgw.IsBusy)
                    m_treeview.Enabled = false;
                else
                    m_treeview.Enabled = true;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            string fName = (string)e.Argument;
            switch (getSEARCHDIRorTEXTState)
            {
                case SearchAndLoad.SEARCH_DIRECTORY:
                    e.Result = DirSearchSongs(fName);
                    break;
                case SearchAndLoad.SEARCH_TEXTFILE:
                    e.Result = TextSearchSongs(fName);
                    break;
                case SearchAndLoad.LOAD_QUEUE_SONGS:
                    e.Result = LoadQueueSongs();
                    break;
                case SearchAndLoad.LOAD_FROM_FILE_TO_QUEUE:
                    LoadFromFileToQueue();
                    e.Result = null;
                    break;
                case SearchAndLoad.SEARCH_LISTVIEW:
                    e.Result = SearchListView(fName);
                    break;
                case SearchAndLoad.SHUFFLE_SONGS:
                    e.Result = Shuffle();
                    break;
                case SearchAndLoad.SORT_SONGS:
                    e.Result = SortList(int.Parse(fName));
                    break;
                case SearchAndLoad.LOAD_FAVORITES:
                    e.Result = m_favoritesArr[m_selected_treenode];
                    break;
                case SearchAndLoad.LOAD_ADDED_SONGS:
                    e.Result = m_songsArr[m_selected_treenode];
                    break;
                case SearchAndLoad.ADD_SELECTED_SONG_TO_QUEUE:
                   // on_SongListView_Item_Selected();
                    break;
                case SearchAndLoad.WRITE_TO_QUEUE_LIST:
                    WriteToQueueList();
                    e.Result = null;
                    break;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Workers_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                BackgroundWorker bgw = (BackgroundWorker)sender;

               // if (getSEARCHDIRorTEXTState != SearchAndLoad.WRITE_TO_QUEUE_LIST)
               // {
                   // PlayerControl.AllSongs = new List<ListViewItem>();
                  //  PlayerControl.AllSongs = (List<ListViewItem>)e.Result;
                // }
                switch (getSEARCHDIRorTEXTState) {
                    case SearchAndLoad.SEARCH_DIRECTORY:

                        PlayerControl.AllSongs = new List<ListViewItem>();
                        PlayerControl.AllSongs = (List<ListViewItem>)e.Result;

                        string fPath = m_filePath + @"songs\";
                        string filename = Path.GetFileName(m_fbd.SelectedPath);
                        var items = PlayerControl.AllSongs.OfType<ListViewItem>().Select(i => i.SubItems[4].Text).ToList();
                        items.Insert(0, m_fbd.SelectedPath);
                        string[] filePathArray = items.ToArray<string>();
                        Directory.CreateDirectory(fPath);
                        File.WriteAllLines(fPath + filename + ".bkN", filePathArray);

                        m_bgws.Remove(bgw);
                        bgw.Dispose();

                        if (m_bgws.Count == 0 && m_IsSearchingListView && PlayerControl.AllSongs != null)
                        {
                            PlayerControl.SongListView.VirtualListSize = PlayerControl.AllSongs.Count;
                            PlayerControl.SongListView.Refresh();
                        }
                        break;
                    case SearchAndLoad.SEARCH_TEXTFILE:
                    case SearchAndLoad.LOAD_QUEUE_SONGS:
                    case SearchAndLoad.LOAD_FROM_FILE_TO_QUEUE:
                    case SearchAndLoad.SEARCH_LISTVIEW:
                    case SearchAndLoad.SHUFFLE_SONGS:
                    case SearchAndLoad.SORT_SONGS:
                    case SearchAndLoad.LOAD_FAVORITES:
                    case SearchAndLoad.LOAD_ADDED_SONGS:
                   // case SearchAndLoad.ADD_SELECTED_SONG_TO_QUEUE:

                        PlayerControl.AllSongs = new List<ListViewItem>();
                        PlayerControl.AllSongs = (List<ListViewItem>)e.Result;

                        m_bgws.Remove(bgw);
                        bgw.Dispose();

                        if (m_bgws.Count == 0 && m_IsSearchingListView && PlayerControl.AllSongs != null)
                        {
                            PlayerControl.SongListView.VirtualListSize = PlayerControl.AllSongs.Count;
                            PlayerControl.SongListView.Refresh();
                        }
                        break;
                    case SearchAndLoad.WRITE_TO_QUEUE_LIST:

                        break;

                }
              /*  if (getSEARCHDIRorTEXTState == SearchAndLoad.SEARCH_DIRECTORY)
                {
                    string fPath = m_filePath + @"songs\";
                    string filename = Path.GetFileName(m_fbd.SelectedPath);
                    var items = PlayerControl.AllSongs.OfType<ListViewItem>().Select(i => i.SubItems[4].Text).ToList();
                    items.Insert(0, m_fbd.SelectedPath);
                    string[] filePathArray = items.ToArray<string>();
                    Directory.CreateDirectory(fPath);
                    File.WriteAllLines(fPath + filename + ".bkN", filePathArray);
                }*/

               /* m_bgws.Remove(bgw);
                bgw.Dispose();

                if (m_bgws.Count == 0 && m_IsSearchingListView && PlayerControl.AllSongs != null)
                {
                    PlayerControl.SongListView.VirtualListSize = PlayerControl.AllSongs.Count;
                    PlayerControl.SongListView.Refresh();
                }*/
                // MessageBox.Show("All workers complete");
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "Workers_Completed", ex.LineNumber(), this.m_thisControl.Name);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_extensions"></param>
        /// <returns></returns>
        private static string HashSetExtensionsToString(HashSet<string> file_extensions)
        {

            string fileExt = "";

            foreach (string s in file_extensions) {
                fileExt += s.Replace(s, s + "|");
            }

            return fileExt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private TrackInfo trackInfo(string file, int count, int duration = 0)
        {
            string SongTitle = "";
            string SongArtist = "";
            string pattern = @"-\s+|–\s+|-|–";

            try
            {
                // string extPattern = ".cdg|.mp4";
                string fName = Path.GetFileName(file);
                string[] regXpattern = Regex.Split(fName, pattern);
                // var containsSwears = extensions.Any(w => file.Contains(w));

                Regex regX = new Regex(m_extPattern, RegexOptions.IgnoreCase);

                switch (regXpattern.Length)
                {
                    case 1:
                        SongTitle = regX.Replace(regXpattern[0], "");
                        SongArtist = regX.Replace(regXpattern[0], "");
                        count++;
                        break;
                    case 2:
                        SongTitle = regX.Replace(regXpattern[regXpattern.Length - 1], "");
                        SongArtist = regX.Replace(regXpattern[0], "");
                        count++;
                        break;
                    case 3:
                        SongTitle = regX.Replace(regXpattern[(regXpattern.Length - 1)], "");
                        SongArtist = regX.Replace(regXpattern[(regXpattern.Length - 2)], "");
                        count++;
                        break;
                    case 4:
                        SongTitle = regX.Replace(regXpattern[(regXpattern.Length - 1)], "");
                        SongArtist = regX.Replace(regXpattern[(regXpattern.Length - 2)], "");
                        count++;
                        break;
                    case 5:
                        SongTitle = regX.Replace(regXpattern[(regXpattern.Length - 1)], "");
                        SongArtist = regX.Replace(regXpattern[(regXpattern.Length - 2)], "");
                        count++;
                        break;
                    case 6:
                        SongTitle = regX.Replace(regXpattern[(regXpattern.Length - 1)], "");
                        SongArtist = regX.Replace(regXpattern[(regXpattern.Length - 2)], "");
                        count++;
                        break;
                }

            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "trackInfo", ex.LineNumber(), this.m_thisControl.Name);

            }
            TrackInfo trackInfo = new TrackInfo();
            trackInfo.ID = Convert.ToString(count);
            trackInfo.Name = SongTitle;
            trackInfo.Artist = SongArtist;
            trackInfo.Duration = Convert.ToString(duration);
            trackInfo.FilePath = file;

            return trackInfo;
        }


        // Bass Operation ###################################

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="user"></param>
        private void OnTrackSync(int handle, int channel, int data, IntPtr user)
        {
            this.m_thisControl.ExecuteAsync(delegate
            {
                SetMediaInfo();
            });

            try
            {
                if (m_songQueue.Count <= 0)
                    m_currentTrack.NextTrackSync = 0;
                else
                    m_currentTrack.NextTrackSync = 1;

                user = new IntPtr(m_currentTrack.NextTrackSync);

                if (user.ToInt32() == 0)
                {
                    // END SYNC
                    this.m_thisControl.ExecuteAsync(playNextTrack);
                }
                else
                {
                    // POS SYNC
                    this.m_thisControl.ExecuteAsync(delegate
                    {
                    // this code runs on the UI thread!
                    if (MediaExtension == ".mp3")
                            addToBassMixer();

                        playNextTrack();
                    // and fade out and stop the 'previous' track (for 2 seconds)
                    if (m_previousTrack != null)
                            Bass.BASS_ChannelSlideAttribute(m_previousTrack.Channel, BASSAttribute.BASS_ATTRIB_VOL, -1f, 2000);
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "OnTrackSync", ex.LineNumber(), this.m_thisControl.Name);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="user"></param>
        private void OnMixerStall(int handle, int channel, int data, IntPtr user)
        {
            this.m_thisControl.ExecuteAsync(delegate
            {
                try
                {
                    // this code runs on the UI thread!
                    if (data == 0)
                    {
                        // mixer stalled
                        // 
                        if (!m_IsPlayingCdg && !m_IsPlayingVideo)
                        {
                            m_timerUpdate.Stop();
                            m_timerVideo.Start();

                            m_track = null;
                            m_currentTrack = null;
                            m_previousTrack = null;
                        }

                        // InitControls();
                    }
                   // else
                  //  {
                        // mixer resumed
                  //      m_timerVideo.Stop();
                 //       m_timerUpdate.Start();
                 //   }
                }
                catch (Exception ex)
                {
                    Logger.LogFile(ex.Message, "", "OnMixerStall", ex.LineNumber(), this.m_thisControl.Name);

                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="posF"></param>
        /// <param name="len"></param>
        private void DrawPlayerPosition(long pos, float posF, long len = 0)
        {

            if (pos < 0 || posF < 0)
            {
                PlayerControl.PlayProgress.Image = null;
                return;
            }

            Bitmap bitmap = null;
            Graphics g = null;
            double bpp = 0;
            SolidBrush myBrush = null;
            LinearGradientBrush lb = null;
            int x = 0;

            try
            {
                if (len != 0)
                {
                    bpp = len / (double)PlayerControl.PlayProgress.Width;
                    x = (int)Math.Round(pos / bpp); // position (x) where to draw the line
                }
                else
                    x = (int)Math.Round(posF * (double)PlayerControl.PlayProgress.Width);

                myBrush = new SolidBrush(Color.Red);
                Rectangle rec = new Rectangle(0, 0, PlayerControl.PlayProgress.Width, PlayerControl.PlayProgress.Height);
                lb = new LinearGradientBrush(rec, Color.FromArgb(168, 224, 99), Color.FromArgb(86, 171, 47), LinearGradientMode.Vertical);

                bitmap = new Bitmap(PlayerControl.PlayProgress.Width, PlayerControl.PlayProgress.Height);
                g = Graphics.FromImage(bitmap);
                g.Clear(Color.Black);

                g.FillRectangle(lb, 0, 0, x, PlayerControl.PlayProgress.Height - 1);
                bitmap.MakeTransparent(Color.Black);
            }

            catch (Exception ex)
            {
                bitmap = null;
            }
            finally
            {
                // clean up graphics resources
                if (myBrush != null)
                    myBrush.Dispose();
                if (lb != null)
                    lb.Dispose();
                if (g != null)
                    g.Dispose();
            }

            PlayerControl.PlayProgress.Image = bitmap;
            PlayerControl.PlayProgress.Refresh();
        }


    
        //Temporary Solution for Android remote app

        public string GetNowPlaying()
        {
            string nowPlaying = PlayerControl.SongTitle.Text + " " + PlayerControl.SongArtist.Text;
            return nowPlaying = string.IsNullOrEmpty(nowPlaying) ? "" : nowPlaying;

        }

        public List<List<ListViewItem>> UpdateSongsDatabase() 
        {
            return m_songsArr;
        }

        public void AddSongToQueue(string songId) {

            if (this.m_thisControl.InvokeRequired)
            {
                this.m_thisControl.BeginInvoke(new MethodInvoker(delegate () { AddSongToQueue(songId); }));
                return;
            }

            try
            {
                TrackInfo trackInfo = null;
                m_songItems = new ListViewItem();

                m_songItems = m_songsArr[m_selected_treenode][Convert.ToInt32(songId) - 1];

                string filePath = m_songItems.SubItems[4].Text;
                string MediaFileName = GetExtPatern(filePath);
                if (File.Exists(MediaFileName))
                {
                    if (MediaFileName.EndsWith(".mp3"))
                    {
                        trackInfo = new TrackInfo(MediaFileName);
                    }
                    else
                    {
                        trackInfo = new TrackInfo();
                        m_vlc.GetDuration(MediaFileName);
                        trackInfo.Duration = GetVlcTimeOrDuration(Convert.ToDouble(m_vlc.GetTimeDuration)).ToString();
                    }


                    MediaExtension = Path.GetExtension(MediaFileName).ToLower();
                    trackInfo.ID = m_songItems.SubItems[0].Text.ToString();
                    trackInfo.Name = m_songItems.SubItems[1].Text.ToString();
                    trackInfo.Artist = m_songItems.SubItems[2].Text.ToString();
                    trackInfo.FilePath = filePath;

                    m_songItems = new ListViewItem();
                    m_songItems = trackInfo.toListViewItem();
                    addToQueue();


                    setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_QUEUE_SONGS);
                    startWorker();
                }
                else
                {
                   // MessageBox.Show("Cannot find " + Path.GetFileName(MediaFileName) + " file to play.");
                    return;

                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "AddSongToQueue", ex.LineNumber(), this.m_thisControl.Name);

            }

        }
        private string MediaExtension { get; set; }
    }
}
