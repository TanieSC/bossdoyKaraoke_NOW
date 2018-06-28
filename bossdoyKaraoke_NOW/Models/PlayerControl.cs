using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Enums;
using bossdoyKaraoke_NOW.Models.VocalEffects;
using static bossdoyKaraoke_NOW.Enums.DefaultAudio;
using static bossdoyKaraoke_NOW.Enums.RemoveVocal;

namespace bossdoyKaraoke_NOW.Models
{
    public class PlayerControl
    {
        private static Main_Form main_form;

        public static void Init(Control playerWindow)
        {
            main_form = new Main_Form();
            main_form.Initialize(playerWindow);
        }

        public static void UpdateEQPresets(int preset)
        {
            main_form.UpdateEQPresets(preset);
        }

        public static void UpdateEQ(int band, float gain)
        {
            main_form.UpdateEQ(band, gain);
        }

        public static void UpdateEQPreamp(float gain)
        {
            main_form.UpdateEQPreamp(gain);
        }

        public static void TreeView_AfterSelect(TreeViewEventArgs e)
        {
            main_form.treeView1_AfterSelect(e);
        }

        public static void RenameFavoriteFile(NodeLabelEditEventArgs e)
        {
            main_form.RenameFavoriteFile(e);
        }

        public static void AddAllPlayedSongsToFavorites() {
            main_form.AddAllPlayedSongsToFavorite();
        }

        public static void AddFavoritesSongsToQueue()
        {
            main_form.AddFavoritesSongsToQueue();
        }

        public static void LoadTreviewMenu(TreeView treeView)
        {
            main_form.LoadTreviewMenu(treeView);
        }

        public static void LoadSongsQueue(TreeView treeView)
        {
           // main_form.LoadSongsQueue(treeView);
        }

        public static List<List<ListViewItem>> UpdateSongsDatabase()
        {
            return main_form.UpdateSongsDatabase();
        }

        public static void TextBoxSearch(string textToSearch)
        {
            main_form.startWorker(textToSearch);
        }

        public static void ShuffleSongs()
        {
            main_form.ShuffleSongs();
        }

        public static void SortSongs(int column)
        {
            main_form.SortSongs(column);
        }

        public static void PlayPause()
        {
            main_form.PlayPause();
        }

        public static void PlayNext()
        {
            main_form.PlayNext();
        }

        public static void SetAsNext()
        {
            main_form.SetAsNext();
        }

        public static void RemoveSongFromQueue()
        {
            main_form.RemoveSongFromQueue();
        }

        public static void RemoveFileFromTreeView()
        {
            main_form.RemoveFileFromTreeView();
        }

        public static void EmptyQueue()
        {
            main_form.EmptyQueue();
        }

        public static void VolumeControl()
        {
            main_form.volumeBtn();
        }

        public static void VolumeTrack()
        {
            main_form.volumeTrack();
        }

        public static void MicVolumeTrack()
        {
            Player.MicVolumeTrack();
        }

        public static void VlcAsioChannelVolume(bool mute)
        {
           
        }

        public static void VolumePlus()
        {
            main_form.VolumePlus();
        }

        public static void VolumeMinus()
        {
            main_form.VolumeMinus();
        }

        public static void RemoveVocalLeftOrRight()
        {
            main_form.RemoveVocalLeftOrRight();
        }

        public static bool AddRemoteSongToQueue(string song)
        {
            return main_form.AddRemoteSongToQueue(song);
        }

        public static void On_SongListView_Item_Selected()
        {
            main_form.on_SongListView_Item_Selected();
        }

        public static void KeyPlus()
        {
            main_form.keyPlusbtn();
        }

        public static void KeyMinus()
        {
            main_form.keyMinusBtn();
        }

        public static void TempoPlus()
        {
            main_form.tempoPlusBtn();
        }

        public static void TempoMinus()
        {
            main_form.tempoMinusBtn();
        }

       /* public static void ShowEffectsChannel1()
        {
            Channel1Fx.ShowInterface();
        }

        public static void ShowEffectsChannel2()
        {
            Channel2Fx.ShowInterface();
        }

        public static void ShowEffectsChannel3()
        {
            Channel3Fx.ShowInterface();
        }

        public static void ShowEffectsChannel4()
        {
            Channel4Fx.ShowInterface();
        }*/

        public static void ResizeScreen1()
        {
            main_form.ResizeScreen1();
        }

        public static void ResizeScreen2()
        {
            main_form.ResizeScreen2();
        }

        public static void FullScreenPlayer_Resize() {

        }

        public static void ShowFullScreen()
        {
            main_form.ShowCDGWindow();
        }

        public static void LoadPreferecesForm()
        {
            Effects.GetorSetFx = Effects.Load.CHANNETSTRIP;
            PlayerControl.PRefsTabIndex = 0;
            main_form.LoadPreferecesForm();
        }

        public static bool GetVideoBG(string sDir)
        {
            return main_form.GetVideoBG(sDir);
        }

        public static void SetDefaultVideoBG(IntPtr handle)
        {
            main_form.SetDefaultVideoBG(handle);
        }

        public static void ViewNextPreviewVideoBG()
        {
            main_form.ViewNextPreviewVideoBG();
        }

        public static void ViewPreviousPreviewVideoBG()
        {
            main_form.ViewPreviousPreviewVideoBG();
        }

        public static void StopPreviewVideoBG()
        {
            main_form.StopPreviewVideoBG();
        }

        public static void LoadDefaultVideoBG()
        {
            main_form.LoadDefaultVideoBG();
        }

        public static void SetAudioOutputDevice()
        {
            main_form.SetAudioOutputDevice();
        }

        public static void Dispose()
        {
            main_form.FormClosed();
        }

        public static MainForm MainFormControl { get; set; }
        public static int PRefsTabIndex { get; set; }
        public static Preferences PrefsForm { get; set; }
        public static List<ListViewItem> AllSongs { get; set; }
        public static PictureBox PlayProgress { get; set; }
        public static PictureBox LeftChannelLevelDb { get; set; }
        public static PictureBox RightChannelLevelDb { get; set; }
        public static ListView SongListView { get; set; }
        public static Label SongTitle { get; set; }
        public static Label SongArtist { get; set; }
        public static Label Time { get; set; }
        public static Label TimeRemain { get; set; }
        public static Label KeyValue { get; set; }
        public static Label TempoValue { get; set; }
        public static TrackBar VolumeScroll { get; set; }
        public static TrackBar MicVolumeScroll { get; set; }
        public static string VolumeLevel { get; set; }
        public static Panel KeyTempoPanel { get; set; }
        public static Button PlayPauseButton { get; set; }
        public static Button NextButton { get; set; }
        public static Button VolumeButton { get; set; }
        public static Button RemoveVocal { get; set; }
        public static string GetNextSong { get; set; }
        public static bool SetAsNextSong { get; set; }
        public static int CurrentTrackChannel { get; set; }
        public static bool IsAddToReserve { get; set; }
        public static DefaultAudioOutput DefaultAudioOutput { get; set; }
        public static ChannelSelected ChannelSelected { get; set; }
        public static int LoadSaveProgress { get; set; }
        public static SplitContainer splitContainer { get; set; }

        //Temporary Solution for Android kfn remote app so we can still use kfn remote on karafun

        public static string  GetNowPlaying()
        {
            return main_form.GetNowPlaying();
        }

        public static void AddSongToQueue(string songId)
        {
             main_form.AddSongToQueue(songId);
        }
    }
}
