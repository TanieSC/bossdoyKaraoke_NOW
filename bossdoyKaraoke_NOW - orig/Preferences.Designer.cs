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
            this.audio_device = new System.Windows.Forms.GroupBox();
            this.radioBtnWasapi = new System.Windows.Forms.RadioButton();
            this.chkBoxRefreshAsio = new System.Windows.Forms.CheckBox();
            this.asio_control_btn = new System.Windows.Forms.Button();
            this.radioBtnAsio = new System.Windows.Forms.RadioButton();
            this.radioBtnBass = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.audio_setting = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxOutputDevice = new System.Windows.Forms.ComboBox();
            this.micTabPage = new System.Windows.Forms.TabPage();
            this.groupMicrophoneEffects = new System.Windows.Forms.GroupBox();
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
            this.channel1panel = new System.Windows.Forms.Panel();
            this.DeEsser_0 = new System.Windows.Forms.Button();
            this.EQ4_0 = new System.Windows.Forms.Button();
            this.Compressor_0 = new System.Windows.Forms.Button();
            this.EQ1_0 = new System.Windows.Forms.Button();
            this.microphone_setting = new System.Windows.Forms.GroupBox();
            this.comboBoxInputDevice = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.PrefTabControl.SuspendLayout();
            this.generalTabPage.SuspendLayout();
            this.audio_device.SuspendLayout();
            this.audio_setting.SuspendLayout();
            this.micTabPage.SuspendLayout();
            this.groupMicrophoneEffects.SuspendLayout();
            this.microphone_setting.SuspendLayout();
            this.SuspendLayout();
            // 
            // PrefTabControl
            // 
            this.PrefTabControl.Controls.Add(this.generalTabPage);
            this.PrefTabControl.Controls.Add(this.micTabPage);
            this.PrefTabControl.Location = new System.Drawing.Point(12, 7);
            this.PrefTabControl.Name = "PrefTabControl";
            this.PrefTabControl.SelectedIndex = 0;
            this.PrefTabControl.Size = new System.Drawing.Size(527, 567);
            this.PrefTabControl.TabIndex = 0;
            // 
            // generalTabPage
            // 
            this.generalTabPage.Controls.Add(this.audio_device);
            this.generalTabPage.Controls.Add(this.audio_setting);
            this.generalTabPage.Location = new System.Drawing.Point(4, 22);
            this.generalTabPage.Name = "generalTabPage";
            this.generalTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.generalTabPage.Size = new System.Drawing.Size(519, 541);
            this.generalTabPage.TabIndex = 0;
            this.generalTabPage.Text = "General";
            this.generalTabPage.UseVisualStyleBackColor = true;
            // 
            // audio_device
            // 
            this.audio_device.Controls.Add(this.radioBtnWasapi);
            this.audio_device.Controls.Add(this.chkBoxRefreshAsio);
            this.audio_device.Controls.Add(this.asio_control_btn);
            this.audio_device.Controls.Add(this.radioBtnAsio);
            this.audio_device.Controls.Add(this.radioBtnBass);
            this.audio_device.Controls.Add(this.label3);
            this.audio_device.Location = new System.Drawing.Point(6, 6);
            this.audio_device.Name = "audio_device";
            this.audio_device.Size = new System.Drawing.Size(507, 98);
            this.audio_device.TabIndex = 1;
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
            // audio_setting
            // 
            this.audio_setting.Controls.Add(this.label2);
            this.audio_setting.Controls.Add(this.comboBoxOutputDevice);
            this.audio_setting.Location = new System.Drawing.Point(6, 133);
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
            this.groupMicrophoneEffects.Controls.Add(this.channel1panel);
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
            // channel4panel
            // 
            this.channel4panel.Location = new System.Drawing.Point(373, 20);
            this.channel4panel.Name = "channel4panel";
            this.channel4panel.Size = new System.Drawing.Size(74, 434);
            this.channel4panel.TabIndex = 65;
            // 
            // channel3panel
            // 
            this.channel3panel.Location = new System.Drawing.Point(250, 20);
            this.channel3panel.Name = "channel3panel";
            this.channel3panel.Size = new System.Drawing.Size(74, 434);
            this.channel3panel.TabIndex = 64;
            // 
            // channel2panel
            // 
            this.channel2panel.Location = new System.Drawing.Point(128, 22);
            this.channel2panel.Name = "channel2panel";
            this.channel2panel.Size = new System.Drawing.Size(74, 434);
            this.channel2panel.TabIndex = 63;
            // 
            // DeEsser_1
            // 
            this.DeEsser_1.Location = new System.Drawing.Point(198, 97);
            this.DeEsser_1.Name = "DeEsser_1";
            this.DeEsser_1.Size = new System.Drawing.Size(46, 19);
            this.DeEsser_1.TabIndex = 70;
            this.DeEsser_1.Text = "D-Esser";
            this.DeEsser_1.UseVisualStyleBackColor = true;
            this.DeEsser_1.Click += new System.EventHandler(this.DeEsser_1_Click);
            // 
            // EQ4_1
            // 
            this.EQ4_1.Location = new System.Drawing.Point(198, 72);
            this.EQ4_1.Name = "EQ4_1";
            this.EQ4_1.Size = new System.Drawing.Size(46, 19);
            this.EQ4_1.TabIndex = 69;
            this.EQ4_1.Text = "EQ4";
            this.EQ4_1.UseVisualStyleBackColor = true;
            this.EQ4_1.Click += new System.EventHandler(this.EQ4_1_Click);
            // 
            // Compressor_1
            // 
            this.Compressor_1.Location = new System.Drawing.Point(198, 47);
            this.Compressor_1.Name = "Compressor_1";
            this.Compressor_1.Size = new System.Drawing.Size(46, 19);
            this.Compressor_1.TabIndex = 68;
            this.Compressor_1.Text = "COMPRSSOR";
            this.Compressor_1.UseVisualStyleBackColor = true;
            this.Compressor_1.Click += new System.EventHandler(this.Compressor_1_Click);
            // 
            // EQ1_1
            // 
            this.EQ1_1.Location = new System.Drawing.Point(198, 22);
            this.EQ1_1.Name = "EQ1_1";
            this.EQ1_1.Size = new System.Drawing.Size(46, 19);
            this.EQ1_1.TabIndex = 67;
            this.EQ1_1.Text = "EQ1";
            this.EQ1_1.UseVisualStyleBackColor = true;
            this.EQ1_1.Click += new System.EventHandler(this.EQ1_1_Click);
            // 
            // DeEsser_2
            // 
            this.DeEsser_2.Location = new System.Drawing.Point(321, 95);
            this.DeEsser_2.Name = "DeEsser_2";
            this.DeEsser_2.Size = new System.Drawing.Size(46, 19);
            this.DeEsser_2.TabIndex = 74;
            this.DeEsser_2.Text = "D-Esser";
            this.DeEsser_2.UseVisualStyleBackColor = true;
            this.DeEsser_2.Click += new System.EventHandler(this.DeEsser_2_Click);
            // 
            // EQ4_2
            // 
            this.EQ4_2.Location = new System.Drawing.Point(321, 70);
            this.EQ4_2.Name = "EQ4_2";
            this.EQ4_2.Size = new System.Drawing.Size(46, 19);
            this.EQ4_2.TabIndex = 73;
            this.EQ4_2.Text = "EQ4";
            this.EQ4_2.UseVisualStyleBackColor = true;
            this.EQ4_2.Click += new System.EventHandler(this.EQ4_2_Click);
            // 
            // Compressor_2
            // 
            this.Compressor_2.Location = new System.Drawing.Point(321, 45);
            this.Compressor_2.Name = "Compressor_2";
            this.Compressor_2.Size = new System.Drawing.Size(46, 19);
            this.Compressor_2.TabIndex = 72;
            this.Compressor_2.Text = "COMPRSSOR";
            this.Compressor_2.UseVisualStyleBackColor = true;
            this.Compressor_2.Click += new System.EventHandler(this.Compressor_2_Click);
            // 
            // EQ1_2
            // 
            this.EQ1_2.Location = new System.Drawing.Point(321, 20);
            this.EQ1_2.Name = "EQ1_2";
            this.EQ1_2.Size = new System.Drawing.Size(46, 19);
            this.EQ1_2.TabIndex = 71;
            this.EQ1_2.Text = "EQ1";
            this.EQ1_2.UseVisualStyleBackColor = true;
            this.EQ1_2.Click += new System.EventHandler(this.EQ1_2_Click);
            // 
            // Reverb
            // 
            this.Reverb.Location = new System.Drawing.Point(444, 20);
            this.Reverb.Name = "Reverb";
            this.Reverb.Size = new System.Drawing.Size(59, 19);
            this.Reverb.TabIndex = 66;
            this.Reverb.Text = "REVERB";
            this.Reverb.UseVisualStyleBackColor = true;
            this.Reverb.Click += new System.EventHandler(this.Reverb_Click);
            // 
            // channel1panel
            // 
            this.channel1panel.Location = new System.Drawing.Point(6, 22);
            this.channel1panel.Name = "channel1panel";
            this.channel1panel.Size = new System.Drawing.Size(74, 434);
            this.channel1panel.TabIndex = 46;
            // 
            // DeEsser_0
            // 
            this.DeEsser_0.Location = new System.Drawing.Point(76, 97);
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
            this.microphone_setting.Location = new System.Drawing.Point(6, 6);
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
            this.ClientSize = new System.Drawing.Size(550, 586);
            this.Controls.Add(this.PrefTabControl);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximumSize = new System.Drawing.Size(566, 624);
            this.MinimumSize = new System.Drawing.Size(566, 624);
            this.Name = "Preferences";
            this.Text = "Preferences";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Preferences_FormClosing);
            this.Load += new System.EventHandler(this.Preferences_Load);
            this.PrefTabControl.ResumeLayout(false);
            this.generalTabPage.ResumeLayout(false);
            this.audio_device.ResumeLayout(false);
            this.audio_device.PerformLayout();
            this.audio_setting.ResumeLayout(false);
            this.audio_setting.PerformLayout();
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
        public System.Windows.Forms.Panel channel1panel;
        private System.Windows.Forms.Button DeEsser_0;
        private System.Windows.Forms.Button EQ4_0;
        private System.Windows.Forms.Button Compressor_0;
        private System.Windows.Forms.Button EQ1_0;
        private System.Windows.Forms.GroupBox audio_device;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioBtnAsio;
        private System.Windows.Forms.RadioButton radioBtnBass;
        private System.Windows.Forms.Button asio_control_btn;
        private System.Windows.Forms.CheckBox chkBoxRefreshAsio;
        private System.Windows.Forms.RadioButton radioBtnWasapi;
    }
}