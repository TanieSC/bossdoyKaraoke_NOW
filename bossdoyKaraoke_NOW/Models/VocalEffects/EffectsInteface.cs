using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Enums;
using Un4seen.Bass.AddOn.Vst;

namespace bossdoyKaraoke_NOW.Models.VocalEffects
{
    public class EffectsInteface
    {
        private float _paramValue;
        private BASS_VST_PARAM_INFO _paramInfo;
        private BASS_VST_INFO _vstInfo;

        private static  int _vstEffectHandle = 0;


        public static bool Is64Bit()
        {
            bool is64Bit = false;

            if (Un4seen.Bass.Utils.Is64Bit)
                is64Bit = true;
            else
                is64Bit = false;

            return is64Bit;
        }

        public void Show( int width = 0, int height = 0)
        {

           // if (_vstEffectHandle != 0)
           // {
          //      return;
           // }

            _paramInfo = new BASS_VST_PARAM_INFO();
            int c = BassVst.BASS_VST_GetParamCount(VstEffectHandle);

            for (int i = 0; i < c; i++)
            {
                // get the info about the parameter
                _paramValue = BassVst.BASS_VST_GetParam(VstEffectHandle, i);
                BassVst.BASS_VST_GetParamInfo(VstEffectHandle, i, _paramInfo);
                _paramValue = BassVst.BASS_VST_GetParam(VstEffectHandle, i);
                Console.WriteLine(_paramInfo.ToString() + " : " + _paramValue.ToString());
            }

            // show the embedded editor
            _vstInfo = new BASS_VST_INFO();
            if (BassVst.BASS_VST_GetInfo(VstEffectHandle, _vstInfo) && _vstInfo.hasEditor)
            {
                // create a new System.Windows.Forms.Form
                Form f = new Form();
                if (width != 0 && height != 0)
                    f.ClientSize = new Size(width, height);
                else
                    f.ClientSize = new Size(_vstInfo.editorWidth, _vstInfo.editorHeight);

                f.MaximizeBox = false;
                f.MinimizeBox = false;
                f.FormBorderStyle = FormBorderStyle.FixedSingle;
                f.Closing += new CancelEventHandler(f_Closing);
                f.Text = _vstInfo.effectName + " " + Effects.GetorSetFx;
                f.Show();

                BassVst.BASS_VST_EmbedEditor(VstEffectHandle, f.Handle);

                _vstEffectHandle = VstEffectHandle;
            }
        }

        public void LoadChannelStrips(Preferences form, Panel prefsPanel, int width = 0, int height = 0) {

            _paramInfo = new BASS_VST_PARAM_INFO();
            int c = BassVst.BASS_VST_GetParamCount(VstEffectHandle);

            for (int i = 0; i < c; i++)
            {
                // get the info about the parameter
                _paramValue = BassVst.BASS_VST_GetParam(VstEffectHandle, i);
                BassVst.BASS_VST_GetParamInfo(VstEffectHandle, i, _paramInfo);
                _paramValue = BassVst.BASS_VST_GetParam(VstEffectHandle, i);
                Console.WriteLine(_paramInfo.ToString() + " : " + _paramValue.ToString());
            }

            // show the embedded editor
            _vstInfo = new BASS_VST_INFO();
            if (BassVst.BASS_VST_GetInfo(VstEffectHandle, _vstInfo) && _vstInfo.hasEditor)
            {
                form.Closing += new CancelEventHandler(f_Closing);
                //form.Text = _vstInfo.effectName + " " + Effects.GetorSetFx;

                BassVst.BASS_VST_EmbedEditor(VstEffectHandle, prefsPanel.Handle);

                _vstEffectHandle = VstEffectHandle;
            }
        }

       

        private void f_Closing(object sender, CancelEventArgs e)
        {
            // unembed the VST editor
            BassVst.BASS_VST_EmbedEditor(VstEffectHandle, IntPtr.Zero);
            VstEffectHandle = 0;
            _vstEffectHandle = 0;

        }

        public int VstEffectHandle { get; set; }
    }
}
