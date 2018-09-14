using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Adb;
using bossdoyKaraoke_NOW.Enums;
using bossdoyKaraoke_NOW.Models;
using bossdoyKaraoke_NOW.Models.VocalEffects;
using bossdoyKaraoke_NOW.Nlog;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using static bossdoyKaraoke_NOW.Enums.DefaultAudio;

namespace bossdoyKaraoke_NOW
{
    public partial class Preferences : Form
    {
        ToolTip m_tooltip = new ToolTip();
        FolderBrowserDialog m_fbd = new FolderBrowserDialog();

        private Implementation.Equalizer m_equalizer;
        private Equalizer.BandValue[] defBandValue = new Equalizer.BandValue[12];
        private bool m_inputDev;
        private bool m_outputDev;
        private bool m_EQdefault;
        private bool m_customEQ;

        private float m_trackBarGain;
        private int m_asioDevice;
        private bool m_isRequiredPuginsInstalled;
        private string[] m_puginsInstalled = new string[] { "ASIO4ALL", "VBCABLE, The Virtual Audio Cable", "MeldaProduction Audio Plugins" };

        public Preferences()
        {
            InitializeComponent();
        }

        private void Preferences_Load(object sender, EventArgs e)
        {
            try
            {

                PrefTabControl.SelectTab(PlayerControl.PRefsTabIndex);
                m_tooltip.SetToolTip(chkBoxRefreshAsio, "Enable this option when noise artifact is present in audio.");

                string videoDir = Main_Form.iniFileHelper.Read("Video", "Video Path");

                videoLbl.Text = videoDir == string.Empty ? "Video: Default" : "Video: " + videoDir;

                if (videoDir == string.Empty) videoDir = VlcPlayer.BGVideoPath + @"VIDEO_NATURE\";

                PlayerControl.GetVideoBG(videoDir);

                PlayerControl.SetDefaultVideoBG(panelPlayer.Handle);

                DataTable dt = new DataTable();
                dt.Columns.Add("ID", typeof(int));
                dt.Columns.Add("Name");

                DataRow dr = dt.NewRow();

                if (Equalizer.ArrBandValue[11].PreSet == -1)
                {
                    dr["Name"] = "";
                    dr["ID"] = 0;

                    dt.Rows.Add(dr);

                    defBandValue = Equalizer.ArrBandValue;

                }

                for (int i = 0; i < VlcPlayer.EqPresets.Values.Count(); i++)
                {
                    dr = dt.NewRow();
                    dr["Name"] = VlcPlayer.EqPresets[i].Name;
                    dr["ID"] = VlcPlayer.EqPresets[i].Index;

                    dt.Rows.Add(dr);
                }

                comboBoxEQPresets.DataSource = dt;// VlcPlayer.EqPresets.Values.ToList();
                comboBoxEQPresets.DisplayMember = "Name";
                comboBoxEQPresets.DropDownStyle = ComboBoxStyle.DropDownList;

                comboBoxEQPresets.SelectedIndex = Equalizer.ArrBandValue[11].PreSet;

                SetBandGain();

                if (PlayerControl.DefaultAudioOutput == DefaultAudioOutput.Bass)
                {
                    radioBtnBass.Checked = true;
                    asio_control_btn.Enabled = false;
                }

                if (PlayerControl.DefaultAudioOutput == DefaultAudioOutput.Wasapi)
                {
                    radioBtnWasapi.Checked = true;
                    asio_control_btn.Enabled = false;
                }

                if (PlayerControl.DefaultAudioOutput == DefaultAudioOutput.Asio)
                {
                    radioBtnAsio.Checked = true;
                    asio_control_btn.Enabled = true;
                }

                if (Player.IsWasapiInitialized)
                {
                    int defaulInputDevice = 0;
                    int defaulOutputDevice = 0;
                    this.comboBoxInputDevice.Items.Clear();
                    this.comboBoxOutputDevice.Items.Clear();
                    BASS_WASAPI_DEVICEINFO[] wasapiDevices = BassWasapi.BASS_WASAPI_GetDeviceInfos();
                    for (int i = 0; i < wasapiDevices.Length; i++)
                    {
                        BASS_WASAPI_DEVICEINFO info = wasapiDevices[i];

                        if (info.IsEnabled && info.IsInput)
                        {
                            defaulInputDevice = i;
                            int index = this.comboBoxInputDevice.Items.Add(new DeviceInfo(info, defaulInputDevice)); //string.Format("{0} - {1}", i, info.name));
                            if (info.IsDefault)
                                this.comboBoxInputDevice.SelectedIndex = index;

                            //defaulInputDevice++;
                        }

                        if (info.IsEnabled && !info.IsInput)
                        {
                            defaulOutputDevice = i;
                            int index = this.comboBoxOutputDevice.Items.Add(new DeviceInfo(info, defaulOutputDevice));
                            if (info.IsDefault)
                                this.comboBoxOutputDevice.SelectedIndex = index;

                            // defaulOutputDevice++;
                        }
                    }

                    RefreshEffects();
                }

                if (Player.IsAsioInitialized)
                {
                    this.comboBoxInputDevice.Items.Clear();
                    this.comboBoxInputDevice.Items.AddRange(BassAsioDevice.GetAsioInputChannels.ToArray());
                    if (this.comboBoxInputDevice.Items.Count > 0)
                        this.comboBoxInputDevice.SelectedIndex = BassAsioDevice.inputDevice;

                    this.comboBoxOutputDevice.Items.Clear();
                    this.comboBoxOutputDevice.Items.AddRange(BassAsioDevice.AsioOutputChannels.ToArray());
                    if (this.comboBoxOutputDevice.Items.Count > 0)
                        this.comboBoxOutputDevice.SelectedIndex = BassAsioDevice.outputDevice;

                    microphone_setting.Enabled = true;

                    if (AppSettings.Get<bool>("IsAsioAutoRestart"))
                        chkBoxRefreshAsio.Checked = true;
                    else
                        chkBoxRefreshAsio.Checked = false;

                    chkBoxRefreshAsio.Enabled = true;

                    groupMicrophoneEffects.Enabled = true;

                    RefreshEffects();
                }

                if (Player.IsBassInitialized)
                {
                    this.comboBoxOutputDevice.Items.Clear();
                    this.comboBoxOutputDevice.Items.AddRange(Bass.BASS_GetDeviceInfos());

                    if (this.comboBoxOutputDevice.Items.Count > 0)
                        this.comboBoxOutputDevice.SelectedIndex = Player.DefaultDevice;

                    microphone_setting.Enabled = false;
                    chkBoxRefreshAsio.Enabled = false;
                    groupMicrophoneEffects.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "Preferences_Load", ex.LineNumber(), this.Name);

            }
        }

        private void Preferences_FormClosed(object sender, FormClosedEventArgs e)
        {
            PlayerControl.StopPreviewVideoBG();
        }

        private void RefreshEffects()
        {
            // PlayerControl.ShowEffectsChannel1();
            //PlayerControl.ShowEffectsChannel2();
            //PlayerControl.ShowEffectsChannel3();
            // PlayerControl.ShowEffectsChannel4();

            Channel1Fx.ShowInterface();
            Channel2Fx.ShowInterface();
            Channel3Fx.ShowInterface();
            Channel4Fx.ShowInterface();
        }

        private void comboBoxInputDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*   if (inputDev)
               {
                   //Get the device index from the selected device
                   DeviceInfo info = (DeviceInfo)comboBoxInputDevice.Items[comboBoxInputDevice.SelectedIndex];
                   if (info == null) return;

                   WasapiDevice.Stop();

                   WasapiDevice.SetDevice(info.WasapiDeviceNum, WasapiDevice.GetOutputDefaultDevice(), this);
                   WasapiDevice.Start();

               }*/
            try
            {
                if (m_inputDev)
                {
                    if (Player.IsAsioInitialized)
                    {
                        BassAsioDevice.inputDevice = comboBoxInputDevice.SelectedIndex;
                        //groupMicrophoneEffects.Visible = false;
                        BASS_ASIO_CHANNELINFO chanInfo = BassAsio.BASS_ASIO_ChannelGetInfo(true, comboBoxInputDevice.SelectedIndex * 2);
                        BassAsioDevice.Stop();
                        BassAsioDevice.SetDevice(comboBoxInputDevice.SelectedIndex * 2, comboBoxOutputDevice.SelectedIndex * 2);
                        BassAsioDevice.Start();
                        Effects.GetorSetFx = Effects.Load.CHANNETSTRIP;
                        RefreshEffects();
                        // groupMicrophoneEffects.Visible = true;
                        m_inputDev = false;
                        m_outputDev = false;
                    }

                    if (Player.IsWasapiInitialized)
                    {
                        //Get the device index from the selected device
                        DeviceInfo info = (DeviceInfo)comboBoxInputDevice.Items[comboBoxInputDevice.SelectedIndex];
                        if (info == null) return;

                        WasapiDevice.Stop();
                        WasapiDevice.SetDevice(info.WasapiDeviceNum, WasapiDevice.GetOutputDefaultDevice());
                        WasapiDevice.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "comboBoxInputDevice_SelectedIndexChanged", ex.LineNumber(), this.Name);

            }
        }

        private void comboBoxOutputDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_outputDev)
                {
                    if (Player.IsAsioInitialized)
                    {
                        BassAsioDevice.outputDevice = comboBoxOutputDevice.SelectedIndex;
                        //groupMicrophoneEffects.Visible = false;
                        BASS_ASIO_CHANNELINFO chanInfo = BassAsio.BASS_ASIO_ChannelGetInfo(false, comboBoxOutputDevice.SelectedIndex * 2);
                        BassAsioDevice.Stop();
                        BassAsioDevice.SetDevice(comboBoxInputDevice.SelectedIndex * 2, comboBoxOutputDevice.SelectedIndex * 2);
                        BassAsioDevice.Start();
                        Effects.GetorSetFx = Effects.Load.CHANNETSTRIP;
                        RefreshEffects();
                        // groupMicrophoneEffects.Visible = true;
                    }
                    if (Player.IsWasapiInitialized)
                    {
                        //Get the device index from the selected device
                        DeviceInfo info = (DeviceInfo)comboBoxOutputDevice.Items[comboBoxOutputDevice.SelectedIndex];
                        if (info == null) return;

                        WasapiDevice.Stop();

                        WasapiDevice.SetDevice(WasapiDevice.InputDevice, info.WasapiDeviceNum);
                        WasapiDevice.Start();
                    }
                    if (Player.IsBassInitialized)
                    {
                        Player.DefaultDevice = comboBoxOutputDevice.SelectedIndex;
                        Bass.BASS_ChannelSetDevice(Player.Mixer, comboBoxOutputDevice.SelectedIndex);
                    }

                    m_outputDev = false;
                    m_inputDev = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "comboBoxOutputDevice_SelectedIndexChanged", ex.LineNumber(), this.Name);

            }
        }

        private void EQ1_0_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.EQ1_0;
            //PlayerControl.ShowEffectsChannel1();
            Channel1Fx.ShowInterface();
        }

        private void Compressor_0_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.COMPRESSOR1_0;
            // PlayerControl.ShowEffectsChannel1();
            Channel1Fx.ShowInterface();
        }

        private void EQ4_0_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.EQ7_0;
            //PlayerControl.ShowEffectsChannel1();
            Channel1Fx.ShowInterface();
        }

        private void EQ4_0_Phone_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.EQ7_PHONE;
            //PlayerControl.ShowEffectsChannel1();
            Channel1Fx.ShowInterface();
        }

        private void DeEsser_0_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.DeEsser0_0;
            //PlayerControl.ShowEffectsChannel1();
            Channel1Fx.ShowInterface();
        }

        private void EQ1_1_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.EQ1_1;
            //PlayerControl.ShowEffectsChannel2();
            Channel2Fx.ShowInterface();
        }

        private void Compressor_1_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.COMPRESSOR1_1;
            //PlayerControl.ShowEffectsChannel2();
            Channel2Fx.ShowInterface();
        }

        private void EQ4_1_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.EQ7_1;
            // PlayerControl.ShowEffectsChannel2();
            Channel2Fx.ShowInterface();
        }

        private void DeEsser_1_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.DeEsser0_1;
            // PlayerControl.ShowEffectsChannel2();
            Channel2Fx.ShowInterface();
        }

        private void EQ1_2_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.EQ1_2;
            //PlayerControl.ShowEffectsChannel3();
            Channel3Fx.ShowInterface();
        }

        private void Compressor_2_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.COMPRESSOR1_2;
            // PlayerControl.ShowEffectsChannel3();
            Channel3Fx.ShowInterface();
        }

        private void EQ4_2_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.EQ7_2;
            //PlayerControl.ShowEffectsChannel3();
            Channel3Fx.ShowInterface();
        }

        private void DeEsser_2_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.DeEsser0_2;
            //PlayerControl.ShowEffectsChannel3();
            Channel3Fx.ShowInterface();
        }

        private void Reverb_Click(object sender, EventArgs e)
        {
            Effects.GetorSetFx = Effects.Load.REVERB;
            //PlayerControl.ShowEffectsChannel4();
            //Channel4Fx.ShowInterface();
            Channel1Fx.ShowInterface();
        }

        private void comboBoxInputDevice_Click(object sender, EventArgs e)
        {
            m_inputDev = true;
        }

        private void comboBoxOutputDevice_Click(object sender, EventArgs e)
        {
            m_outputDev = true;
        }

        private void radioBtnBass_Click(object sender, EventArgs e)
        {
            try
            {
                if (radioBtnBass.Checked)
                {
                    if (!m_isRequiredPuginsInstalled)
                    {
                        AppSettings.Set("DefaultAudioOutput", DefaultAudioOutput.Bass.ToString());
                        this.Close();
                        Application.Restart();
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "radioBtnBass_Click", ex.LineNumber(), this.Name);

            }
        }

        private void radioBtnWasapi_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("Todo: Still having problem with audio, no sound.");

                /*if (radioBtnWasapi.Checked) {
                    AppSettings.Set("DefaultAudioOutput", DefaultAudioOutput.Wasapi.ToString());
                    this.Close();
                    Application.Restart();
                    Environment.Exit(0);
                }*/
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "radioBtnWasapi_Click", ex.LineNumber(), this.Name);

            }
        }

        private void radioBtnAsio_Click(object sender, EventArgs e)
        {
            string app = string.Empty;
            string a = string.Empty;
            string v = string.Empty;
            string m = string.Empty;

            try
            {

                if (radioBtnAsio.Checked)
                {
                    int count = 0;
                    for (int p = 0; p < m_puginsInstalled.Count(); p++)
                    {
                        m_isRequiredPuginsInstalled = AppSettings.IsApplictionInstalled(m_puginsInstalled[p]); // AppSettings.CheckRequiredPluginsForBassasio("VBCABLE, The Virtual");
                        if (!m_isRequiredPuginsInstalled)
                        {
                            Console.WriteLine(m_puginsInstalled[p]);

                            count += 1;

                            if (p == 0)
                                a = "\n" + count + ". " + m_puginsInstalled[0] + " : " + "http://www.asio4all.org/";
                            if (p == 1)
                                v = "\n" + count + ". " + m_puginsInstalled[1] + " : " + "https://www.vb-audio.com/Cable/";
                            if (p == 2)
                                m = "\n" + count + ". " + m_puginsInstalled[2] + " : " + "https://www.meldaproduction.com/download/plugins";

                            app = a + v + m;
                        }

                    }

                    if (count > 0)
                        m_isRequiredPuginsInstalled = false;

                    if (!m_isRequiredPuginsInstalled)
                    {
                        MessageBox.Show("To use Asio additional program(s) needs to be installed.\n" + app);
                        radioBtnBass.Checked = true;
                        radioBtnAsio.Checked = false;
                        return;
                    }
                    else
                    {
                        AppSettings.Set("DefaultAudioOutput", DefaultAudioOutput.Asio.ToString());
                        this.Close();
                        Application.Restart();
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "radioBtnAsio_Click", ex.LineNumber(), this.Name);

            }
        }

        private void asio_control_btn_Click(object sender, EventArgs e)
        {
            BassAsio.BASS_ASIO_ControlPanel();
        }

        private void Preferences_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void chkBoxRefreshAsio_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkBoxRefreshAsio.Checked)
                    AppSettings.Set("IsAsioAutoRestart", "true");
                else
                    AppSettings.Set("IsAsioAutoRestart", "false");
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "chkBoxRefreshAsio_CheckedChanged", ex.LineNumber(), this.Name);
            }
        }

        private void selectVideobtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_fbd.ShowDialog() == DialogResult.OK)
                {
                    // string[] filePath = new string[] { m_fbd.SelectedPath };
                    string folderName = Path.GetFileName(m_fbd.SelectedPath);
                    bool isVideoFound = PlayerControl.GetVideoBG(m_fbd.SelectedPath);
                    if (isVideoFound)
                    {
                        videoLbl.Text = "Video: " + m_fbd.SelectedPath;
                        panelPlayer.Invalidate();
                        PlayerControl.SetDefaultVideoBG(panelPlayer.Handle);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "selectVideobtn_Click", ex.LineNumber(), this.Name);
            }
        }

        private void nextBtn_Click(object sender, EventArgs e)
        {
            PlayerControl.ViewNextPreviewVideoBG();
        }

        private void prevbtn_Click(object sender, EventArgs e)
        {
            PlayerControl.ViewPreviousPreviewVideoBG();
        }

        private void applyVideobtn_Click(object sender, EventArgs e)
        {
            PlayerControl.LoadDefaultVideoBG();
            Main_Form.iniFileHelper.Write("Video", "Video Path", m_fbd.SelectedPath);
        }

        public IntPtr VideoHandle { get { return panelPlayer.Handle; } }

        private void SetBandGain()
        {
            trackBarEQ0.Value = (int)Equalizer.ArrBandValue[0].Gain;
            trackBarEQ1.Value = (int)Equalizer.ArrBandValue[1].Gain;
            trackBarEQ2.Value = (int)Equalizer.ArrBandValue[2].Gain;
            trackBarEQ3.Value = (int)Equalizer.ArrBandValue[3].Gain;
            trackBarEQ4.Value = (int)Equalizer.ArrBandValue[4].Gain;
            trackBarEQ5.Value = (int)Equalizer.ArrBandValue[5].Gain;
            trackBarEQ6.Value = (int)Equalizer.ArrBandValue[6].Gain;
            trackBarEQ7.Value = (int)Equalizer.ArrBandValue[7].Gain;
            trackBarEQ8.Value = (int)Equalizer.ArrBandValue[8].Gain;
            trackBarEQ9.Value = (int)Equalizer.ArrBandValue[9].Gain;
            trackBarPreamp.Value = (int)Equalizer.ArrBandValue[10].PreAmp;
        }

        private void trackBarEQ0_ValueChanged(object sender, EventArgs e)
        {

            m_trackBarGain = (float)trackBarEQ0.Value / 10;
            lblBand1.Text = m_trackBarGain == 0.0f ? (0).ToString() : m_trackBarGain.ToString();

           // PlayerControl.UpdateEQ(0, trackBarEQ0.Value);
            //if (!m_customEQ)
            //{
            //    PlayerControl.SaveEQSettings(0);
            //}
        }

        private void trackBarEQ0_Scroll(object sender, EventArgs e)
        {
            PlayerControl.UpdateEQ(0, trackBarEQ0.Value);
            m_customEQ = true;
        }

        private void trackBarEQ0_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_customEQ)
            {
                Equalizer.ArrBandValue[11].PreSet = -1; //PlayerControl.UpdateEQPresets(-1);
                PlayerControl.SaveEQSettings(0);
                m_customEQ = false;
            }
        }

        private void trackBarEQ1_ValueChanged(object sender, EventArgs e)
        {
            m_trackBarGain = (float)trackBarEQ1.Value / 10;
            lblBand2.Text = m_trackBarGain == 0.0f ? (0).ToString() : m_trackBarGain.ToString();

           // PlayerControl.UpdateEQ(1, trackBarEQ1.Value);
           // if (!m_customEQ)
           // {
          //      PlayerControl.SaveEQSettings(1);
          //  }
        }

        private void trackBarEQ1_Scroll(object sender, EventArgs e)
        {
            PlayerControl.UpdateEQ(1, trackBarEQ1.Value);
            m_customEQ = true;
        }

        private void trackBarEQ1_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_customEQ)
            {
                Equalizer.ArrBandValue[11].PreSet = -1; //PlayerControl.UpdateEQPresets(-1);
                PlayerControl.SaveEQSettings(1);
                m_customEQ = false;
            }
        }

        private void trackBarEQ2_ValueChanged(object sender, EventArgs e)
        {
            m_trackBarGain = (float)trackBarEQ2.Value / 10;
            lblBand3.Text = m_trackBarGain == 0.0f ? (0).ToString() : m_trackBarGain.ToString();

            //PlayerControl.UpdateEQ(2, trackBarEQ2.Value);
            //if (!m_customEQ)
            //{
           //     PlayerControl.SaveEQSettings(2);
           // }
        }

        private void trackBarEQ2_Scroll(object sender, EventArgs e)
        {
            PlayerControl.UpdateEQ(2, trackBarEQ2.Value);
            m_customEQ = true;
        }

        private void trackBarEQ2_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_customEQ)
            {
                Equalizer.ArrBandValue[11].PreSet = -1; // PlayerControl.UpdateEQPresets(-1);
                PlayerControl.SaveEQSettings(2);
                m_customEQ = false;
            }
        }

        private void trackBarEQ3_ValueChanged(object sender, EventArgs e)
        {
            m_trackBarGain = (float)trackBarEQ3.Value / 10;
            lblBand4.Text = m_trackBarGain == 0.0f ? (0).ToString() : m_trackBarGain.ToString();

           // PlayerControl.UpdateEQ(3, trackBarEQ3.Value);
           // if (!m_customEQ)
           // {
           //     PlayerControl.SaveEQSettings(3);
           // }
        }

        private void trackBarEQ3_Scroll(object sender, EventArgs e)
        {
            PlayerControl.UpdateEQ(3, trackBarEQ3.Value);
            m_customEQ = true;
        }

        private void trackBarEQ3_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_customEQ)
            {
                Equalizer.ArrBandValue[11].PreSet = -1; //PlayerControl.UpdateEQPresets(-1);
                PlayerControl.SaveEQSettings(3);
                m_customEQ = false;
            }
        }

        private void trackBarEQ4_ValueChanged(object sender, EventArgs e)
        {
            m_trackBarGain = (float)trackBarEQ4.Value / 10;
            lblBand5.Text = m_trackBarGain == 0.0f ? (0).ToString() : m_trackBarGain.ToString();

            //PlayerControl.UpdateEQ(4, trackBarEQ4.Value);
            //if (!m_customEQ)
           // {
           //     PlayerControl.SaveEQSettings(4);
           // }
        }

        private void trackBarEQ4_Scroll(object sender, EventArgs e)
        {
            PlayerControl.UpdateEQ(4, trackBarEQ4.Value);
            m_customEQ = true;
        }

        private void trackBarEQ4_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_customEQ)
            {
                Equalizer.ArrBandValue[11].PreSet = -1; //PlayerControl.UpdateEQPresets(-1);
                PlayerControl.SaveEQSettings(4);
                m_customEQ = false;
            }
        }


        private void trackBarEQ5_ValueChanged(object sender, EventArgs e)
        {
            m_trackBarGain = (float)trackBarEQ5.Value / 10;
            lblBand6.Text = m_trackBarGain == 0.0f ? (0).ToString() : m_trackBarGain.ToString();

           // PlayerControl.UpdateEQ(5, trackBarEQ5.Value);
            //if (!m_customEQ)
           // {
            //    PlayerControl.SaveEQSettings(5);
            //}
        }

        private void trackBarEQ5_Scroll(object sender, EventArgs e)
        {
            PlayerControl.UpdateEQ(5, trackBarEQ5.Value);
            m_customEQ = true;
        }

        private void trackBarEQ5_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_customEQ)
            {
                Equalizer.ArrBandValue[11].PreSet = -1; //PlayerControl.UpdateEQPresets(-1);
                PlayerControl.SaveEQSettings(5);
                m_customEQ = false;
            }
        }

        private void trackBarEQ6_ValueChanged(object sender, EventArgs e)
        {
            m_trackBarGain = (float)trackBarEQ6.Value / 10;
            lblBand7.Text = m_trackBarGain == 0.0f ? (0).ToString() : m_trackBarGain.ToString();

            //PlayerControl.UpdateEQ(6, trackBarEQ6.Value);
           // if (!m_customEQ)
            //{
           //     PlayerControl.SaveEQSettings(6);
           // }
        }


        private void trackBarEQ6_Scroll(object sender, EventArgs e)
        {
            PlayerControl.UpdateEQ(6, trackBarEQ6.Value);
            m_customEQ = true;
        }

        private void trackBarEQ6_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_customEQ)
            {
                Equalizer.ArrBandValue[11].PreSet = -1; //PlayerControl.UpdateEQPresets(-1);
                PlayerControl.SaveEQSettings(6);
                m_customEQ = false;
            }
        }

        private void trackBarEQ7_ValueChanged(object sender, EventArgs e)
        {
            m_trackBarGain = (float)trackBarEQ7.Value / 10;
            lblBand8.Text = m_trackBarGain == 0.0f ? (0).ToString() : m_trackBarGain.ToString();

           // PlayerControl.UpdateEQ(7, trackBarEQ7.Value);
            //if (!m_customEQ)
           // {
           //     PlayerControl.SaveEQSettings(7);
           // }
        }

        private void trackBarEQ7_Scroll(object sender, EventArgs e)
        {
            PlayerControl.UpdateEQ(7, trackBarEQ7.Value);
            m_customEQ = true;
        }

        private void trackBarEQ7_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_customEQ)
            {
                Equalizer.ArrBandValue[11].PreSet = -1; //PlayerControl.UpdateEQPresets(-1);
                PlayerControl.SaveEQSettings(7);
                m_customEQ = false;
            }
        }

        private void trackBarEQ8_ValueChanged(object sender, EventArgs e)
        {
            m_trackBarGain = (float)trackBarEQ8.Value / 10;
            lblBand9.Text = m_trackBarGain == 0.0f ? (0).ToString() : m_trackBarGain.ToString();

           // PlayerControl.UpdateEQ(8, trackBarEQ8.Value);
           // if (!m_customEQ)
           // {
           //     PlayerControl.SaveEQSettings(8);
           // }
        }

        private void trackBarEQ8_Scroll(object sender, EventArgs e)
        {
            PlayerControl.UpdateEQ(8, trackBarEQ8.Value);
            m_customEQ = true;
        }

        private void trackBarEQ8_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_customEQ)
            {
                Equalizer.ArrBandValue[11].PreSet = -1; //PlayerControl.UpdateEQPresets(-1);
                PlayerControl.SaveEQSettings(8);
                m_customEQ = false;
            }
        }

        private void trackBarEQ9_ValueChanged(object sender, EventArgs e)
        {
            m_trackBarGain = (float)trackBarEQ9.Value / 10;
            lblBand10.Text = m_trackBarGain == 0.0f ? (0).ToString() : m_trackBarGain.ToString();

           // PlayerControl.UpdateEQ(9, trackBarEQ9.Value);
           // if (!m_customEQ)
           // {
           //     PlayerControl.SaveEQSettings(9);
           // }
        }

        private void trackBarEQ9_Scroll(object sender, EventArgs e)
        {
            PlayerControl.UpdateEQ(9, trackBarEQ9.Value);
            m_customEQ = true;
        }

        private void trackBarEQ9_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_customEQ)
            {
                Equalizer.ArrBandValue[11].PreSet = -1; //PlayerControl.UpdateEQPresets(-1);
                PlayerControl.SaveEQSettings(9);
                m_customEQ = false;
            }
        }

        private void trackBarPreamp_ValueChanged(object sender, EventArgs e)
        {
            m_trackBarGain = (float)trackBarPreamp.Value / 10;
            lblPreampGain.Text = m_trackBarGain == 0.0f ? (0).ToString() : m_trackBarGain.ToString();

           // PlayerControl.UpdateEQPreamp(trackBarPreamp.Value);
           // if (!m_customEQ)
          //  {
          //      PlayerControl.SaveEQSettings(-1);
           // }
        }

        private void trackBarPreamp_Scroll(object sender, EventArgs e)
        {
            PlayerControl.UpdateEQPreamp(trackBarPreamp.Value);
            m_customEQ = true;
        }

        private void trackBarPreamp_MouseUp(object sender, MouseEventArgs e)
        {
            if (m_customEQ)
            {
                //PlayerControl.UpdateEQPresets(-1);
                Equalizer.ArrBandValue[11].PreSet = -1;
                PlayerControl.SaveEQSettings(-1);
                m_customEQ = false;
            }
        }

        private void comboBoxEQPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (m_EQdefault)
                {
                    if (m_equalizer != null) m_equalizer.Dispose();

                    int selectedIndex = comboBoxEQPresets.SelectedIndex;

                    if (comboBoxEQPresets.Items.Count > VlcPlayer.EqPresets.Count() && selectedIndex > 0)
                    {
                        selectedIndex = selectedIndex - 1;
                        m_equalizer = new Implementation.Equalizer(VlcPlayer.EqPresets[selectedIndex]);
                    }
                    else if (comboBoxEQPresets.Items.Count > VlcPlayer.EqPresets.Count() && selectedIndex == 0)
                    {
                        m_equalizer = new Implementation.Equalizer();
                        m_equalizer.Preamp = defBandValue[10].PreAmp / 10;
                        for (int i = 0; i < defBandValue.Length - 2; i++)
                        {
                            m_equalizer.Bands[i].Amplitude = defBandValue[i].Gain / 10;
                        }
                        selectedIndex = -1;
                    }
                    else
                    {
                        m_equalizer = new Implementation.Equalizer(VlcPlayer.EqPresets[selectedIndex]);
                    }

                    Equalizer.ArrBandValue[10].PreAmp = (m_equalizer.Preamp * 10) > 150 ? 150 : m_equalizer.Preamp * 10;
                    Equalizer.ArrBandValue[11].PreSet = selectedIndex;

                    Console.WriteLine("m_equalizer.Preamp: " + m_equalizer.Preamp + " : " + Equalizer.ArrBandValue[10].PreAmp);

                    for (int i = 0; i < Equalizer.ArrBandValue.Length - 2; i++)
                    {
                        float amplitude = m_equalizer.Bands[i].Amplitude * 10;

                        if (amplitude > 150)
                        {
                            amplitude = amplitude - 150;
                            amplitude = (m_equalizer.Bands[i].Amplitude * 10) - amplitude;
                        }

                        Equalizer.ArrBandValue[i].Gain = amplitude;
                        PlayerControl.UpdateEQ(i, amplitude);
                        PlayerControl.SaveEQSettings(i);
                    }
                    PlayerControl.SaveEQSettings(-1);
                    SetBandGain();

                    m_EQdefault = false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogFile(ex.Message, "", "comboBoxEQPresets_SelectedIndexChanged", ex.LineNumber(), this.Name);

            }
        }

        private void comboBoxEQPresets_Click(object sender, EventArgs e)
        {
            m_EQdefault = true;
        }

        private void UpdateEQPreset()
        {

            Equalizer.ArrBandValue[10].PreAmp = (m_equalizer.Preamp * 10) > 150 ? 150 : m_equalizer.Preamp * 10;

            for (int i = 0; i < Equalizer.ArrBandValue.Length - 2; i++)
            {
                float amplitude = m_equalizer.Bands[i].Amplitude * 10;

                if (amplitude > 150)
                {
                    amplitude = amplitude - 150;
                    amplitude = (m_equalizer.Bands[i].Amplitude * 10) - amplitude;
                }

                Equalizer.ArrBandValue[i].Gain = amplitude;
            }
        }

        private void chkBoxEQ_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBoxEQ.Checked)
            {
                comboBoxEQPresets.Enabled = true;
                EQpanel.Enabled = true;
                PlayerControl.EnableEQ(true);
            }
            else
            {
                comboBoxEQPresets.Enabled = false;
                EQpanel.Enabled = false;
                PlayerControl.EnableEQ(false);
            }
        }

        private void installAndroidbtn_Click(object sender, EventArgs e)
        {
            string success;
            int status = AdbClient.Instance.InstallApp();

            if (status == 0)
                success = "KaraokeNow successfully installed";
            else
                success = "Error installing karaokeNow";


            MessageBox.Show(success, "Mobile App Install", MessageBoxButtons.OK);
        }

        private void txtBoxIntro_TextChanged(object sender, EventArgs e)
        {
            PlayerControl.Text_Intro = txtBoxIntro.Text;
        }
    }

    class DeviceInfo
    {
        public BASS_WASAPI_DEVICEINFO WasapiDeviceInfo { get; private set; }
        public int WasapiDeviceNum { get; private set; }

        public DeviceInfo(BASS_WASAPI_DEVICEINFO info, int num)
        {
            WasapiDeviceInfo = info;
            WasapiDeviceNum = num;
        }

        public override string ToString()
        {
            return WasapiDeviceInfo.name;
        }
    }

}
