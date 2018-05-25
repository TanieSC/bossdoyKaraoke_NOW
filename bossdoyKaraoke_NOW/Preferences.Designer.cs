namespace bossdoyKaraoke_NOW
{
    partial class Preferences
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preferences));
            this.PrefTabControl = new System.Windows.Forms.TabControl();
            this.generalTabPage = new System.Windows.Forms.TabPage();
            this.audio_setting = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxOutputDevice = new System.Windows.Forms.ComboBox();
            this.audioTabPage = new System.Windows.Forms.TabPage();
            this.audio_device = new System.Windows.Forms.GroupBox();
            this.radioBtnWasapi = new System.Windows.Forms.RadioButton();
            this.chkBoxRefreshAsio = new System.Windows.Forms.CheckBox();
            this.asio_control_btn = new System.Windows.Forms.Button();
            this.radioBtnAsio = new System.Windows.Forms.RadioButton();
            this.radioBtnBass = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.videoTabPage = new System.Windows.Forms.TabPage();
            this.applyVideobtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.video_settings = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.prevbtn = new System.Windows.Forms.Button();
            this.nextBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.selectVideobtn = new System.Windows.Forms.Button();
            this.videoLbl = new System.Windows.Forms.Label();
            this.panelPlayer = new System.Windows.Forms.Panel();
            this.micTabPage = new System.Windows.Forms.TabPage();
            this.groupMicrophoneEffects = new System.Windows.Forms.GroupBox();
            this.channel1panel = new System.Windows.Forms.Panel();
            this.EQ4_0_Phone = new System.Windows.Forms.Button();
            this.channel4panel = new System.Windows.Forms.Panel();
            this.channel3panel = new System.Windows.Forms.Panel();
            this.channel2panel = new System.Windows.Forms.Panel();
            this.DeEsser_1 = new System.Windows.Forms.Button();
            this.EQ4_1 = new System.Windows.Forms.Button();
            this.Compressor_1 = new System.Windows.Forms.Button();
            this.EQ1_1 = new System.Windows.Forms.Button();
            this.DeEsser_2 = new System.Windows.Forms.Button();
            this.EQ4_2 = new System.Windows.Forms.Button();
            this.Compressor_2 = new System.Windows.Forms.Button();
            this.EQ1_2 = new System.Windows.Forms.Button();
            this.Reverb = new System.Windows.Forms.Button();
            this.DeEsser_0 = new System.Windows.Forms.Button();
            this.EQ4_0 = new System.Windows.Forms.Button();
            this.Compressor_0 = new System.Windows.Forms.Button();
            this.EQ1_0 = new System.Windows.Forms.Button();
            this.microphone_setting = new System.Windows.Forms.GroupBox();
            this.comboBoxInputDevice = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PrefTabControl.SuspendLayout();
            this.generalTabPage.SuspendLayout();
            this.audio_setting.SuspendLayout();
            this.audioTabPage.SuspendLayout();
            this.audio_device.SuspendLayout();
            this.videoTabPage.SuspendLayout();
            this.video_settings.SuspendLayout();
            this.panel1.SuspendLayout();
            this.micTabPage.SuspendLayout();
            this.groupMicrophoneEffects.SuspendLayout();
            this.microphone_setting.SuspendLayout();
            this.SuspendLayout();
            // 
            // PrefTabControl
            // 
            this.PrefTabControl.Controls.Add(this.generalTabPage);
            this.PrefTabControl.Controls.Add(this.audioTabPage);
            this.PrefTabControl.Controls.Add(this.videoTabPage);
            this.PrefTabControl.Controls.Add(this.micTabPage);
            this.PrefTabControl.Location = new System.Drawing.Point(5, 4);
            this.PrefTabControl.Name = "PrefTabControl";
            this.PrefTabControl.SelectedIndex = 0;
            this.PrefTabControl.Size = new System.Drawing.Size(527, 567);
            this.PrefTabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.PrefTabControl.TabIndex = 0;
            // 
            // generalTabPage
            // 
            this.generalTabPage.Controls.Add(this.audio_setting);
            this.generalTabPage.Location = new System.Drawing.Point(4, 22);
            this.generalTabPage.Name = "generalTabPage";
            this.generalTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalTabPage.Size = new System.Drawing.Size(519, 541);
            this.generalTabPage.TabIndex = 0;
            this.generalTabPage.Text = "General";
            this.generalTabPage.UseVisualStyleBackColor = true;
            // 
            // audio_setting
            // 
            this.audio_setting.Controls.Add(this.label2);
            this.audio_setting.Controls.Add(this.comboBoxOutputDevice);
            this.audio_setting.Location = new System.Drawing.Point(6, 8);
            this.audio_setting.Name = "audio_setting";
            this.audio_setting.Size = new System.Drawing.Size(507, 55);
            this.audio_setting.TabIndex = 0;
            this.audio_setting.TabStop = false;
            this.audio_setting.Text = "Sound setting";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Sound device";
            // 
            // comboBoxOutputDevice
            // 
            this.comboBoxOutputDevice.FormattingEnabled = true;
            this.comboBoxOutputDevice.Location = new System.Drawing.Point(120, 19);
            this.comboBoxOutputDevice.Name = "comboBoxOutputDevice";
            this.comboBoxOutputDevice.Size = new System.Drawing.Size(372, 21);
            this.comboBoxOutputDevice.TabIndex = 5;
            this.comboBoxOutputDevice.SelectedIndexChanged += new System.EventHandler(this.comboBoxOutputDevice_SelectedIndexChanged);
            this.comboBoxOutputDevice.Click += new System.EventHandler(this.comboBoxOutputDevice_Click);
            // 
            // audioTabPage
            // 
            this.audioTabPage.Controls.Add(this.audio_device);
            this.audioTabPage.Location = new System.Drawing.Point(4, 22);
            this.audioTabPage.Name = "audioTabPage";
            this.audioTabPage.Size = new System.Drawing.Size(519, 541);
            this.audioTabPage.TabIndex = 2;
            this.audioTabPage.Text = "Audio";
            this.audioTabPage.UseVisualStyleBackColor = true;
            // 
            // audio_device
            // 
            this.audio_device.Controls.Add(this.radioBtnWasapi);
            this.audio_device.Controls.Add(this.chkBoxRefreshAsio);
            this.audio_device.Controls.Add(this.asio_control_btn);
            this.audio_device.Controls.Add(this.radioBtnAsio);
            this.audio_device.Controls.Add(this.radioBtnBass);
            this.audio_device.Controls.Add(this.label3);
            this.audio_device.Location = new System.Drawing.Point(6, 8);
            this.audio_device.Name = "audio_device";
            this.audio_device.Size = new System.Drawing.Size(507, 98);
            this.audio_device.TabIndex = 2;
            this.audio_device.TabStop = false;
            this.audio_device.Text = "Audio setting";
            // 
            // radioBtnWasapi
            // 
            this.radioBtnWasapi.AutoSize = true;
            this.radioBtnWasapi.Location = new System.Drawing.Point(228, 18);
            this.radioBtnWasapi.Name = "radioBtnWasapi";
            this.radioBtnWasapi.Size = new System.Drawing.Size(129, 17);
            this.radioBtnWasapi.TabIndex = 11;
            this.radioBtnWasapi.Text = "Wasapi (BassWasapi)";
            this.radioBtnWasapi.UseVisualStyleBackColor = true;
            this.radioBtnWasapi.Click += new System.EventHandler(this.radioBtnWasapi_Click);
            // 
            // chkBoxRefreshAsio
            // 
            this.chkBoxRefreshAsio.AutoSize = true;
            this.chkBoxRefreshAsio.Enabled = false;
            this.chkBoxRefreshAsio.Location = new System.Drawing.Point(388, 41);
            this.chkBoxRefreshAsio.Name = "chkBoxRefreshAsio";
            this.chkBoxRefreshAsio.Size = new System.Drawing.Size(111, 17);
            this.chkBoxRefreshAsio.TabIndex = 10;
            this.chkBoxRefreshAsio.Text = "Auto Refresh Asio";
            this.chkBoxRefreshAsio.UseVisualStyleBackColor = true;
            this.chkBoxRefreshAsio.CheckedChanged += new System.EventHandler(this.chkBoxRefreshAsio_CheckedChanged);
            // 
            // asio_control_btn
            // 
            this.asio_control_btn.Enabled = false;
            this.asio_control_btn.Location = new System.Drawing.Point(368, 64);
            this.asio_control_btn.Name = "asio_control_btn";
            this.asio_control_btn.Size = new System.Drawing.Size(118, 23);
            this.asio_control_btn.TabIndex = 9;
            this.asio_control_btn.Text = "Asio Control Panel";
            this.asio_control_btn.UseVisualStyleBackColor = true;
            this.asio_control_btn.Click += new System.EventHandler(this.asio_control_btn_Click);
            // 
            // radioBtnAsio
            // 
            this.radioBtnAsio.AutoSize = true;
            this.radioBtnAsio.Location = new System.Drawing.Point(368, 18);
            this.radioBtnAsio.Name = "radioBtnAsio";
            this.radioBtnAsio.Size = new System.Drawing.Size(97, 17);
            this.radioBtnAsio.TabIndex = 8;
            this.radioBtnAsio.Text = "Asio (BassAsio)";
            this.radioBtnAsio.UseVisualStyleBackColor = true;
            this.radioBtnAsio.Click += new System.EventHandler(this.radioBtnAsio_Click);
            // 
            // radioBtnBass
            // 
            this.radioBtnBass.AutoSize = true;
            this.radioBtnBass.Checked = true;
            this.radioBtnBass.Location = new System.Drawing.Point(120, 18);
            this.radioBtnBass.Name = "radioBtnBass";
            this.radioBtnBass.Size = new System.Drawing.Size(97, 17);
            this.radioBtnBass.TabIndex = 7;
            this.radioBtnBass.TabStop = true;
            this.radioBtnBass.Text = "Default - (Bass)";
            this.radioBtnBass.UseVisualStyleBackColor = true;
            this.radioBtnBass.Click += new System.EventHandler(this.radioBtnBass_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Audio output";
            // 
            // videoTabPage
            // 
            this.videoTabPage.Controls.Add(this.applyVideobtn);
            this.videoTabPage.Controls.Add(this.label4);
            this.videoTabPage.Controls.Add(this.video_settings);
            this.videoTabPage.Location = new System.Drawing.Point(4, 22);
            this.videoTabPage.Name = "videoTabPage";
            this.videoTabPage.Size = new System.Drawing.Size(519, 541);
            this.videoTabPage.TabIndex = 3;
            this.videoTabPage.Text = "Video";
            this.videoTabPage.UseVisualStyleBackColor = true;
            // 
            // applyVideobtn
            // 
            this.applyVideobtn.Location = new System.Drawing.Point(438, 502);
            this.applyVideobtn.Name = "applyVideobtn";
            this.applyVideobtn.Size = new System.Drawing.Size(75, 23);
            this.applyVideobtn.TabIndex = 14;
            this.applyVideobtn.Text = "Apply";
            this.applyVideobtn.UseVisualStyleBackColor = true;
            this.applyVideobtn.Click += new System.EventHandler(this.applyVideobtn_Click);
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(5, 494);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(510, 3);
            this.label4.TabIndex = 13;
            // 
            // video_settings
            // 
            this.video_settings.Controls.Add(this.label5);
            this.video_settings.Controls.Add(this.prevbtn);
            this.video_settings.Controls.Add(this.nextBtn);
            this.video_settings.Controls.Add(this.panel1);
            this.video_settings.Controls.Add(this.panelPlayer);
            this.video_settings.Location = new System.Drawing.Point(6, 7);
            this.video_settings.Name = "video_settings";
            this.video_settings.Size = new System.Drawing.Size(507, 335);
            this.video_settings.TabIndex = 8;
            this.video_settings.TabStop = false;
            this.video_settings.Text = "Video setting";
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(244, 299);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(3, 30);
            this.label5.TabIndex = 18;
            // 
            // prevbtn
            // 
            this.prevbtn.BackgroundImage = global::bossdoyKaraoke_NOW.Properties.Resources.ic_skip_previous_black_24dp_1x;
            this.prevbtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.prevbtn.Location = new System.Drawing.Point(212, 299);
            this.prevbtn.Name = "prevbtn";
            this.prevbtn.Size = new System.Drawing.Size(26, 30);
            this.prevbtn.TabIndex = 17;
            this.prevbtn.UseVisualStyleBackColor = true;
            this.prevbtn.Click += new System.EventHandler(this.prevbtn_Click);
            // 
            // nextBtn
            // 
            this.nextBtn.BackgroundImage = global::bossdoyKaraoke_NOW.Properties.Resources.ic_skip_next_black_24dp_1x;
            this.nextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.nextBtn.Location = new System.Drawing.Point(253, 299);
            this.nextBtn.Name = "nextBtn";
            this.nextBtn.Size = new System.Drawing.Size(26, 30);
            this.nextBtn.TabIndex = 16;
            this.nextBtn.UseVisualStyleBackColor = true;
            this.nextBtn.Click += new System.EventHandler(this.nextBtn_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.selectVideobtn);
            this.panel1.Controls.Add(this.videoLbl);
            this.panel1.Location = new System.Drawing.Point(6, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(496, 50);
            this.panel1.TabIndex = 15;
            // 
            // selectVideobtn
            // 
            this.selectVideobtn.Location = new System.Drawing.Point(414, 13);
            this.selectVideobtn.Name = "selectVideobtn";
            this.selectVideobtn.Size = new System.Drawing.Size(75, 23);
            this.selectVideobtn.TabIndex = 8;
            this.selectVideobtn.Text = "Select Video";
            this.selectVideobtn.UseVisualStyleBackColor = true;
            this.selectVideobtn.Click += new System.EventHandler(this.selectVideobtn_Click);
            // 
            // videoLbl
            // 
            this.videoLbl.AutoEllipsis = true;
            this.videoLbl.Location = new System.Drawing.Point(3, 18);
            this.videoLbl.Name = "videoLbl";
            this.videoLbl.Size = new System.Drawing.Size(405, 13);
            this.videoLbl.TabIndex = 7;
            this.videoLbl.Text = "Video";
            // 
            // panelPlayer
            // 
            this.panelPlayer.BackColor = System.Drawing.Color.Black;
            this.panelPlayer.Location = new System.Drawing.Point(6, 75);
            this.panelPlayer.Name = "panelPlayer";
            this.panelPlayer.Size = new System.Drawing.Size(496, 218);
            this.panelPlayer.TabIndex = 9;
            // 
            // micTabPage
            // 
            this.micTabPage.Controls.Add(this.groupMicrophoneEffects);
            this.micTabPage.Controls.Add(this.microphone_setting);
            this.micTabPage.Location = new System.Drawing.Point(4, 22);
            this.micTabPage.Name = "micTabPage";
            this.micTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.micTabPage.Size = new System.Drawing.Size(519, 541);
            this.micTabPage.TabIndex = 1;
            this.micTabPage.Text = "Mic Settings";
            this.micTabPage.UseVisualStyleBackColor = true;
            // 
            // groupMicrophoneEffects
            // 
            this.groupMicrophoneEffects.Controls.Add(this.channel1panel);
            this.groupMicrophoneEffects.Controls.Add(this.EQ4_0_Phone);
            this.groupMicrophoneEffects.Controls.Add(this.channel4panel);
            this.groupMicrophoneEffects.Controls.Add(this.channel3panel);
            this.groupMicrophoneEffects.Controls.Add(this.channel2panel);
            this.groupMicrophoneEffects.Controls.Add(this.DeEsser_1);
            this.groupMicrophoneEffects.Controls.Add(this.EQ4_1);
            this.groupMicrophoneEffects.Controls.Add(this.Compressor_1);
            this.groupMicrophoneEffects.Controls.Add(this.EQ1_1);
            this.groupMicrophoneEffects.Controls.Add(this.DeEsser_2);
            this.groupMicrophoneEffects.Controls.Add(this.EQ4_2);
            this.groupMicrophoneEffects.Controls.Add(this.Compressor_2);
            this.groupMicrophoneEffects.Controls.Add(this.EQ1_2);
            this.groupMicrophoneEffects.Controls.Add(this.Reverb);
            this.groupMicrophoneEffects.Controls.Add(this.DeEsser_0);
            this.groupMicrophoneEffects.Controls.Add(this.EQ4_0);
            this.groupMicrophoneEffects.Controls.Add(this.Compressor_0);
            this.groupMicrophoneEffects.Controls.Add(this.EQ1_0);
            this.groupMicrophoneEffects.Location = new System.Drawing.Point(6, 67);
            this.groupMicrophoneEffects.Name = "groupMicrophoneEffects";
            this.groupMicrophoneEffects.Size = new System.Drawing.Size(507, 468);
            this.groupMicrophoneEffects.TabIndex = 8;
            this.groupMicrophoneEffects.TabStop = false;
            this.groupMicrophoneEffects.Text = "Microphone effects";
            // 
            // channel1panel
            // 
            this.channel1panel.Location = new System.Drawing.Point(6, 20);
            this.channel1panel.Name = "channel1panel";
            this.channel1panel.Size = new System.Drawing.Size(74, 434);
            this.channel1panel.TabIndex = 77;
            // 
            // EQ4_0_Phone
            // 
            this.EQ4_0_Phone.Location = new System.Drawing.Point(74, 97);
            this.EQ4_0_Phone.Name = "EQ4_0_Phone";
            this.EQ4_0_Phone.Size = new System.Drawing.Size(63, 19);
            this.EQ4_0_Phone.TabIndex = 76;
            this.EQ4_0_Phone.Text = "EQ4_P";
            this.EQ4_0_Phone.UseVisualStyleBackColor = true;
            this.EQ4_0_Phone.Click += new System.EventHandler(this.EQ4_0_Phone_Click);
            // 
            // channel4panel
            // 
            this.channel4panel.Location = new System.Drawing.Point(384, 20);
            this.channel4panel.Name = "channel4panel";
            this.channel4panel.Size = new System.Drawing.Size(74, 434);
            this.channel4panel.TabIndex = 65;
            // 
            // channel3panel
            // 
            this.channel3panel.Location = new System.Drawing.Point(261, 20);
            this.channel3panel.Name = "channel3panel";
            this.channel3panel.Size = new System.Drawing.Size(74, 434);
            this.channel3panel.TabIndex = 64;
            // 
            // channel2panel
            // 
            this.channel2panel.Location = new System.Drawing.Point(139, 22);
            this.channel2panel.Name = "channel2panel";
            this.channel2panel.Size = new System.Drawing.Size(74, 434);
            this.channel2panel.TabIndex = 63;
            // 
            // DeEsser_1
            // 
            this.DeEsser_1.Location = new System.Drawing.Point(209, 97);
            this.DeEsser_1.Name = "DeEsser_1";
            this.DeEsser_1.Size = new System.Drawing.Size(46, 19);
            this.DeEsser_1.TabIndex = 70;
            this.DeEsser_1.Text = "D-Esser";
            this.DeEsser_1.UseVisualStyleBackColor = true;
            this.DeEsser_1.Click += new System.EventHandler(this.DeEsser_1_Click);
            // 
            // EQ4_1
            // 
            this.EQ4_1.Location = new System.Drawing.Point(209, 72);
            this.EQ4_1.Name = "EQ4_1";
            this.EQ4_1.Size = new System.Drawing.Size(46, 19);
            this.EQ4_1.TabIndex = 69;
            this.EQ4_1.Text = "EQ4";
            this.EQ4_1.UseVisualStyleBackColor = true;
            this.EQ4_1.Click += new System.EventHandler(this.EQ4_1_Click);
            // 
            // Compressor_1
            // 
            this.Compressor_1.Location = new System.Drawing.Point(209, 47);
            this.Compressor_1.Name = "Compressor_1";
            this.Compressor_1.Size = new System.Drawing.Size(46, 19);
            this.Compressor_1.TabIndex = 68;
            this.Compressor_1.Text = "COMPRSSOR";
            this.Compressor_1.UseVisualStyleBackColor = true;
            this.Compressor_1.Click += new System.EventHandler(this.Compressor_1_Click);
            // 
            // EQ1_1
            // 
            this.EQ1_1.Location = new System.Drawing.Point(209, 22);
            this.EQ1_1.Name = "EQ1_1";
            this.EQ1_1.Size = new System.Drawing.Size(46, 19);
            this.EQ1_1.TabIndex = 67;
            this.EQ1_1.Text = "EQ1";
            this.EQ1_1.UseVisualStyleBackColor = true;
            this.EQ1_1.Click += new System.EventHandler(this.EQ1_1_Click);
            // 
            // DeEsser_2
            // 
            this.DeEsser_2.Location = new System.Drawing.Point(332, 95);
            this.DeEsser_2.Name = "DeEsser_2";
            this.DeEsser_2.Size = new System.Drawing.Size(46, 19);
            this.DeEsser_2.TabIndex = 74;
            this.DeEsser_2.Text = "D-Esser";
            this.DeEsser_2.UseVisualStyleBackColor = true;
            this.DeEsser_2.Click += new System.EventHandler(this.DeEsser_2_Click);
            // 
            // EQ4_2
            // 
            this.EQ4_2.Location = new System.Drawing.Point(332, 70);
            this.EQ4_2.Name = "EQ4_2";
            this.EQ4_2.Size = new System.Drawing.Size(46, 19);
            this.EQ4_2.TabIndex = 73;
            this.EQ4_2.Text = "EQ4";
            this.EQ4_2.UseVisualStyleBackColor = true;
            this.EQ4_2.Click += new System.EventHandler(this.EQ4_2_Click);
            // 
            // Compressor_2
            // 
            this.Compressor_2.Location = new System.Drawing.Point(332, 45);
            this.Compressor_2.Name = "Compressor_2";
            this.Compressor_2.Size = new System.Drawing.Size(46, 19);
            this.Compressor_2.TabIndex = 72;
            this.Compressor_2.Text = "COMPRSSOR";
            this.Compressor_2.UseVisualStyleBackColor = true;
            this.Compressor_2.Click += new System.EventHandler(this.Compressor_2_Click);
            // 
            // EQ1_2
            // 
            this.EQ1_2.Location = new System.Drawing.Point(332, 20);
            this.EQ1_2.Name = "EQ1_2";
            this.EQ1_2.Size = new System.Drawing.Size(46, 19);
            this.EQ1_2.TabIndex = 71;
            this.EQ1_2.Text = "EQ1";
            this.EQ1_2.UseVisualStyleBackColor = true;
            this.EQ1_2.Click += new System.EventHandler(this.EQ1_2_Click);
            // 
            // Reverb
            // 
            this.Reverb.Location = new System.Drawing.Point(78, 147);
            this.Reverb.Name = "Reverb";
            this.Reverb.Size = new System.Drawing.Size(59, 19);
            this.Reverb.TabIndex = 66;
            this.Reverb.Text = "REVERB";
            this.Reverb.UseVisualStyleBackColor = true;
            this.Reverb.Click += new System.EventHandler(this.Reverb_Click);
            // 
            // DeEsser_0
            // 
            this.DeEsser_0.Location = new System.Drawing.Point(76, 122);
            this.DeEsser_0.Name = "DeEsser_0";
            this.DeEsser_0.Size = new System.Drawing.Size(46, 19);
            this.DeEsser_0.TabIndex = 50;
            this.DeEsser_0.Text = "D-Esser";
            this.DeEsser_0.UseVisualStyleBackColor = true;
            this.DeEsser_0.Click += new System.EventHandler(this.DeEsser_0_Click);
            // 
            // EQ4_0
            // 
            this.EQ4_0.Location = new System.Drawing.Point(76, 72);
            this.EQ4_0.Name = "EQ4_0";
            this.EQ4_0.Size = new System.Drawing.Size(46, 19);
            this.EQ4_0.TabIndex = 49;
            this.EQ4_0.Text = "EQ4";
            this.EQ4_0.UseVisualStyleBackColor = true;
            this.EQ4_0.Click += new System.EventHandler(this.EQ4_0_Click);
            // 
            // Compressor_0
            // 
            this.Compressor_0.Location = new System.Drawing.Point(76, 47);
            this.Compressor_0.Name = "Compressor_0";
            this.Compressor_0.Size = new System.Drawing.Size(46, 19);
            this.Compressor_0.TabIndex = 48;
            this.Compressor_0.Text = "COMPRSSOR";
            this.Compressor_0.UseVisualStyleBackColor = true;
            this.Compressor_0.Click += new System.EventHandler(this.Compressor_0_Click);
            // 
            // EQ1_0
            // 
            this.EQ1_0.Location = new System.Drawing.Point(76, 22);
            this.EQ1_0.Name = "EQ1_0";
            this.EQ1_0.Size = new System.Drawing.Size(46, 19);
            this.EQ1_0.TabIndex = 47;
            this.EQ1_0.Text = "EQ1";
            this.EQ1_0.UseVisualStyleBackColor = true;
            this.EQ1_0.Click += new System.EventHandler(this.EQ1_0_Click);
            // 
            // microphone_setting
            // 
            this.microphone_setting.Controls.Add(this.comboBoxInputDevice);
            this.microphone_setting.Controls.Add(this.label1);
            this.microphone_setting.Location = new System.Drawing.Point(6, 7);
            this.microphone_setting.Name = "microphone_setting";
            this.microphone_setting.Size = new System.Drawing.Size(507, 55);
            this.microphone_setting.TabIndex = 7;
            this.microphone_setting.TabStop = false;
            this.microphone_setting.Text = "Microphone setting";
            // 
            // comboBoxInputDevice
            // 
            this.comboBoxInputDevice.FormattingEnabled = true;
            this.comboBoxInputDevice.Location = new System.Drawing.Point(120, 19);
            this.comboBoxInputDevice.Name = "comboBoxInputDevice";
            this.comboBoxInputDevice.Size = new System.Drawing.Size(372, 21);
            this.comboBoxInputDevice.TabIndex = 6;
            this.comboBoxInputDevice.SelectedIndexChanged += new System.EventHandler(this.comboBoxInputDevice_SelectedIndexChanged);
            this.comboBoxInputDevice.Click += new System.EventHandler(this.comboBoxInputDevice_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Microphone input";
            // 
            // Preferences
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 576);
            this.Controls.Add(this.PrefTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(552, 614);
            this.MinimumSize = new System.Drawing.Size(552, 614);
            this.Name = "Preferences";
            this.Text = "Preferences";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Preferences_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Preferences_FormClosed);
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.PrefTabControl.ResumeLayout(false);
            this.generalTabPage.ResumeLayout(false);
            this.audio_setting.ResumeLayout(false);
            this.audio_setting.PerformLayout();
            this.audioTabPage.ResumeLayout(false);
            this.audio_device.ResumeLayout(false);
            this.audio_device.PerformLayout();
            this.videoTabPage.ResumeLayout(false);
            this.video_settings.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.micTabPage.ResumeLayout(false);
            this.groupMicrophoneEffects.ResumeLayout(false);
            this.microphone_setting.ResumeLayout(false);
            this.microphone_setting.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl PrefTabControl;
        private System.Windows.Forms.TabPage generalTabPage;
        private System.Windows.Forms.TabPage micTabPage;
        private System.Windows.Forms.GroupBox audio_setting;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxOutputDevice;
        private System.Windows.Forms.GroupBox microphone_setting;
        private System.Windows.Forms.ComboBox comboBoxInputDevice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupMicrophoneEffects;
        public System.Windows.Forms.Panel channel4panel;
        public System.Windows.Forms.Panel channel3panel;
        public System.Windows.Forms.Panel channel2panel;
        private System.Windows.Forms.Button DeEsser_1;
        private System.Windows.Forms.Button EQ4_1;
        private System.Windows.Forms.Button Compressor_1;
        private System.Windows.Forms.Button EQ1_1;
        private System.Windows.Forms.Button DeEsser_2;
        private System.Windows.Forms.Button EQ4_2;
        private System.Windows.Forms.Button Compressor_2;
        private System.Windows.Forms.Button EQ1_2;
        private System.Windows.Forms.Button Reverb;
        private System.Windows.Forms.Button DeEsser_0;
        private System.Windows.Forms.Button EQ4_0;
        private System.Windows.Forms.Button Compressor_0;
        private System.Windows.Forms.Button EQ1_0;
        public System.Windows.Forms.Panel channel1panel;
        private System.Windows.Forms.Button EQ4_0_Phone;
        private System.Windows.Forms.TabPage audioTabPage;
        private System.Windows.Forms.GroupBox audio_device;
        private System.Windows.Forms.RadioButton radioBtnWasapi;
        private System.Windows.Forms.CheckBox chkBoxRefreshAsio;
        private System.Windows.Forms.Button asio_control_btn;
        private System.Windows.Forms.RadioButton radioBtnAsio;
        private System.Windows.Forms.RadioButton radioBtnBass;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TabPage videoTabPage;
        private System.Windows.Forms.GroupBox video_settings;
        private System.Windows.Forms.Label videoLbl;
        private System.Windows.Forms.Panel panelPlayer;
        private System.Windows.Forms.Button selectVideobtn;
        private System.Windows.Forms.Button applyVideobtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button prevbtn;
        private System.Windows.Forms.Button nextBtn;
        private System.Windows.Forms.Label label5;
    }
}