using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using bossdoyKaraoke_NOW.CDG;
using bossdoyKaraoke_NOW.Models;
using System.Drawing;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.Bass.AddOn.Mix;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using static bossdoyKaraoke_NOW.Enums.Songs;
using bossdoyKaraoke_NOW.Properties;
using static bossdoyKaraoke_NOW.Enums.TreviewNode;
using static bossdoyKaraoke_NOW.Enums.PlayerState;
using Un4seen.Bass.AddOn.Tags;

namespace bossdoyKaraoke_NOW
{
    public class Main_Form1
    {
        string m_filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\karaokeNow\\";

        List<BackgroundWorker> bgws = new List<BackgroundWorker>();

        private GraphicUtilDX gdx;
        private Control targetControl;

        private VlcPlayer vlc;
        private Bitmap BgVideo;
        private bool IsPlayingVideo;
        private bool IsPlayingCdg;

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
        private List<List<ListViewItem>> songsArr = new List<List<ListViewItem>>();
        private ListViewItem m_songItems;
        private List<ListViewItem> m_songQueue = new List<ListViewItem>();
        private TreeView treeview;
        private int m_selected_treenode;
        private double m_songTotalDuration = 0D;
        private bool m_IsSearchingListView;

        /// <summary>
        /// 
        /// </summary>
        private double m_duration = 0d;
        private string m_MediaFileName;
        private int m_volumeCounter;
        private SYNCPROC _mixerStallSync;
        private Player m_track = null;
        private Player m_currentTrack = null;
        private Player m_previousTrack = null;
        private ASIOPROC _asioProc;
        private BASS_ASIO_CHANNELINFO chaninfo;
        private int chan = 0;

        public static FullScreenPlayer m_fullScreenPlayer = new FullScreenPlayer();
        private Preferences prefs = new Preferences();

        private Control thisControl;
        private Control playerWindow;
        private Control playerWindowFullScreen;
        Timer timerUpdate;
        Timer timerVolume;
        Timer timerVideo;

        HashSet<string> extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".cdg", ".mp4" };

        public void Initialize(Control form, Control playerWindow)
        {
            this.thisControl = form;
            // this.playerWindow = playerWindow;
            targetControl = playerWindow;
            InitTimer();
            InitAsio();
            InitVlc();
            InitGraphics(targetControl);
        }

        public void FormClosed()
        {
            this.playerWindow = null;
            timerUpdate.Stop();
            // close bass
            if (m_track != null)
                m_track.Dispose();

            if (m_currentTrack != null)
                m_currentTrack.Dispose();

            if (m_previousTrack != null)
                m_previousTrack.Dispose();

            Bass.BASS_Free();
        }

        public void ResizeScreen1()
        {
            gdx.ResizeScreen1();
        }

        public void ResizeScreen2()
        {
            gdx.ResizeScreen2();
        }

        private void InitAsio()
        {
          // Player.InitializeAsio(new ASIOPROC(AsioCallback));
          //  timerUpdate.Start();
            
        }

        /// <summary>
        /// Initialize Vlc for video palyback 
        /// </summary>
        private void InitVlc()
        {

            vlc = new VlcPlayer();
            vlc.Volume(PlayerControl.VolumeScroll.Value);
        }

        /// <summary>
        /// Initailize the draw utility and assign main menu handle
        /// </summary>
        /// <param name="target"></param>
        private void InitGraphics(Control target)
        {

            gdx = new GraphicUtilDX();

            gdx.Initialize(target, m_fullScreenPlayer.panelPlayer);

            timerVideo.Start();

        }

        private void InitTimer()
        {

            timerUpdate = new Timer();
            timerVolume = new Timer();
            timerVideo = new Timer();

            timerUpdate.Tick += TimerUpdate_Tick;
            timerVolume.Tick += TimerVolume_Tick;
            timerVideo.Tick += TimerVideo_Tick;

            timerUpdate.Enabled = false;
            timerUpdate.Interval = 50;
            timerVolume.Enabled = false;
            timerVolume.Interval = 100;
            timerVideo.Enabled = false;
            timerVideo.Interval = 50;
        }

        private void InitControls()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.BeginInvoke(new Action(() =>
                {
                    InitControls();
                }));
                return;
            }

            if (IsPlayingCdg)
            {
                PlayerControl.KeyTempoPanel.Enabled = true;
                PlayerControl.KeyValue.Text = m_currentTrack.Tempo.Key.ToString();
                PlayerControl.TempoValue.Text = m_currentTrack.Tempo.Tempo.ToString() + "%";
            }
            if (IsPlayingVideo)
            {
                PlayerControl.KeyTempoPanel.Enabled = false;
                PlayerControl.KeyValue.Text = "0";
                PlayerControl.TempoValue.Text = "0%";
            }

            if (!IsPlayingVideo && !IsPlayingCdg)
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

        private void TimerVideo_Tick(object sender, EventArgs e)
        {
            if (vlc.Video != null)
            {
                BgVideo = new Bitmap(vlc.Video);
                gdx.DoRender(BgVideo, null, m_songQueue.Count.ToString(), false);
                BgVideo.Dispose();
            }
        }


        private void TimerVolume_Tick(object sender, EventArgs e)
        {
            m_volumeCounter--;
            if (IsPlayingVideo)
                vlc.Volume(m_volumeCounter);

            if (m_volumeCounter <= 0)
            {
                timerVolume.Stop();
                vlc.Volume(PlayerControl.VolumeScroll.Value);
                playNextTrack();
            }
        }

        private void TimerUpdate_Tick(object sender, EventArgs e)
        {
            if (IsPlayingCdg && !IsPlayingVideo)
            {
                BgVideo = new Bitmap(vlc.Video);
                gdx.DoRender(BgVideo, (Bitmap)m_CDGFile.RGBImage, m_songQueue.Count.ToString(), true);
                BgVideo.Dispose();
              //  float level = BassAsio.BASS_ASIO_ChannelGetLevel(false, Player.Mixer);
              //  PlayerControl.LeftChannelLevel.Value = Un4seen.Bass.Utils.LowWord32((int)level);
              //  PlayerControl.RightChannelLevel.Value = Un4seen.Bass.Utils.HighWord32((int)level);

                if (m_currentTrack != null)
                {

                    long pos = BassMix.BASS_Mixer_ChannelGetPosition(m_currentTrack.Channel);
                    double time = Bass.BASS_ChannelBytes2Seconds(m_currentTrack.Channel, pos);
                    long renderAtPosition = Convert.ToInt64(time * 1000);

                    m_CDGFile.renderAtPosition(renderAtPosition);

                    PlayerControl.Time.Text = Un4seen.Bass.Utils.FixTimespan(time, "HHMMSS");
                    PlayerControl.TimeRemain.Text = Un4seen.Bass.Utils.FixTimespan(Bass.BASS_ChannelBytes2Seconds(m_currentTrack.Channel, m_currentTrack.TrackLength - pos), "HHMMSS");
                    DrawPalyerPosition(pos, 0, m_currentTrack.TrackLength);
                }
            }
            if (!IsPlayingCdg && IsPlayingVideo)
            {
                if (vlc.Time == "00:00:00")
                    PlayerControl.Time.Text = "00:00:00";
                else
                {
                    double time = Convert.ToDouble(vlc.Time);
                    double timeRemain = Convert.ToDouble(vlc.TimeDuration);
                    PlayerControl.Time.Text = TimeSpan.FromMilliseconds(time).ToString().Substring(0, 8);
                    PlayerControl.TimeRemain.Text = TimeSpan.FromMilliseconds(timeRemain - time).ToString().Substring(0, 8);

                    BgVideo = new Bitmap(vlc.Video);
                    gdx.DoRender(BgVideo, null, m_songQueue.Count.ToString(), true);
                    BgVideo.Dispose();
                    DrawPalyerPosition(0, vlc.PlayerPosition);
                }

                if (vlc.MediaEnded)
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

                        timerUpdate.Stop();

                        vlc.LoadDefaultVideoBackGround();
                        timerVideo.Start();

                        IsPlayingCdg = false;
                        IsPlayingVideo = false;

                        return;
                    }
                }

            }
            GetNextTrack();
        }

        private int AsioCallback(bool input, int channel, IntPtr buffer, int length, IntPtr user)
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
        /// <summary>
        /// 
        /// </summary>
        public void keyPlusbtn()
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

        /// <summary>
        /// 
        /// </summary>
        public void keyMinusBtn()
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

        /// <summary>
        /// 
        /// </summary>
        public void tempoPlusBtn()
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

        /// <summary>
        /// 
        /// </summary>
        public void tempoMinusBtn()
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

        /// <summary>
        /// 
        /// </summary>
        public void volumeBtn()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    volumeBtn();
                }));
                return;
            }

            if (!m_Mute)
            {
                m_Volume = PlayerControl.VolumeScroll.Value * 0.01f;

                if (m_currentTrack != null)
                    m_currentTrack.Mute();

                if (IsPlayingVideo)
                {
                    vlc.Mute();
                    vlc.Volume(PlayerControl.VolumeScroll.Value);
                }
                PlayerControl.VolumeButton.BackgroundImage = Resources.ic_volume_off_black_24dp_1x;
                m_Mute = true;

            }
            else
            {
                if (m_currentTrack != null)
                    m_currentTrack.Mute();

                if (IsPlayingVideo)
                    vlc.Mute();

                PlayerControl.VolumeButton.BackgroundImage = Resources.ic_volume_up_black_24dp_1x;
                m_Mute = false;
            }

        }

        public void RemoveVocalLeftOrRight(int removeVocal) {
            switch (removeVocal) {
                case 0: // Center no vocal removed
                    break;
                case 1: // Remove Right Vocal
                    break;
                case 2: // Remove Left Vocal 
                    break;
            }
        }


        /// <summary>
        ///
        /// </summary>
        public void volumeTrack()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    volumeTrack();
                }));
                return;
            }

            m_Volume = PlayerControl.VolumeScroll.Value * 0.01f;
            PlayerControl.VolumeLevel = PlayerControl.VolumeScroll.Value.ToString();

            if (m_currentTrack != null)
                m_currentTrack.Volume = m_Volume;

            if (IsPlayingVideo)
                vlc.Volume(PlayerControl.VolumeScroll.Value);

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

        public void VolumePlus()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    VolumePlus();
                }));
                return;
            }

            if (PlayerControl.VolumeScroll.Value != PlayerControl.VolumeScroll.Maximum)
                PlayerControl.VolumeScroll.Value += 1;
        }

        public void VolumeMinus()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    VolumeMinus();
                }));
                return;
            }

            if (PlayerControl.VolumeScroll.Value != PlayerControl.VolumeScroll.Minimum)
                PlayerControl.VolumeScroll.Value -= 1;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeview"></param>
        public void LoadTreviewMenu(TreeView treeview)
        {

            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    LoadTreviewMenu(treeview);
                }));
                return;
            }

            this.treeview = new TreeView();
            this.treeview = treeview;
            PlayerControl.SongListView.FullRowSelect = false;
            List<string> songs;
            int count = 0;

            foreach (var node in Collect(this.treeview.Nodes))
            {
                RootNode rootNode;
                if (Enum.TryParse(node.Name.ToUpper(), out rootNode))
                {
                    switch (rootNode)
                    {
                        case RootNode.SONG_QUEUE:
                            songs = new List<string>();
                            songs = Directory.EnumerateFiles(m_filePath, "*.que", SearchOption.AllDirectories).ToList();
                            if (songs.Count() > 0)
                            {
                                setSEARCHDIRorTEXTState(SearchAndLoad.SEARCH_TEXTFILE);
                                startWorker(songs[0]);
                                if (PlayerControl.AllSongs.Count() > 0)
                                {
                                    PlayerControl.AllSongs.Select(s =>
                                    {
                                        string MediaFileName = GetExtPatern(s.SubItems[4].Text);
                                        MediaExtension = Path.GetExtension(MediaFileName).ToLower();
                                        ListViewItem items = trackInfo(s.SubItems[4].Text, count++).toListViewItem();

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
                                            vlc.GetDuration(MediaFileName);
                                            items.SubItems[3].Text = GetVlcTimeOrDuration(Convert.ToDouble(vlc.GetTimeDuration)).ToString();
                                        }

                                        m_songItems = new ListViewItem();
                                        m_songItems = items;
                                        addToQueue();

                                        return items;

                                    }).ToList();

                                }
                            }
                            break;
                        case RootNode.MY_COMPUTER:
                            songs = new List<string>();
                            songs = Directory.EnumerateFiles(m_filePath, "*.bkN", SearchOption.AllDirectories).OrderByDescending(file => new FileInfo(file).CreationTime).ToList();
                            if (songs.Count() > 0)
                            {
                                setSEARCHDIRorTEXTState(SearchAndLoad.SEARCH_TEXTFILE);
                                for (int i = 0; i < songs.Count; i++)
                                {
                                    string folderName = Path.GetFileName(songs[i]).Replace(".bkN", "");
                                    node.Nodes.Insert(i, folderName, folderName);
                                    startWorker(songs[i]);
                                    songsArr.Add(PlayerControl.AllSongs);
                                }

                                this.treeview.SelectedNode = node.FirstNode;
                                PlayerControl.SongListView.VirtualListSize = songsArr[node.FirstNode.Index].Count;
                            }
                            break;
                    }
                }
            }

            PlayerControl.SongListView.FullRowSelect = true;
            PlayerControl.SongListView.Refresh();
            m_IsSearchingListView = true;

            /*songs = Directory.EnumerateFiles(m_filePath, "*.bkN", SearchOption.AllDirectories).OrderByDescending(file => new FileInfo(file).CreationTime).ToList();
            if (songs.Count() > 0)
            {
                TreeNode rootNode = treeview.Nodes.Cast<TreeNode>().ToList().Find(n => n.Name.Equals("My_Computer"));
                setSEARCHDIRorTEXTState(SearchAndLoad.SEARCH_TEXTFILE);
                for (int i = 0; i < songs.Count; i++)
                {
                    string folderName = Path.GetFileName(songs[i]).Replace(".bkN", "");
                    rootNode.Nodes.Insert(i, folderName, folderName);
                    startWorker(songs[i]);
                    songsArr.Add(PlayerControl.AllSongs);
                }

                PlayerControl.SongListView.FullRowSelect = true;
                this.treeview.SelectedNode = rootNode.FirstNode;
                PlayerControl.SongListView.VirtualListSize = songsArr[rootNode.FirstNode.Index].Count;
                PlayerControl.SongListView.Refresh();
                m_IsSearchingListView = true;
            }*/
        }

        public void SetAsNext()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    SetAsNext();
                }));
                return;
            }

            if (m_songQueue.Count > 1)
            {
                this.thisControl.Execute(delegate
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

        public void RemoveFileFromTreeView()
        {

            string sDir = m_filePath + this.treeview.SelectedNode.Text + ".bkN";
            if (File.Exists(sDir))
            {
                File.Delete(sDir);
                treeview.SelectedNode.Remove();
                songsArr.Remove(songsArr[m_selected_treenode]);
                PlayerControl.SongListView.VirtualListSize = 0;
                PlayerControl.SongListView.Refresh();
                this.treeview.SelectedNode = this.treeview.SelectedNode.Parent;

            }
        }

        public void RemoveSongFromQueue()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    RemoveSongFromQueue();
                }));
                return;
            }

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

                WriteToQueueList();
            }
        }

        public void EmptyQueue()
        {
            if (m_songQueue.Count > 0)
            {
                m_songQueue.RemoveRange(0, m_songQueue.Count);
                PlayerControl.SongListView.VirtualListSize = 0;
                PlayerControl.SongListView.Refresh();
                m_songTotalDuration = 0;
                m_duration = 0;
                updateSongQueueDuration("minus");

                WriteToQueueList();
            }
        }

        public void ShuffleSongs()
        {
            setSEARCHDIRorTEXTState(SearchAndLoad.SHUFFLE_SONGS);
            startWorker();
        }

        /// <summary>
        /// Not in use at the moment
        /// </summary>
        public void AddSelectedSongToQueue()
        {
            setSEARCHDIRorTEXTState(SearchAndLoad.ADD_SELECTED_SONG_TO_QUEUE);
            startWorker();
        }

        /// <summary>
        /// 
        /// </summary>
        public void on_SongListView_Item_Selected()
        {

            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    on_SongListView_Item_Selected();
                }));
                return;
            }

            ListView.SelectedIndexCollection col = PlayerControl.SongListView.SelectedIndices;

            foreach (int i in col)
            {
                TrackInfo trackInfo = null;
                string filePath = PlayerControl.SongListView.Items[i].SubItems[4].Text.ToString();
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
                        vlc.GetDuration(MediaFileName);
                        trackInfo.Duration = GetVlcTimeOrDuration(Convert.ToDouble(vlc.GetTimeDuration)).ToString();
                    }


                    MediaExtension = Path.GetExtension(MediaFileName).ToLower();
                    trackInfo.ID = PlayerControl.SongListView.Items[i].SubItems[0].Text.ToString();
                    trackInfo.Name = PlayerControl.SongListView.Items[i].SubItems[1].Text.ToString();
                    trackInfo.Artist = PlayerControl.SongListView.Items[i].SubItems[2].Text.ToString();
                    trackInfo.FilePath = filePath;

                    m_songItems = new ListViewItem();
                    m_songItems = trackInfo.toListViewItem();
                    addToQueue();
                    WriteToQueueList();
                }
                else
                {
                    MessageBox.Show("Cannot find " + Path.GetFileName(MediaFileName) + " file to play.");
                    return;

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public void treeView1_AfterSelect(TreeViewEventArgs e)
        {

            RootNode rootNode;
            if (Enum.TryParse(e.Node.Name.ToUpper(), out rootNode))
            {

                switch (rootNode)
                {
                    case RootNode.SONG_QUEUE:
                        setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_QUEUE_SONGS);
                        startWorker();
                        break;
                    case RootNode.MY_COMPUTER:

                        break;
                    case RootNode.ADD_FOLDER:
                        m_fbd = new FolderBrowserDialog();
                        setSEARCHDIRorTEXTState(SearchAndLoad.SEARCH_DIRECTORY);
                        if (m_fbd.ShowDialog() == DialogResult.OK)
                        {
                            startWorker(m_fbd.SelectedPath);
                            string[] filePath = new string[] { m_fbd.SelectedPath };
                            string folderName = Path.GetFileName(m_fbd.SelectedPath);
                            this.treeview.SelectedNode.Parent.Nodes.Insert(0, folderName, folderName); //(this.treeview.SelectedNode.Index - 1, folderName);

                            if (this.treeview.SelectedNode.Name.ToUpper() == RootNode.ADD_FOLDER.ToString())
                            {
                                this.treeview.SelectedNode = this.treeview.SelectedNode.Parent;
                            }
                        }
                        break;
                }
            }

            if (e.Node.Parent != null && e.Node.Parent.GetType() == typeof(TreeNode))
            {
                if (this.treeview.SelectedNode.Level != 0 && this.treeview.SelectedNode.Name.ToUpper() != RootNode.ADD_FOLDER.ToString())
                {
                    m_selected_treenode = this.treeview.SelectedNode.Index;
                    setSEARCHDIRorTEXTState(SearchAndLoad.LOAD_ADDED_SONGS);

                    string sDir = m_filePath + this.treeview.SelectedNode.Text + ".bkN";
                    string path = File.ReadLines(sDir).First();
                    this.thisControl.ExecuteAsync(delegate
                    {
                        if (!Directory.Exists(path))
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

        /// <summary>
        /// 
        /// </summary>
        public void ShowCDGWindow()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    ShowCDGWindow();
                }));
                return;
            }

            if (m_fullScreenPlayer == null || m_fullScreenPlayer.IsDisposed)
            {
                m_fullScreenPlayer = new FullScreenPlayer();
                gdx.InitScreen2(m_fullScreenPlayer.panelPlayer);
            }
            m_fullScreenPlayer.Show();
            m_fullScreenPlayer.Activate();

        }

        /// <summary>
        /// 
        /// </summary>
        public void HideCDGWindow()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    m_fullScreenPlayer.Hide();
                }));
                return;
            }
            else
                m_fullScreenPlayer.Hide();

        }

        /// <summary>
        /// 
        /// </summary>
        public void LoadPreferecesForm()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    LoadPreferecesForm();
                }));
                return;
            }

            if (prefs == null || prefs.IsDisposed)
            {
                prefs = new Preferences();
            }

            PlayerControl.PrefsForm = prefs;

            prefs.Show();
            prefs.Activate();

        }

        /// <summary>
        /// 
        /// </summary>
        public void PlayPause()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    PlayPause();
                }));
                return;
            }

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

            if (IsPlayingVideo)
            {
                m_Paused = vlc.PlayPause();
            }

            if (m_Paused)
                PlayerControl.PlayPauseButton.BackgroundImage = Resources.ic_play_arrow_black_36dp_1x;
            else
                PlayerControl.PlayPauseButton.BackgroundImage = Resources.ic_pause_black_36dp_1x;

        }


        // Private functions ===============================
        /// <summary>
        /// 
        /// </summary>
        private void WriteToQueueList()
        {
            string file = m_filePath + "SonQueueList.que";

            var items = m_songQueue.OfType<ListViewItem>().Select(i => i.SubItems[4].Text).ToList();
            string[] filaPathArray = items.ToArray<string>();
            Directory.CreateDirectory(m_filePath);

            if (File.Exists(file))
                File.WriteAllLines(m_filePath + "SonQueueList.que", filaPathArray);
            // else
            //     File.AppendAllText(file, Environment.NewLine + filePath);

        }

        /// <summary>
        /// 
        /// </summary>
        private void addToQueue()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.BeginInvoke(new Action(() =>
                {
                    addToQueue();
                }));
                return;
            }

            lock (m_songQueue)
            {
                if (PlayerControl.SetAsNextSong && m_songQueue.Count > 1)
                {
                    m_songQueue.Insert(0, m_songItems);
                    updateSongQueueDuration("add");
                    PlayerControl.SetAsNextSong = false;
                }
                else
                {
                    m_songQueue.Add(m_songItems);
                    updateSongQueueDuration("add");
                }

            }

            if (m_currentTrack == null && MediaExtension == ".mp3" && !IsPlayingVideo && !IsPlayingCdg)
            {
                IsPlayingCdg = true;
                IsPlayingVideo = false;
                addToBassMixer();
                playNextTrack();
            }
            else if (m_currentTrack == null && MediaExtension != ".mp3" && !IsPlayingCdg && !IsPlayingVideo)
            {

                IsPlayingVideo = true;
                IsPlayingCdg = false;
                SetMediaInfo();
                playNextTrack();
            }

            if (timerVideo.Enabled) timerVideo.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plusMinusduration"></param>
        private void updateSongQueueDuration(string plusMinusduration)
        {

            string totalDuration = "";
            int count = m_songQueue.Count;

            switch (plusMinusduration.ToLower())
            {
                case "add":
                    if (MediaExtension != ".mp3")
                        m_duration = GetVlcTimeOrDuration(Convert.ToDouble(vlc.GetTimeDuration));
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
                treeview.Nodes[0].Text = "Song Queue (Empty)";
            }
            else
            {
                if (count < 1)
                    treeview.Nodes[0].Text = "Song Queue (Empty)";
                if (count == 1)
                    treeview.Nodes[0].Text = "Song Queue (" + count + " Song - " + totalDuration + ")";
                else
                    treeview.Nodes[0].Text = "Song Queue (" + count + " Songs - " + totalDuration + ")";
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void addToBassMixer()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    addToBassMixer();
                }));
                return;
            }

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


        /// <summary>
        /// 
        /// </summary>
        private void AddToReserveNotification()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.BeginInvoke(new Action(() =>
                {
                    AddToReserveNotification();
                }));
                return;
            }



        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        private void GetNextTrack()
        {

            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.BeginInvoke(new Action(() =>
                {
                    GetNextTrack();
                }));
                return;
            }

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

        /// <summary>
        /// 
        /// </summary>
        public void PlayNext()
        {

            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.BeginInvoke(new Action(() =>
                {
                    PlayNext();
                }));
                return;
            }

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

        /// <summary>
        /// 
        /// </summary>
        private void SetMediaInfo()
        {
            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.BeginInvoke(new Action(() =>
                {
                    SetMediaInfo();
                }));
                return;
            }

            if (m_songQueue.Count > 0)
            {
                PlayerControl.SongTitle.Text = m_songQueue[0].SubItems[1].Text;
                PlayerControl.SongArtist.Text = m_songQueue[0].SubItems[2].Text;
                string duration = m_songQueue[0].SubItems[3].Text;
                m_MediaFileName = m_songQueue[0].SubItems[4].Text;
                if (duration == "")
                {
                    vlc.GetDuration(m_MediaFileName);
                    m_duration = Convert.ToDouble(vlc.TimeDuration);
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

        /// <summary>
        /// 
        /// </summary>
        private void playNextTrack()
        {

            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.BeginInvoke(new Action(() =>
                {
                    playNextTrack();
                }));
                return;
            }
            try
            {
                lock (m_songQueue)
                {
                    if (m_songQueue.Count > 0)
                    {
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

                            m_songQueue.RemoveAt(0);

                            IsPlayingCdg = vlc.PlayCDG();
                            IsPlayingVideo = false;

                            InitControls();
                            m_track.Volume = m_Volume;

                            m_track.Play();

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
                            if (timerVideo.Enabled)
                            {
                                timerVideo.Stop();
                            }

                            IsPlayingVideo = vlc.PlayVideo(m_MediaFileName);
                            vlc.Volume(PlayerControl.VolumeScroll.Value);
                            InitControls();
                            if (!timerUpdate.Enabled) timerUpdate.Start();
                            IsPlayingCdg = false;
                        }

                        WriteToQueueList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("playNextTrack: " + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void VolumeSlideAttribute()
        {

            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.BeginInvoke(new Action(() =>
                {
                    VolumeSlideAttribute();
                }));
                return;
            }

            int interval = 2000 / PlayerControl.VolumeScroll.Value;
            m_volumeCounter = PlayerControl.VolumeScroll.Value;
            timerVolume.Interval = interval;
            timerVolume.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeOrDuration"></param>
        /// <returns></returns>
        private double GetVlcTimeOrDuration(double timeOrDuration)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(timeOrDuration);
            return t.TotalSeconds;
        }

        /// <summary>
        /// 
        /// </summary>
        private void PreProcessFiles()
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

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        /// <returns></returns>
        private List<ListViewItem> LoadQueueSongs()
        {
            PlayerControl.AllSongs = new List<ListViewItem>();

            PlayerControl.AllSongs.AddRange(m_songQueue);

            return PlayerControl.AllSongs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private string GetExtPatern(string fileName)
        {
            string extPattern = ".cdg$|.CDG$";
            Regex regX = new Regex(extPattern);
            string MediaFileName = regX.Replace(fileName, ".mp3");

            return MediaFileName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        private List<ListViewItem> SearchListView(string searchString)
        {
            PlayerControl.AllSongs = new List<ListViewItem>();
            if (searchString != "")
                PlayerControl.AllSongs.AddRange(songsArr[m_selected_treenode].Where(s => s.SubItems[1].Text.ToLower().Contains(searchString.ToLower()) || s.SubItems[2].Text.ToLower().Contains(searchString.ToLower())).ToList());
            else
                PlayerControl.AllSongs.AddRange(songsArr[m_selected_treenode]);

            return PlayerControl.AllSongs;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="sDir"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        private List<ListViewItem> TextSearchSongs(string sDir)
        {
            int count = 0;

            PlayerControl.AllSongs = new List<ListViewItem>();
            //extensions.Contains(Path.GetExtension(s))

            PlayerControl.AllSongs.AddRange(File.ReadAllLines(sDir).Where(s => extensions.Contains(Path.GetExtension(s))) // s.EndsWith(".cdg") || s.EndsWith(".CDG") || s.EndsWith(".mp4") || s.EndsWith(".MP4"))
                .Select(s =>
                {
                    return trackInfo(s, count++).toListViewItem();

                }).ToList());

            return PlayerControl.AllSongs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sDir"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        private List<ListViewItem> DirSearchSongs(string sDir)
        {
            int count = 0;
            PlayerControl.AllSongs = new List<ListViewItem>();
            PlayerControl.AllSongs = Directory.EnumerateFiles(sDir, "*.*", SearchOption.AllDirectories)
              .Where(s => extensions.Contains(Path.GetExtension(s))).Select(s =>
              {

                  return trackInfo(s, count++).toListViewItem();

              }).ToList();

            songsArr.Insert(0, PlayerControl.AllSongs);

            return PlayerControl.AllSongs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private List<ListViewItem> Shuffle()
        {

            if (this.thisControl.InvokeRequired)
            {
                this.thisControl.Invoke(new Action(() =>
                {
                    Shuffle();

                }));
                return PlayerControl.AllSongs;
            }


            string nodeName = "";
            Random rnd = new Random();
            PlayerControl.AllSongs = new List<ListViewItem>();

            nodeName = this.treeview.SelectedNode.Name;
            foreach (var node in Collect(this.treeview.Nodes))
            {
                if (nodeName == node.Name)
                {
                    if (nodeName == node.Text)
                    {//its ceated dynamically
                        if (songsArr[m_selected_treenode].Count > 0)
                        {
                            songsArr[m_selected_treenode] = new List<ListViewItem>(songsArr[m_selected_treenode].OrderBy(x => rnd.Next()).ToList());
                            PlayerControl.AllSongs.AddRange(songsArr[m_selected_treenode]);
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


            return PlayerControl.AllSongs;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arg"></param>
        public void startWorker(object arg = null)
        {
            BackgroundWorker bgw = new BackgroundWorker();
            bgw.DoWork += DoWork;
            bgw.RunWorkerCompleted += Workers_Completed;
            bgws.Add(bgw);
            bgw.RunWorkerAsync(arg);

            while (bgw.IsBusy)
            {
                Application.DoEvents();
                if (bgw.IsBusy)
                    treeview.Enabled = false;
                else
                    treeview.Enabled = true;
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
                case SearchAndLoad.SEARCH_LISTVIEW:
                    e.Result = SearchListView(fName);
                    break;
                case SearchAndLoad.SHUFFLE_SONGS:
                    e.Result = Shuffle();
                    break;
                case SearchAndLoad.LOAD_ADDED_SONGS:
                    e.Result = songsArr[m_selected_treenode];
                    break;
                case SearchAndLoad.ADD_SELECTED_SONG_TO_QUEUE:
                    // on_SongListView_Item_Selected();
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
            BackgroundWorker bgw = (BackgroundWorker)sender;

            PlayerControl.AllSongs = new List<ListViewItem>();
            PlayerControl.AllSongs = (List<ListViewItem>)e.Result;

            if (getSEARCHDIRorTEXTState == SearchAndLoad.SEARCH_DIRECTORY)
            {
                string filename = Path.GetFileName(m_fbd.SelectedPath);
                var items = PlayerControl.AllSongs.OfType<ListViewItem>().Select(i => i.SubItems[4].Text).ToList();
                items.Insert(0, m_fbd.SelectedPath);
                string[] filaPathArray = items.ToArray<string>();
                Directory.CreateDirectory(m_filePath);
                File.WriteAllLines(m_filePath + filename + ".bkN", filaPathArray);
            }

            bgws.Remove(bgw);
            bgw.Dispose();

            if (bgws.Count == 0 && m_IsSearchingListView)
            {
                PlayerControl.SongListView.VirtualListSize = PlayerControl.AllSongs.Count;
                PlayerControl.SongListView.Refresh();
            }
            // MessageBox.Show("All workers complete");

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
            string extPattern = ".cdg|.CDG|.mp4|.MP4";
            string fName = Path.GetFileName(file);
            string[] regXpattern = Regex.Split(fName, pattern);
            // var containsSwears = extensions.Any(w => file.Contains(w));

            Regex regX = new Regex(extPattern);

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
            this.thisControl.ExecuteAsync(delegate
            {
                SetMediaInfo();
            });

            if (m_songQueue.Count <= 0)
                m_currentTrack.NextTrackSync = 0;
            else
                m_currentTrack.NextTrackSync = 1;

            user = new IntPtr(m_currentTrack.NextTrackSync);

            if (user.ToInt32() == 0)
            {
                // END SYNC
                this.thisControl.ExecuteAsync(playNextTrack);
                IsPlayingCdg = false;
                IsPlayingVideo = false;
            }
            else
            {
                // POS SYNC
                this.thisControl.ExecuteAsync(delegate
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <param name="user"></param>
        private void OnMixerStall(int handle, int channel, int data, IntPtr user)
        {
            this.thisControl.ExecuteAsync(delegate
            {
                // this code runs on the UI thread!
                if (data == 0)
                {
                    // mixer stalled
                    // 
                    if (!IsPlayingCdg && !IsPlayingVideo)
                    {
                        timerUpdate.Stop();
                        timerVideo.Start();

                        m_track = null;
                        m_currentTrack = null;
                        m_previousTrack = null;
                    }

                    // InitControls();
                }
                else
                {
                    // mixer resumed
                    timerVideo.Stop();
                    timerUpdate.Start();
                }
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="posF"></param>
        /// <param name="len"></param>
        private void DrawPalyerPosition(long pos, float posF, long len = 0)
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


        //Temporary Solution for Android kfn remote app so we can still use kfn remote on karafun

        public string GetNowPlaying()
        {
            string nowPlaying = PlayerControl.SongTitle.Text.PadRight(1) + PlayerControl.SongArtist.Text;

            return nowPlaying = string.IsNullOrEmpty(nowPlaying) ? "" : nowPlaying;

        }

        public void AddSongToQueue(string songId)
        {

            TrackInfo trackInfo = null;
            m_songItems = new ListViewItem();

            m_songItems = songsArr[m_selected_treenode][Convert.ToInt32(songId) - 1];

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
                    vlc.GetDuration(MediaFileName);
                    trackInfo.Duration = GetVlcTimeOrDuration(Convert.ToDouble(vlc.GetTimeDuration)).ToString();
                }


                MediaExtension = Path.GetExtension(MediaFileName).ToLower();
                trackInfo.ID = m_songItems.SubItems[0].Text.ToString();
                trackInfo.Name = m_songItems.SubItems[1].Text.ToString();
                trackInfo.Artist = m_songItems.SubItems[2].Text.ToString();
                trackInfo.FilePath = filePath;

                m_songItems = new ListViewItem();
                m_songItems = trackInfo.toListViewItem();
                addToQueue();
            }
            else
            {
                MessageBox.Show("Cannot find " + Path.GetFileName(MediaFileName) + " file to play.");
                return;

            }

        }
        private string MediaExtension { get; set; }
    }
}
