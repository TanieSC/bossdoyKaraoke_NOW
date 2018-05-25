using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bossdoyKaraoke_NOW.Enums;
using Un4seen.Bass.AddOn.Vst;

namespace bossdoyKaraoke_NOW.Models.VocalEffects
{
    public class Channel4Fx
    {
        static EffectsInteface _effectsInteface;
        static string m_vst_plugin_Path = AppDomain.CurrentDomain.BaseDirectory + @"vst_plugins\";
        private static int _vstEffectReverb;
        private static int _vstBasicStrip;

        public static void SetEffects(int stream)
        {
            //EQ 1 Band -----------------
            _vstEffectReverb = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"OrilRiver.dll", BASSVSTDsp.BASS_VST_DEFAULT, 15);

            int rev = BassVst.BASS_VST_GetParamCount(_vstEffectReverb);

            for (int i = 0; i < rev; i++)
            {
                //  BassVst.BASS_VST_SetParam(_vstEffectReverb, 0, 0.2540541f);
                //  BassVst.BASS_VST_SetParam(_vstEffectReverb, 17, 0.8810811f);
                //  BassVst.BASS_VST_SetParam(_vstEffectReverb, 18, 0.4432433f);
                BassVst.BASS_VST_SetParam(_vstEffectReverb, 0, 0.4972973f);
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
      
            _vstBasicStrip = BassVst.BASS_VST_ChannelSetDSP(stream, m_vst_plugin_Path + @"BasicStrip3.dll", BASSVSTDsp.BASS_VST_DEFAULT, 16);

            int strip = BassVst.BASS_VST_GetParamCount(_vstBasicStrip);

            for (int i = 0; i < strip; i++)
            {
                BassVst.BASS_VST_SetParam(_vstBasicStrip, 0, 0.81f);
                BassVst.BASS_VST_SetParam(_vstBasicStrip, 1, 0.5f);
            }
        }

        public static void ShowInterface()
        {
            _effectsInteface = new EffectsInteface();

            switch (Effects.GetorSetFx)
            {
                case Effects.Load.REVERB:
                    _effectsInteface.VstEffectHandle = _vstEffectReverb;
                    _effectsInteface.Show();
                    break;
                case Effects.Load.CHANNETSTRIP:
                    _effectsInteface.VstEffectHandle = _vstBasicStrip;
                    _effectsInteface.LoadChannelStrips(PlayerControl.PrefsForm, PlayerControl.PrefsForm.channel4panel, 1, 434);
                    break;
            }
        }
    }
}
