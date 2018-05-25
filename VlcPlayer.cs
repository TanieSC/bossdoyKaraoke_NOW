using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Declarations;
using Declarations.Enums;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using Implementation;

namespace bossdoyKaraoke_NOW.Models
{
    class VlcPlayer : IDisposable
    {
      
        public static string BGVideoPath = AppDomain.CurrentDomain.BaseDirectory;
        static HashSet<string> m_extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".vob", ".mp4", ".flv", ".avi", ".3gp", ".mov", ".mkv", ".mpg", ".wav", ".wmv" };
        // string path = @"D:\Downloads\PlatinumDVD_Vol72\New folder\VIDEO_EVENT\001.vob";
        // string path2 = @"D:\Downloads\PlatinumDVD_Vol72\New folder\VIDEO_EVENT\002.vob";
        // string path3 = @"D:\Downloads\PlatinumDVD_Vol72\New folder\VIDEO_EVENT\003.vob";

            // string path1 = m_bgVideoPath + @"VIDEO_EVENT\001.vob";
            //  string path2 = m_bgVideoPath + @"VIDEO_EVENT\002.vob";
            // string path3 = m_bgVideoPath + @"VIDEO_EVENT\003.vob";
            // string path4 = m_bgVideoPath + @"VIDEO_EVENT\004.vob";

        static string path1 = BGVideoPath + @"VIDEO_MTV\001.mp4";
        static string path2 = BGVideoPath + @"VIDEO_MTV\002.mp4";
        static string path3 = BGVideoPath + @"VIDEO_MTV\003.mp4";
        static string path4 = BGVideoPath + @"VIDEO_MTV\004.mp4";
        string[] videoPath;// = new string[] { path1, path2, path3, path4 };
        string videoDir;


        IMediaPlayerFactory m_factory;
        IVideoPlayer m_player;
        IMedia m_media;
        IMemoryRenderer m_memRender;
        IMediaList m_media_list, m_media_list_preview;
        IMediaListPlayer m_list_player, m_list_preview_player;
        IAudioRenderer m_audioRender;
        IMemoryInputMedia m_renderMedia;
        IAudioPlayer m_renderPlayer;
        private int bassStreamHandle;
        private bool m_cdg;
        private bool m_video;
        private bool m_paused;
        private bool m_isVbcableInstalled = false;
        private bool m_isSetpreviewCalled = false;

        public VlcPlayer()
        {

            m_factory = new MediaPlayerFactory();
            m_player = m_factory.CreatePlayer<IVideoPlayer>();
            m_media_list = m_factory.CreateMediaList<IMediaList>();
           /* m_media_list_preview = m_factory.CreateMediaList<IMediaList>();

          

            m_list_preview_player = m_factory.CreateMediaListPlayer<IMediaListPlayer>(m_media_list_preview);
            m_list_preview_player.PlaybackMode = PlaybackMode.Loop;  
            m_list_preview_player.InnerPlayer.Mute = true;*/

            m_player.Events.PlayerPositionChanged += new EventHandler<MediaPlayerPositionChanged>(Events_PlayerPositionChanged);
            m_player.Events.TimeChanged += new EventHandler<MediaPlayerTimeChanged>(Events_TimeChanged);
            m_player.Events.MediaEnded += new EventHandler(Events_MediaEnded);
            m_player.Events.PlayerStopped += new EventHandler(Events_PlayerStopped);

            videoDir = Main_Form.iniFileHelper.Read("Video", "Video Path");

            if (videoDir == string.Empty) videoDir = BGVideoPath + @"VIDEO_NATURE\";

            GetVideoBG(videoDir);

            SetAudioOutputDevice();

        }

        public void SetVideoSize(int w, int h) {
            m_list_player.InnerPlayer.Stop();
            LoadDefaultVideoBG();
        }

        public void SetAudioOutputDevice() {

            foreach (AudioOutputModuleInfo module in m_factory.AudioOutputModules)
            {
                List<AudioOutputDeviceInfo> info = m_factory.GetAudioOutputDevices(module).ToList();

                foreach (var s in info)
                {
                    if (Player.IsAsioInitialized)
                    {
                        if (s.Longname.Contains("VB-Audio Virtual"))
                        {
                            //Console.WriteLine(s.Id + " " + s.Longname);
                            m_player.SetAudioOutputModuleAndDevice(module, s);
                            m_isVbcableInstalled = true;
                            LoadDefaultVideoBG();
                            return;
                        }

                        m_isVbcableInstalled = false;
                    }
                    if (Player.IsBassInitialized || Player.IsWasapiInitialized)
                    {
                        if (s.Longname.Contains(Player.DefaultDeviceLongName.Substring(0, 31)))
                        {
                            m_player.SetAudioOutputModuleAndDevice(module, s);
                            LoadDefaultVideoBG();
                        }
                    }

                }
            }
        }

        public bool GetVideoBG(string sDir)
        {

            bool isVideoFound = false;

            if (sDir != string.Empty)
            {
                videoPath = Directory.EnumerateFiles(sDir, "*.*", SearchOption.AllDirectories)
                       .Where(s => m_extensions.Contains(Path.GetExtension(s)))
                       .Select(s =>
                       {
                           return s;
                       }).ToArray();

                if (videoPath.Count() > 0)
                {
                    isVideoFound = true;
                }
                else
                {
                    MessageBox.Show("No supported video file(s) found!");
                }
            }

            return isVideoFound;

            /*List<string> folders = new DirectoryInfo(@"D:\Downloads\SunFly Karaoke").EnumerateDirectories("*.*", SearchOption.AllDirectories)
                 .Where(d => d.EnumerateFiles("*.*", SearchOption.AllDirectories)
                 .Where(f => m_extensions.Contains(Path.GetExtension(f.Extension)))
                 .Any()).Select(s => s.FullName).ToList();*/
        }

        public void SetDefaultVideoBG(IntPtr handle) {

            if (videoDir != string.Empty)
            {

                if (m_media_list_preview.Count() > 0)
                    m_media_list_preview.Clear();

                for (int i = 0; i < videoPath.Length; i++)
                {
                    m_media = m_factory.CreateMedia<IMediaFromFile>(videoPath[i]);
                    m_media_list_preview.Add(m_media);
                }

                m_list_preview_player.InnerPlayer.WindowHandle = handle;
                StopPreviewVideoBG();
                m_media.Parse(true);
                m_list_preview_player.Play();
                // Console.WriteLine(m_media_list_preview.Count);
                // m_list_preview_player.Position = m_list_player.Position;
            }
        }


        public void ViewNextPreviewVideoBG()
        {
            m_list_preview_player.PlayNext();
        }

        public void ViewPreviousPreviewVideoBG()
        {
            m_list_preview_player.PlayPrevios();
        }

        public void StopPreviewVideoBG()
        {
            if (m_list_preview_player.IsPlaying) m_list_preview_player.Stop();
        }

        public void LoadDefaultVideoBG()
        {          
            if (m_player.IsPlaying) m_player.Stop();

            if (videoDir != string.Empty)
            {
                if(m_media_list.Count() > 0) m_media_list.Clear();
   
                for (int i = 0; i < videoPath.Length; i++)
                {
                    m_media = m_factory.CreateMedia<IMediaFromFile>(videoPath[i]);
                    m_media_list.Add(m_media);
                }


                m_list_player = m_factory.CreateMediaListPlayer<IMediaListPlayer>(m_media_list);
                m_memRender = m_list_player.InnerPlayer.CustomRenderer;
                m_memRender.SetCallback(delegate (Bitmap frame)
                {
                    Video = (Bitmap)frame.Clone();
                });

                // 4:3 aspect ratio resolutions: 640×480, 800×600, 960×720, 1024×768, 1280×960, 1400×1050, 1440×1080 , 1600×1200, 1856×1392, 1920×1440, and 2048×1536
                //16:9 aspect ratio resolutions: 1024×576, 1152×648, 1280×720, 1366×768, 1600×900, 1920×1080, 2560×1440 and 3840×2160

                m_memRender.SetFormat(new BitmapFormat(1024, 576, ChromaType.RV32));
                m_list_player.PlaybackMode = PlaybackMode.Loop;
                m_list_player.InnerPlayer.Mute = true;
                m_media.Parse(true);
                if (m_list_player.IsPlaying) m_list_player.Stop();
                m_list_player.Play();
            }
        }

        public bool PlayCDG()
        {
            if (m_player.IsPlaying) m_player.Stop();
            m_list_player.PlayNext();
            m_cdg = true;
            return m_cdg;
        }

        public bool PlayVideo(string filePath)
        {

            if (m_list_player.IsPlaying)
            {
                m_list_player.Stop();
            }

            m_media = m_factory.CreateMedia<IMediaFromFile>(filePath);
            m_memRender = m_player.CustomRenderer;
            m_memRender.SetCallback(delegate (Bitmap frame)
            {
                Video = (Bitmap)frame.Clone();

            });

            m_memRender.SetFormat(new BitmapFormat(1024, 576, ChromaType.RV32));
            m_media.Events.DurationChanged += new EventHandler<MediaDurationChange>(Events_DurationChanged);
            m_media.Events.StateChanged += new EventHandler<MediaStateChange>(Events_StateChanged);
            m_media.Events.ParsedChanged += new EventHandler<MediaParseChange>(Events_ParsedChanged);

            /*  m_renderPlayer = m_factory.CreatePlayer<IAudioPlayer>();
             m_renderMedia = m_factory.CreateMedia<IMemoryInputMedia>(MediaStrings.IMEM);
             m_audioRender = m_player.CustomAudioRenderer;

             var fc = new Func<SoundFormat, SoundFormat>(SoundFormatCallback);
             m_player.CustomAudioRenderer.SetFormatCallback(fc);
             var ac = new AudioCallbacks { SoundCallback = SoundCallback };
             m_player.CustomAudioRenderer.SetCallbacks(ac);
             m_player.CustomAudioRenderer.SetExceptionHandler(Handler);
             GC.KeepAlive(m_player);*/

            m_player.Channel = AudioChannelType.Stereo;
            m_player.Open(m_media);
            m_media.Parse(true);
            m_player.Play();
            m_video = true;
            MediaEnded = false;

            return m_video;
        }

        private SoundFormat SoundFormatCallback(SoundFormat sf)
        {
            var streamInfo = new StreamInfo();
            streamInfo.Category = StreamCategory.Audio;
            streamInfo.Codec = sf.SoundType;
            streamInfo.Channels = sf.Channels;
            streamInfo.Samplerate = sf.Rate;

            m_renderMedia.Initialize(streamInfo);
            m_renderPlayer.Open(m_renderMedia);
            return sf;
        }
        void Handler(Exception ex)
        {
            Console.WriteLine(ex.StackTrace, "VLC Audio");
        }

        private void SoundCallback(Sound soundData)
        {
            FrameData audioData = new FrameData { DTS = -1 };
            audioData.Data = soundData.SamplesData;
            audioData.DataSize = (int)soundData.SamplesSize;
            audioData.PTS = soundData.Pts;
            m_renderMedia.AddFrame(audioData);
           if (!m_renderPlayer.IsPlaying)
                   m_renderPlayer.Play();//play renderer

        }


        public bool PlayPause()
        {
            if (m_player.IsPlaying) {
                m_player.Pause();
                m_paused = true;
            }
            else
            {
                m_player.Play();
                m_paused = false;
            }
            return m_paused;
        }

        public void Stop() {
            if (m_player.IsPlaying)
                m_player.Stop();
        }

        public void Volume(int volume) {
            m_player.Volume = volume;
        }

        public void Mute() {
            m_player.ToggleMute();
        }

        public void GetDuration(string filePath) {

            IMedia media = m_factory.CreateMedia<IMediaFromFile>(filePath);
            IVideoPlayer player = m_factory.CreatePlayer<IVideoPlayer>();
            m_memRender = player.CustomRenderer;
            m_memRender.SetFormat(new BitmapFormat(1, 1, ChromaType.RV24));
            media.Events.DurationChanged += new EventHandler<MediaDurationChange>(Events_GetTimeDuration);
            media.Parse(true);
            player.Play();
            player.Stop();

            Thread.Sleep(100);
            player.Dispose();
            m_memRender.Dispose();
            media.Dispose();
        }

        void Events_PlayerStopped(object sender, EventArgs e)
        {
            InitControls();
            MediaEnded = true;
        }

        void Events_MediaEnded(object sender, EventArgs e)
        {
            InitControls();
            MediaEnded = true;
        }

        void Events_TimeChanged(object sender, MediaPlayerTimeChanged e)
        {
            Time = Convert.ToDouble(e.NewTime).ToString(); //TimeSpan.FromMilliseconds(e.NewTime).ToString().Substring(0, 8);
        }

        void Events_PlayerPositionChanged(object sender, MediaPlayerPositionChanged e)
        {
            PlayerPosition = e.NewPosition;// (int)(e.NewPosition * 100);
        }

        void Events_StateChanged(object sender, MediaStateChange e)
        {
            
        }

        void Events_GetTimeDuration(object sender, MediaDurationChange e)
        {
            GetTimeDuration = Convert.ToDouble(e.NewDuration).ToString();
        }

        void Events_DurationChanged(object sender, MediaDurationChange e)
        {
            TimeDuration = Convert.ToDouble(e.NewDuration).ToString();
        }

        void Events_ParsedChanged(object sender, MediaParseChange e)
        {
            Console.WriteLine(e.Parsed);
        }

        private void InitControls() {
            PlayerPosition = 0;
            Time = "00:00:00";
            TimeDuration = "00:00:00";
            MediaEnded = true;
        }

        public string Time { get; private set; }
        public string TimeDuration { get; private set; }
        public string GetTimeDuration { get; private set; }
        public float PlayerPosition { get; private set; }
        public Bitmap Video { get; private set; }
        public bool MediaEnded { get; private set; }
        public bool IsVirtualCableInstalled {
            get { return m_isVbcableInstalled; }
            set { m_isVbcableInstalled = value; }
        }

        #region IDisposable Support
        /// <summary>
        /// Performs object finalization.
        /// </summary>
        ~VlcPlayer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes of object resources.
        /// </summary>
        /// <param name="disposeManagedResources">If true, managed resources should be
        /// disposed of in addition to unmanaged resources.</param>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                m_factory.Dispose();
                m_player.Dispose();
                m_media.Dispose();
                m_memRender.Dispose();
                m_media_list.Dispose();
                m_list_player.Dispose();
                m_media_list_preview.Dispose();
                m_list_preview_player.Dispose();
            }
        }
        #endregion
    }
}
