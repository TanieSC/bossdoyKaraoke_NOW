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

        private static int eq1;
        private static int comp1;
        private static int eq7;
        private static int eq7Phone;
        private static int deEs;
        private static int _vstEffectEQ1band;
        private static int _vstEffectCompressor;
        private static int _vstEffectEQ4Band;
        private static int _vstEffectEQ4BandPhone;
        private static int _vstEffectDeEsser;
        private static int _vstEffectReverb;
        private static int _vstBasicStrip;

        private static bool m_isEQ1bandSettingChange;
        private static bool m_isCompressorSettingChange;
        private static bool m_isEQ4BandSettingChange;
        private static bool m_isEQ4BandPhoneSettingChange;
        private static bool m_isDeEsserSettingChange;

        public static void SetEffects(int stream)
        {

            string plugin_path = "";
            if (EffectsInteface.Is64Bit())
                plugin_path = m_vst_plugin_PathX64;
            else
                plugin_path = m_vst_plugin_Path;

            m_VstProc = new VSTPROC(VstProc_CallBack);
           
            //EQ 1 Band -----------------
            _vstEffectEQ1band = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"TDR Nova_0_0.dll", BASSVSTDsp.BASS_VST_DEFAULT, 0);

            eq1 = BassVst.BASS_VST_GetParamCount(_vstEffectEQ1band);

            for (int i = 0; i < eq1; i++)
            {
                BassVst.BASS_VST_SetParam(_vstEffectEQ1band, i, AppSettings.Get<float>("CH1EQ1band" + i));
            }

            BassVst.BASS_VST_SetCallback(_vstEffectEQ1band, m_VstProc, IntPtr.Zero);

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

                comp1 = BassVst.BASS_VST_GetParamCount(_vstEffectCompressor);
                for (int i = 0; i < comp1; i++)
                {
                    BassVst.BASS_VST_SetParam(_vstEffectCompressor, i, AppSettings.Get<float>("CH1Comp" + i));
                }

                BassVst.BASS_VST_SetCallback(_vstEffectCompressor, m_VstProc, IntPtr.Zero);
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
            eq7 = BassVst.BASS_VST_GetParamCount(_vstEffectEQ4Band);

            for (int i = 0; i < eq7; i++)
            {
                BassVst.BASS_VST_SetParam(_vstEffectEQ4Band, i, AppSettings.Get<float>("CH1EQ4band" + i));
            }

            BassVst.BASS_VST_SetCallback(_vstEffectEQ4Band, m_VstProc, IntPtr.Zero);
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
            _vstEffectEQ4BandPhone = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"TDR Nova_0_2.dll", BASSVSTDsp.BASS_VST_DEFAULT, 2);
            eq7Phone = BassVst.BASS_VST_GetParamCount(_vstEffectEQ4BandPhone);

            for (int i = 0; i < eq7Phone; i++)
            {
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, i, AppSettings.Get<float>("CH1EQ4bandPhone" + i));
            }

            BassVst.BASS_VST_SetCallback(_vstEffectEQ4BandPhone, m_VstProc, IntPtr.Zero);

           /* for (int i = 0; i < eq7Phone; i++)
            {
                // get the info about the parameter
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 0, 1);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 1, 1);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 3, 0.563f);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 4, 0.278f);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 5, 0);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 12, 0);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 13, 0);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 15, 0.563f);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 16, 0.361f); //Band 2 F
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 24, 0);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 25, 0);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 27, 0.563f);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 28, 0.63883f); //Band 3 F
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 36, 1);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 37, 1);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 39, 0.563f);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 40, 0.7712674f); //Band 4 F
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 41, 1); //Band 4 T
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 48, 1); //HP Selec
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 49, 1); //HP Activ
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 50, 0.4586f);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 51, 0.25f);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 52, 1); //LP Selec
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 53, 1); //LP Activ
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 54, 0.7812674f); //LP Frequ
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 55, 0.6666667f);
                BassVst.BASS_VST_SetParam(_vstEffectEQ4BandPhone, 69, 0.0f);
            }*/


            _vstEffectDeEsser = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"MCompressor_0_D-Es.dll", BASSVSTDsp.BASS_VST_DEFAULT, 3);

            deEs = BassVst.BASS_VST_GetParamCount(_vstEffectDeEsser);

            for (int i = 0; i < deEs; i++)
            {
                BassVst.BASS_VST_SetParam(_vstEffectDeEsser, i, AppSettings.Get<float>("CH1DeEsser" + i));
            }

            BassVst.BASS_VST_SetCallback(_vstEffectDeEsser, m_VstProc, IntPtr.Zero);

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

            _vstEffectReverb = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"OrilRiver.dll", BASSVSTDsp.BASS_VST_DEFAULT, 15);

            int rev = BassVst.BASS_VST_GetParamCount(_vstEffectReverb);

            for (int i = 0; i < rev; i++)
            {
                //  BassVst.BASS_VST_SetParam(_vstEffectReverb, 0, 0.2540541f);
                //  BassVst.BASS_VST_SetParam(_vstEffectReverb, 17, 0.8810811f);
                //  BassVst.BASS_VST_SetParam(_vstEffectReverb, 18, 0.4432433f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 0, 0.8128305f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 1, 0.03f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 2, 1);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 3, 0);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 4, 0.5f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 5, 0.3574602f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 6, 0.83f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 7, 0.2f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 8, 0.38f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 9, 0.4676717f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 10, 0.5414235f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 11, 0.4f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 12, 0.7827124f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 13, 0.585f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 14, 0.863937f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 15, 0);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 16, 0.9354057f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 17, 0.835603f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 18, 0.4120975f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 19, 0);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 20, 0);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 21, 0);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 22, 0);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 23, 0.1765174f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 24, 1);

            }

            _vstBasicStrip = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"BasicStrip0.dll", BASSVSTDsp.BASS_VST_DEFAULT, 4);

            int strip = BassVst.BASS_VST_GetParamCount(_vstBasicStrip);

            for (int i = 0; i < strip; i++)
            {
                BassVst.BASS_VST_SetParam(_vstBasicStrip, 0, 1.0f);
                BassVst.BASS_VST_SetParam(_vstBasicStrip, 1, 0.5f);
            }

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
                case Effects.Load.EQ7_PHONE:
                    _effectsInteface.VstEffectHandle = _vstEffectEQ4BandPhone;
                    _effectsInteface.Show();
                    break;
                case Effects.Load.DeEsser0_0:
                    _effectsInteface.VstEffectHandle = _vstEffectDeEsser;
                    _effectsInteface.Show();
                    break;
                case Effects.Load.REVERB:
                    _effectsInteface.VstEffectHandle = _vstEffectReverb;
                    _effectsInteface.Show();
                    break;
                case Effects.Load.CHANNETSTRIP:
                    _effectsInteface.VstEffectHandle = _vstBasicStrip;
                    _effectsInteface.LoadChannelStrips(PlayerControl.PrefsForm, PlayerControl.PrefsForm.channel1panel, 1, 434);
                    break;
            }            
            
        }

        public static void UpdateSettings()
        {
            if (m_isEQ1bandSettingChange)
            {
                m_isEQ1bandSettingChange = false;
               // int eq1 = BassVst.BASS_VST_GetParamCount(_vstEffectEQ1band);
                for (int i = 0; i < eq1; i++)
                {
                    AppSettings.Set("CH1EQ1band" + i, BassVst.BASS_VST_GetParam(_vstEffectEQ1band, i).ToString());
                    PlayerControl.LoadSaveProgress += i;
                }
            }
            else if (m_isCompressorSettingChange)
            {
                m_isCompressorSettingChange = false;
               // int comp1 = BassVst.BASS_VST_GetParamCount(_vstEffectCompressor);
                for (int i = 0; i < comp1; i++)
                {
                    AppSettings.Set("CH1Comp" + i, BassVst.BASS_VST_GetParam(_vstEffectCompressor, i).ToString());
                    PlayerControl.LoadSaveProgress += i;
                }
            }
            else if (m_isEQ4BandSettingChange)
            {
                m_isEQ4BandSettingChange = false;
               // int eq7 = BassVst.BASS_VST_GetParamCount(_vstEffectEQ4Band);
                for (int i = 0; i < eq7; i++)
                {
                    AppSettings.Set("CH1EQ4band" + i, BassVst.BASS_VST_GetParam(_vstEffectEQ4Band, i).ToString());
                    PlayerControl.LoadSaveProgress += i;
                }
            }
            else if (m_isEQ4BandPhoneSettingChange)
            {
                m_isEQ4BandPhoneSettingChange = false;
                for (int i = 0; i < eq7Phone; i++)
                {
                    AppSettings.Set("CH1EQ4bandPhone" + i, BassVst.BASS_VST_GetParam(_vstEffectEQ4BandPhone, i).ToString());
                    PlayerControl.LoadSaveProgress += i;
                }
            }           
            else if (m_isDeEsserSettingChange)
            {
                m_isDeEsserSettingChange = false;
               // int deEs = BassVst.BASS_VST_GetParamCount(_vstEffectDeEsser);
                for (int i = 0; i < deEs; i++)
                {
                    AppSettings.Set("CH1DeEsser" + i, BassVst.BASS_VST_GetParam(_vstEffectDeEsser, i).ToString());
                    PlayerControl.LoadSaveProgress += i;
                }
            }
        }

        // your VST callback - referenced below by BASS_VST_SetCallback()
        private static int VstProc_CallBack(int vstHandle, BASSVSTAction action, int param1, int param2, IntPtr user)
        {
            switch (action)
            {
                case BASSVSTAction.BASS_VST_PARAM_CHANGED:

                    if (vstHandle == _vstEffectEQ1band)
                    {
                        m_isEQ1bandSettingChange = true;
                    }
                    else if (vstHandle == _vstEffectCompressor)
                    {
                        m_isCompressorSettingChange = true;
                    }
                    else if (vstHandle == _vstEffectEQ4Band)
                    {
                        m_isEQ4BandSettingChange = true;
                    }
                    else if (vstHandle == _vstEffectEQ4BandPhone)
                    {
                        m_isEQ4BandPhoneSettingChange = true;
                    }
                    else if (vstHandle == _vstEffectDeEsser)
                    {
                        m_isDeEsserSettingChange = true;
                    }
                    else if (vstHandle == _vstBasicStrip)
                        BassVst.BASS_VST_SetParamCopyParams(_vstBasicStrip, Channel2Fx.GetChannelStrip);

                    // we get notified that the user has changed some sliders in the editor - 
                    // do what to do here ...

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
        public static bool GetEQ1bandSettingStatus { get { return m_isEQ1bandSettingChange; } }
        public static bool GetCompressorSettingStatus { get { return m_isCompressorSettingChange; } }
        public static bool GetEQ4BandSettingStatus { get { return m_isEQ4BandSettingChange; } }
        public static bool GetDeEsserSettingStatus { get { return m_isDeEsserSettingChange; } }
    }
}
