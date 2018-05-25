using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using bossdoyKaraoke_NOW.Enums;
using bossdoyKaraoke_NOW.Properties;
using Un4seen.Bass.AddOn.Vst;

namespace bossdoyKaraoke_NOW.Models.VocalEffects
{
    public class Channel1Fx
    {
        private static VSTPROC m_VstProc;
        private static float m_paramValue;
        private static float m_paramValueChan3;
        private static float m_stripParamValue;
        private static float m_stripParamDefaultValue;
        private static float m_stripParamNewValue;
        private static float m_chan3DefaultValue;
        private static float m_chan3NewtValue;
        private static float m_defChan3;

        static EffectsInteface _effectsInteface;

        static string m_vst_plugin_Path = AppDomain.CurrentDomain.BaseDirectory + @"vst_plugins\";
        static string m_vst_plugin_PathX64 = AppDomain.CurrentDomain.BaseDirectory + @"vst_plugins\x64\";
        private static int _vstEffectEQ1band;
        private static int _vstEffectCompressor;
        private static int _vstEffectEQ4Band;
        private static int _vstEffectDeEsser;
        private static int _vstBasicStrip;

        public static void SetEffects(int stream)
        {

            string plugin_path = "";
            if (EffectsInteface.Is64Bit())
                plugin_path = m_vst_plugin_PathX64;
            else
                plugin_path = m_vst_plugin_Path;

            //EQ 1 Band -----------------
            _vstEffectEQ1band = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"TDR Nova_0_0.dll", BASSVSTDsp.BASS_VST_DEFAULT, 0);

            int eq1 = BassVst.BASS_VST_GetParamCount(_vstEffectEQ1band);

            for (int i = 0; i < eq1; i++)
            {
                BassVst.BASS_VST_SetParam(_vstEffectEQ1band, i, AppSettings.Get<float>("CH1EQ1band" + i));
            }

                /*  for (int i = 0; i < eq1; i++)
                  {
                      // get the info about the parameter
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 0, 0);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 1, 1);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 4, 0.260f);
                      //  BassVst.BASS_VST_SetParam(_vstEffectEQ1band[0], 5, 1);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 12, 0);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 13, 0);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 24, 0);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 25, 0);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 36, 0);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 37, 0);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 48, 1);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 49, 1);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 50, 0.260f);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 51, 0.6666667f);
                      BassVst.BASS_VST_SetParam(_vstEffectEQ1band, 69, 0.0f);
                }*/
                //EQ1Band();

                try
            {
                //Compressor
                _vstEffectCompressor = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"MCompressor_0.dll", BASSVSTDsp.BASS_VST_DEFAULT, 1);

                int comp1 = BassVst.BASS_VST_GetParamCount(_vstEffectCompressor);
                for (int i = 0; i < comp1; i++)
                {
                    BassVst.BASS_VST_SetParam(_vstEffectCompressor, i, AppSettings.Get<float>("CH1Comp" + i));
                }
                    /* for (int i = 0; i < comp1; i++)
                     {
                         // get the info about the parameter
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 0, 0.5f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 1, 0.5f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 2, 0.3162278f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 3, 0.3162278f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 4, 0.1f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 5, 0.3444445f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 6, 0.2357953f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 7, 1);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 8, 0.25f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 9, 1);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 10, 1);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 11, 0);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 12, 0.5f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 13, 0.5f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 14, 0.5f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 15, 0.5f);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 16, 0);
                         BassVst.BASS_VST_SetParam(_vstEffectCompressor, 17, 0);
                     }*/
                    //Compressor();
                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //EQ4Band -----
            _vstEffectEQ4Band = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"TDR Nova_0_1.dll", BASSVSTDsp.BASS_VST_DEFAULT, 2);
            int eq7 = BassVst.BASS_VST_GetParamCount(_vstEffectEQ4Band);

            for (int i = 0; i < eq7; i++)
            {
                BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, i, AppSettings.Get<float>("CH1EQ4band" + i));
            }
                /* for (int i = 0; i < eq7; i++)
                 {
                     // get the info about the parameter
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 0, 1);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 1, 1);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 3, 0.563f);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 4, 0.278f);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 5, 0);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 12, 1);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 13, 1);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 15, 0.563f);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 16, 0.361f); //Band 2 F
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 24, 1);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 25, 1);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 27, 0.563f);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 28, 0.63883f); //Band 3 F
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 36, 1);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 37, 1);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 39, 0.563f);
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 40, 0.7712674f); //Band 4 F
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 41, 1); //Band 4 T
                     BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, 69, 0.0f);
                 }*/

                _vstEffectDeEsser = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"MCompressor_0_D-Es.dll", BASSVSTDsp.BASS_VST_DEFAULT, 3);

            int deEs = BassVst.BASS_VST_GetParamCount(_vstEffectDeEsser);

            for (int i = 0; i < deEs; i++)
            {
                BassVst.BASS_VST_SetParam(_vstEffectDeEsser, i, AppSettings.Get<float>("CH1DeEsser" + i));
            }
                /* for (int i = 0; i < deEs; i++)
                 {
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 0, 0.5f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 1, 0.2288729f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 2, 0.3162278f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 3, 0.4728708f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 4, 0.1f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 5, 0.1713131f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 6, 0.3978892f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 7, 1);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 8, 0.25f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 9, 1);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 10, 0);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 11, 0);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 12, 0.5f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 13, 0.5f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 14, 0.5f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 15, 0.5f);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 16, 0);
                     BassVst.BASS_VST_SetParam(_vstEffectDeEsser, 17, 0);
                 }*/

                _vstBasicStrip = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"BasicStrip0.dll", BASSVSTDsp.BASS_VST_DEFAULT, 4);

            int strip = BassVst.BASS_VST_GetParamCount(_vstBasicStrip);

            for (int i = 0; i < strip; i++)
            {
                BassVst.BASS_VST_SetParam(_vstBasicStrip, 0, 1.0f);
                BassVst.BASS_VST_SetParam(_vstBasicStrip, 1, 0.5f);
            }


            m_stripParamDefaultValue = BassVst.BASS_VST_GetParam(_vstBasicStrip, 0);
            m_VstProc = new VSTPROC(VstProc_CallBack);
            BassVst.BASS_VST_SetCallback(_vstBasicStrip, m_VstProc, IntPtr.Zero);

        }

        public static void ShowInterface() {

            _effectsInteface = new EffectsInteface();

            switch (Effects.GetorSetFx) {
                case Effects.Load.EQ1_0:
                    _effectsInteface.VstEffectHandle = _vstEffectEQ1band;
                    _effectsInteface.Show();
                    break;
                case Effects.Load.COMPRESSOR1_0:
                    _effectsInteface.VstEffectHandle = _vstEffectCompressor;
                    _effectsInteface.Show();
                    break;
                case Effects.Load.EQ7_0:
                    _effectsInteface.VstEffectHandle = _vstEffectEQ4Band;
                    _effectsInteface.Show();
                    break;
                case Effects.Load.DeEsser0_0:
                    _effectsInteface.VstEffectHandle = _vstEffectDeEsser;
                    _effectsInteface.Show();
                    break;
                case Effects.Load.CHANNETSTRIP:
                    _effectsInteface.VstEffectHandle = _vstBasicStrip;
                    _effectsInteface.LoadChannelStrips(PlayerControl.PrefsForm, PlayerControl.PrefsForm.channel1panel, 1, 434);
                    break;
            }            
            
        }

        // your VST callback - referenced below by BASS_VST_SetCallback()
        private static int VstProc_CallBack(int vstHandle, BASSVSTAction action, int param1, int param2, IntPtr user)
        { //b = a < 0 ? a*-1 : a*1 ;
            switch (action)
            {
                case BASSVSTAction.BASS_VST_PARAM_CHANGED:
                    // we get notified that the user has changed some sliders in the editor - 
                    // do what to do here ...
                    BassVst.BASS_VST_SetParamCopyParams(_vstBasicStrip, Channel2Fx.GetChannelStrip);
                    break;
                case BASSVSTAction.BASS_VST_EDITOR_RESIZED:
                    // the editor window requests a new size,
                    // maybe we should resize the window the editor is embedded in?
                    // the new width/height can be found in param1/param2
                    break;
                case BASSVSTAction.BASS_VST_AUDIO_MASTER:
                    // this is only for people familiar with the VST SDK,
                    // param1 is a pointer to a BASS_VST_AUDIO_MASTER_PARAM structure
                    // which contains all information needed
                    break;
            }
             return 0;
        }

        public static int GetChannelStrip { get { return _vstBasicStrip; } }
        public static float GetStripParamValue { get; set; }
    }
}
