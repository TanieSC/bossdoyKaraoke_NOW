namespace bossdoyKaraoke_NOW
{
    partial class FullScreenPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FullScreenPlayer));
            this.panelPlayer = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelPlayer
            // 
            this.panelPlayer.BackColor = System.Drawing.Color.Black;
            this.panelPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPlayer.Location = new System.Drawing.Point(0, 0);
            this.panelPlayer.Name = "panelPlayer";
            this.panelPlayer.Size = new System.Drawing.Size(668, 453);
            this.panelPlayer.TabIndex = 0;
            this.panelPlayer.DoubleClick += new System.EventHandler(this.panelPlayer_DoubleClick);
            // 
            // FullScreenPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 453);
            this.Controls.Add(this.panelPlayer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(668, 453);
            this.Name = "FullScreenPlayer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FullScreenPlayer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FullScreenPlayer_FormClosing);
            this.Load += new System.EventHandler(this.FullScreenPlayer_Load);
            this.SizeChanged += new System.EventHandler(this.FullScreenPlayer_SizeChanged);
            this.DoubleClick += new System.EventHandler(this.FullScreenPlayer_DoubleClick);
            this.Resize += new System.EventHandler(this.FullScreenPlayer_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panelPlayer;
    }
}