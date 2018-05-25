using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Enums;
using bossdoyKaraoke_NOW.Models;
using bossdoyKaraoke_NOW.Models.VocalEffects;
using Un4seen.Bass;
using Un4seen.BassAsio;
using Un4seen.BassWasapi;
using static bossdoyKaraoke_NOW.Enums.DefaultAudio;

namespace bossdoyKaraoke_NOW
{
    public partial class Preferences : Form
    {
        ToolTip m_tooltip = new ToolTip();

        private bool inputDev;
        private bool outputDev;

        private int _asioDevice;
        private bool m_isRequiredPuginsInstalled;
        private string[] m_puginsInstalled = new string[] { "ASIO4ALL", "VBCABLE, The Virtual Audio Cable", "MeldaProduction Audio Plugins" };

        public Preferences()
        {
            InitializeComponent();
        }

        private void Preferences_Load(object sender, EventArgs e)
        {

            PrefTabControl.SelectTab(PlayerControl.PRefsTabIndex);
            m_tooltip.SetToolTip(chkBoxRefreshAsio, "Enable this option when noise artifact is present in audio.");

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

                        this.comboBoxInputDevice.Items.Add(info.name); //string.Format("{0} - {1}", i, info.name));
                        if (info.IsDefault)
                            this.comboBoxInputDevice.SelectedIndex = defaulInputDevice;

                        defaulInputDevice++;
                    }

                    if (info.IsEnabled && !info.IsInput)
                    {
                        this.comboBoxOutputDevice.Items.Add(info.name);
                        if (info.IsDefault)
                            this.comboBoxOutputDevice.SelectedIndex = defaulOutputDevice;

                        defaulOutputDevice++;
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
                this.comboBoxOutputDevice.Items.AddRange(BassAsioDevice.GetAsioOutputChannels.ToArray());
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

            if (Player.IsBassInitialized) {
                this.comboBoxOutputDevice.Items.Clear();
                this.comboBoxOutputDevice.Items.AddRange(Bass.BASS_GetDeviceInfos());

                if (this.comboBoxOutputDevice.Items.Count > 0)
                    this.comboBoxOutputDevice.SelectedIndex = Player.DefaultDevice;

                microphone_setting.Enabled = false;
                chkBoxRefreshAsio.Enabled = false;
                groupMicrophoneEffects.Enabled = false;
            }
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
            if (inputDev)
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
                    inputDev = false;
                    outputDev = false;
                }                  
            }
        }

        private void comboBoxOutputDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (outputDev)
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
                if (Player.IsBassInitialized)
                {
                    Player.DefaultDevice = comboBoxOutputDevice.SelectedIndex;
                    Bass.BASS_ChannelSetDevice(Player.Mixer, comboBoxOutputDevice.SelectedIndex);
                }
                outputDev = false;
                inputDev = false;
            }
            /*  if (outputDev)
              {
                  //Get the device index from the selected device
                  DeviceInfo info = (DeviceInfo)comboBoxOutputDevice.Items[comboBoxOutputDevice.SelectedIndex];
                  if (info == null) return;

                  WasapiDevice.Stop();

                  WasapiDevice.SetDevice(WasapiDevice.InputDevice, info.WasapiDeviceNum, this);
                  WasapiDevice.Start();

              }*/


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
            Channel4Fx.ShowInterface();
        }

        private void comboBoxInputDevice_Click(object sender, EventArgs e)
        {
            inputDev = true;
        }

        private void comboBoxOutputDevice_Click(object sender, EventArgs e)
        {
            outputDev = true;
        }

        private void radioBtnBass_Click(object sender, EventArgs e)
        {
            if (radioBtnBass.Checked)
            {
                if (!m_isRequiredPuginsInstalled)
                {
                    AppSettings.Set("DefaultAudioOutput", DefaultAudioOutput.Bass.ToString());
                    Application.Restart();
                    Environment.Exit(0);
                }
            }
        }

        private void radioBtnWasapi_Click(object sender, EventArgs e)
        {
            if (radioBtnWasapi.Checked) {
                AppSettings.Set("DefaultAudioOutput", DefaultAudioOutput.Wasapi.ToString());
                Application.Restart();
                Environment.Exit(0);
            }
        }

        private void radioBtnAsio_Click(object sender, EventArgs e)
        {
            string app = string.Empty;
            string a = string.Empty;
            string v = string.Empty;
            string m = string.Empty;

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
                    Application.Restart();
                    Environment.Exit(0);
                }
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
            if (chkBoxRefreshAsio.Checked)
                AppSettings.Set("IsAsioAutoRestart", "true");
            else
                AppSettings.Set("IsAsioAutoRestart", "false");
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
