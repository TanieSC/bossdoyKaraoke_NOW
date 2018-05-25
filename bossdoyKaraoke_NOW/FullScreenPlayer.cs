using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Models;

namespace bossdoyKaraoke_NOW
{
    public partial class FullScreenPlayer : Form
    {

        public FullScreenPlayer()
        {
            InitializeComponent();
        }

        private void FullScreenPlayer_Load(object sender, EventArgs e)
        {
            AutoSizeWindow();
        }

        private void FullScreenPlayer_FormClosing(object sender, FormClosingEventArgs e)
        {
            panelPlayer = null;
            Hide();
        }

        private void FullScreenPlayer_Resize(object sender, EventArgs e)
        {
            PlayerControl.ResizeScreen2();
        }

        private void panelPlayer_DoubleClick(object sender, EventArgs e)
        {
            AutoSizeWindow();
        }

        private void FullScreenPlayer_DoubleClick(object sender, EventArgs e)
        {
            AutoSizeWindow();
        }

        private void FullScreenPlayer_SizeChanged(object sender, EventArgs e)
        {
            this.Execute(delegate
            {

                if (this.WindowState == FormWindowState.Maximized)
                {
                    // this.FormBorderStyle = FormBorderStyle.None;
                    this.TopMost = true;
                }
                else
                {
                    // this.FormBorderStyle = FormBorderStyle.Sizable;
                    this.TopMost = false;
                }
            });
        }

        private void AutoSizeWindow()
        {
            this.Execute(delegate
            {
                if (this.WindowState == FormWindowState.Normal)
                {
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.WindowState = FormWindowState.Maximized;
                    this.TopMost = true;
                    this.Refresh();
                }
                else
                {
                    this.FormBorderStyle = FormBorderStyle.Sizable;
                    this.WindowState = FormWindowState.Normal;
                    this.TopMost = false;
                    this.Size = new Size(684, 491);
                    this.Refresh();
                }
            });

        }
    }
}
