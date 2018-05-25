using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Enums;
using bossdoyKaraoke_NOW.Models;
using bossdoyKaraoke_NOW.Models.VocalEffects;
using bossdoyKaraoke_NOW.Nlog;
using static bossdoyKaraoke_NOW.Enums.DefaultAudio;
using static bossdoyKaraoke_NOW.Enums.Songs;
using static bossdoyKaraoke_NOW.Enums.TreviewNode;

namespace bossdoyKaraoke_NOW
{
    public partial class MainForm : Form
    {
        ToolTip m_tooltip = new ToolTip();
        private bool m_Loaded = false;
        private bool m_isPlayerMaximized;    

        public MainForm()
        {
            InitializeComponent();
            Splasher.Status = "Loading components...";
            System.Threading.Thread.Sleep(2000);
            PreLoad();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Restore Previous Settings, ie, Go To Sleep Again
            SystemState.RestoreDisplaySettings();

            PlayerControl.Dispose();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            Form form = sender as Form;
            form.Hide();
       
           // Splasher.Show();
           // Splasher.Status = "Closing BossDoy_Karaoke_Now...";
           // System.Threading.Thread.Sleep(1000);
            Channel1Fx.UpdateSettings();
           // Splasher.Close();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Activate();

            if (!m_Loaded)
                throw new InvalidOperationException("Must call PreLoad before calling this function.");
        }

        public void PreLoad()
        {
            //	if this function was already called, simply return
            if (m_Loaded)
                return;

            AppSettings.Initialize();

            //Prevent system sleep
            SystemState.KeepDisplayActive();

            PlayerControl.DefaultAudioOutput = DefaultAudioOutput.Bass;
            PlayerControl.MainFormControl = this;
            PlayerControl.VolumeScroll = volumeTrack;
            PlayerControl.MicVolumeScroll = micVolumeTrack;

            PlayerControl.Init(panelPlayer);
            PlayerControl.splitContainer = splitContainer1;
            PlayerControl.SongListView = songListView;
            PlayerControl.PlayProgress = pictureBoxWaveForm;
            PlayerControl.LeftChannelLevelDb = picBoxLeftChannel;
            PlayerControl.RightChannelLevelDb = picBoxRightChannel;
            PlayerControl.Time = labelTime;
            PlayerControl.TimeRemain = labelRemain;
            PlayerControl.KeyValue = keyValueLbl;
            PlayerControl.TempoValue = tempoValueLbl;
            PlayerControl.SongTitle = songTitleLbl;
            PlayerControl.SongArtist = songArtistLbl;
            PlayerControl.KeyTempoPanel = panelKeyTempo;
            PlayerControl.PlayPauseButton = playPauseBtn;
            PlayerControl.RemoveVocal = removeVocalBtn;
            PlayerControl.NextButton = nextBtn;
            PlayerControl.VolumeButton = volumeBtn;

            m_tooltip.SetToolTip(playPauseBtn, "Play/Pause");
            m_tooltip.SetToolTip(nextBtn, "Play Next");
            m_tooltip.SetToolTip(dualScreenBtn, "Full screen view");
            m_tooltip.SetToolTip(volumeBtn, "Audio settings");
            m_tooltip.SetToolTip(MicSettings, "Mic settings");
            m_tooltip.SetToolTip(removeVocalBtn, "Remove left/right vocal");

            m_Loaded = true;
            Splasher.Close();

        }

        private void playPauseBtn_Click(object sender, EventArgs e)
        {
            PlayerControl.PlayPause();
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            PlayerControl.PlayNext();
        }

        private void dualScreenBtn_Click(object sender, EventArgs e)
        {
            if (!m_isPlayerMaximized)
            {
                PlayerControl.ShowFullScreen();
                m_isPlayerMaximized = true;
            }
            else
                MainForm_SizeChanged(sender, e);

        }

        private void volumeBtn_Click(object sender, EventArgs e)
        {
            PlayerControl.VolumeControl();
        }

        private void keyPlusbtn_Click(object sender, EventArgs e)
        {
            PlayerControl.KeyPlus();
        }

        private void keyMinusBtn_Click(object sender, EventArgs e)
        {
            PlayerControl.KeyMinus();
        }

        private void tempoPlusBtn_Click(object sender, EventArgs e)
        {
            PlayerControl.TempoPlus();
        }

        private void tempoMinusBtn_Click(object sender, EventArgs e)
        {
            PlayerControl.TempoMinus();
        }

        private void volumeBtn_MouseHover(object sender, EventArgs e)
        {
            int Y = volumePnl.Location.Y + (panelFuntions.Height - 1);
            int X = volumePnl.Location.X + (picBoxLeftChannel.Width + 2); //15;
            Point loc = new Point(X, Y);


            volumePnlPopUp.Parent = MainForm.ActiveForm;
            volumePnlPopUp.Location = loc;
            volumePnlPopUp.BringToFront();
            volumePnlPopUp.Show();
        }

        private void volumePnl_MouseMove(object sender, MouseEventArgs e)
        {
            volumePnlPopUp.Hide();
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            volumePnlPopUp.Hide();
        }

        private void volumeTrack_Scroll(object sender, EventArgs e)
        {
           // PlayerControl.Volume = volumeTrack.Value;
        }

        private void volumeTrack_ValueChanged(object sender, EventArgs e)
        {
            PlayerControl.VolumeTrack();
            volumeLevelLbl.Text = volumeTrack.Value.ToString();
        }

        private void micVolumeTrack_ValueChanged(object sender, EventArgs e)
        {
            PlayerControl.MicVolumeTrack();
            micVolumeLevelLbl.Text = micVolumeTrack.Value.ToString();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            PlayerControl.TreeView_AfterSelect(e);
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (this.treeView1.SelectedNode.Level != 0 && this.treeView1.SelectedNode.Parent.Name.ToUpper() == RootNode.MY_FAVORITES.ToString() && this.treeView1.SelectedNode.Name.ToUpper() != RootNode.ADD_FAVORITES.ToString())
            {
                this.treeView1.LabelEdit = true;
                this.treeView1.SelectedNode.Parent.Nodes[e.Node.Index].BeginEdit();
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            setSEARCHDIRorTEXTState(SearchAndLoad.SEARCH_LISTVIEW);
            PlayerControl.TextBoxSearch(textBoxSearch.Text);
        }

        private void songListView_DoubleClick(object sender, EventArgs e)
        {
           PlayerControl.On_SongListView_Item_Selected();
        }

        private void songListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {
                if (e.ItemIndex >= 0 && e.ItemIndex < PlayerControl.AllSongs.Count)
                {
                    e.Item = PlayerControl.AllSongs[e.ItemIndex];
                }
                else
                {
                    //A cache miss, so create a new ListViewItem and pass it back. 
                    e.Item = new ListViewItem();
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "songListView_RetrieveVirtualItem", ex.LineNumber(), this.Name);

            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            PlayerControl.ResizeScreen1();
        }

        private void songListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                var hitTestInfo = songListView.HitTest(e.X, e.Y);
                if (hitTestInfo.Item != null && treeView1.SelectedNode.Level == 1 && treeView1.SelectedNode.Parent.Name.ToUpper() == RootNode.MY_COMPUTER.ToString())
                {
                    var loc = e.Location;
                    loc.Offset(songListView.Location);

                    songListViewMenuStrip.Show(songListView, loc);
                }
                if (hitTestInfo.Item != null && treeView1.SelectedNode.Index == 0 && treeView1.SelectedNode.Level == 0 ) {
                    var loc = e.Location;
                    loc.Offset(songListView.Location);

                    songQueueMenuStrip.Show(songListView, loc);
                }
            }
        }

        private void addToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.SetAsNextSong = false;
            PlayerControl.On_SongListView_Item_Selected();
        }

        private void addToQueueAsNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.SetAsNextSong = true;
            PlayerControl.On_SongListView_Item_Selected();
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hitTestInfo = treeView1.HitTest(e.X, e.Y);
                if (hitTestInfo.Node != null)
                {
                    string s = hitTestInfo.Node.Name;
                    if ( hitTestInfo.Node.Name.ToUpper() != RootNode.ADD_FOLDER.ToString() && hitTestInfo.Node.Name.ToUpper() != RootNode.ADD_FAVORITES.ToString())
                    {
                        var loc = e.Location;
                        loc.Offset(treeView1.Location);

                        if (hitTestInfo.Node.Index == 0 && hitTestInfo.Node.Level == 0)
                        {
                            treeViewMenuStrip.Items[0].Enabled = true;
                            treeViewMenuStrip.Items[2].Enabled = false;
                            treeViewMenuStrip.Items[3].Enabled = false;
                            treeViewMenuStrip.Items[4].Enabled = false;
                            treeViewMenuStrip.Show(treeView1, loc);
                        }
                        else
                        {
                            treeViewMenuStrip.Items[0].Enabled = false;
                            treeViewMenuStrip.Items[2].Enabled = true;
                            treeViewMenuStrip.Items[3].Enabled = true;
                            treeViewMenuStrip.Items[4].Enabled = true;

                            if (hitTestInfo.Node.Level != 0)
                                treeViewMenuStrip.Show(treeView1, loc);
                        } 
                    }
                }
            }
        }

        private void shuffleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.ShuffleSongs();
        }

        private void setAsNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.SetAsNext();
        }

        private void removeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PlayerControl.RemoveSongFromQueue();
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.On_SongListView_Item_Selected();
        }

        private void playToolStripMenuItem1_Click(object sender, EventArgs e)
        {
           
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            PlayerControl.LoadTreviewMenu(treeView1);   
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.RemoveFileFromTreeView();
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) treeView1.SelectedNode = e.Node;

            this.treeView1.LabelEdit = false;
        }

        private void emptyQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.EmptyQueue();
        }


        private void MicSettings_Click(object sender, EventArgs e)
        {
            PlayerControl.LoadPreferecesForm();
        }

        private void songListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
          //  PlayerControl.SortSongs(e.Column);
        }

        private void addAllPlayedSongsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.AddAllPlayedSongsToFavorites(); 
        }

        private void addAllToSongsToQueueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.AddFavoritesSongsToQueue();
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            PlayerControl.RenameFavoriteFile(e);
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void preferencesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PlayerControl.LoadPreferecesForm();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm.ActiveForm.Close();
        }

        private void playToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            PlayerControl.PlayPause();
        }

        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.PlayNext();
        }

        private void volumeUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.VolumePlus();
        }

        private void volumeDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.VolumeMinus();
        }

        private void muteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.VolumeControl();
        }

        private void keyUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.KeyPlus();
        }

        private void keyDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.KeyMinus();
        }

        private void tempoUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.TempoPlus();
        }

        private void tempoDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayerControl.TempoMinus();
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.treeView1.SelectedNode = this.treeView1.Nodes["My_Computer"].Nodes["Add_Folder"];
        }

        private void removeVocalBtn_Click(object sender, EventArgs e)
        {
            PlayerControl.RemoveVocalLeftOrRight();
        }

        private void picBoxRightChannel_MouseMove(object sender, MouseEventArgs e)
        {
            volumePnlPopUp.Hide();
        }

        private void picBoxLeftChannel_MouseMove(object sender, MouseEventArgs e)
        {
            volumePnlPopUp.Hide();
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            if (m_isPlayerMaximized)
            {
                PlayerControl.ShowFullScreen();
                m_isPlayerMaximized = false;
            }
        }

        /*private void songListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var loc = e.Location;
                //loc.Offset(songListView.Location);
                loc.X = songListView.Location.X + 5;
                pictureBoxNotification.Location = loc;
                //pictureBoxNotification.BackColor = Color.Transparent;
                PlayerControl.On_SongListView_Item_Selected();
            }
        }*/

    }
}