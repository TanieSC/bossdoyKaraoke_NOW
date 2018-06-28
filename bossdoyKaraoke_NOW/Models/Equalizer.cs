using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;
using Un4seen.Bass.Misc;

namespace bossdoyKaraoke_NOW.Models
{
    class Equalizer
    {
        public static BandValue[] ArrBandValue = new BandValue[12];
        private BandValue m_bandValue;
        private int m_preampHandle;
        private DSP_Gain m_dsp_gain;
        private Implementation.Equalizer m_equalizer;
        private int m_handle = -1;
        private bool m_isEnabled;

        Dictionary<float, BandValue> m;

        // 60Hz, 170, 310, 600, 1K, 3K, 6K,12K,14K,16KHz
        private float[] m_centers =
       {
            31.0f,
            63.0f,
            125.0f,
            250.0f,
            500.0f,
            1000.0f,
            2000.0f,
            4000.0f,
            8000.0f,
            16000.0f

         /*   60.0f,
            170.0f,
            310.0f,
            600.0f,
            1000.0f,
            3000.0f,
            6000.0f,
            12000.0f,
            14000.0f,
            16000.0f*/
        };


        /*private float[] m_centers =
        {
            80.0f,
            120.0f,
            250.0f,
            500.0f,
            1000.0f,
            1800.0f,
            3500.0f,
            7000.0f,
            10000.0f,
            14000.0f
        };*/

        public Equalizer()
        {
            

            AppSettings.Get<bool>("IsDefaultInit");

            for (int i = 0; i < ArrBandValue.Length; i++)
            {
                ArrBandValue[i]  = new BandValue();
                ArrBandValue[i].Gain = AppSettings.Get<float>("AudioEQBand" + i);
            }

            ArrBandValue[10].PreAmp = AppSettings.Get<float>("AudioEQPreamp");
            ArrBandValue[11].PreSet = AppSettings.Get<int>("AudioEQPreset");

            m_bandValue = new BandValue();
            m_bandValue.Handle = -1;

        }

        public void Init(int handle)
        {
            if (handle != -1)
            {

                BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();
                BASS_BFX_COMPRESSOR2 comp = new BASS_BFX_COMPRESSOR2();
                BASS_BFX_VOLUME preamp = new BASS_BFX_VOLUME();
               

                m_bandValue = new BandValue();

                // int compVal = Bass.BASS_ChannelSetFX(handle, BASSFXType.BASS_FX_BFX_COMPRESSOR2, 0);

                // comp.lChannel = BASSFXChan.BASS_BFX_CHANALL;
                //comp.fGain = 7.0f;
                //comp.fAttack = 24.9f;
                //comp.fRelease = 99.9f;
                //comp.fThreshold = -11.0f;
                //comp.fRatio = 4f;
                // Bass.BASS_FXSetParameters(compVal, comp);

                m_bandValue.Handle = Bass.BASS_ChannelSetFX(handle, BASSFXType.BASS_FX_BFX_PEAKEQ, 5);

                if (m_dsp_gain != null) m_dsp_gain.Dispose();
                m_dsp_gain = new DSP_Gain(handle, 6);
                m_dsp_gain.Gain_dBV = ArrBandValue[10].PreAmp / 10;

              //  m_preampHandle = Bass.BASS_ChannelSetFX(handle, BASSFXType.BASS_FX_BFX_VOLUME, 6);
              //  preamp.lChannel = BASSFXChan.BASS_BFX_CHANNONE;
              //  preamp.fVolume = (float)Math.Pow(10, (ArrBandValue[10].PreAmp / 10) / 20); //ArrBandValue[10].PreAmp;
              //   Bass.BASS_FXSetParameters(m_preampHandle, preamp);

                // m_handle = handle;

                eq.fQ = 0f;
                eq.fBandwidth = 0.5f;
                eq.lChannel = BASSFXChan.BASS_BFX_CHANALL;

                for (int i = 0; i < m_centers.Length; i++)
                {
                    eq.lBand = i;
                    eq.fCenter = m_centers[i];
                    Bass.BASS_FXSetParameters(m_bandValue.Handle, eq);
                    float gain = ArrBandValue[i].Gain;
                    UpdateEQBass(i, gain);
                    Console.WriteLine(m_bandValue.Handle + " : " + gain + " : " + i);

                }
            }
            else
            {
                if (m_equalizer != null) m_equalizer.Dispose();

                m_equalizer = new Implementation.Equalizer();
                m_equalizer.Preamp = ArrBandValue[10].PreAmp / 10;

                for (int i = 0; i < m_centers.Length; i++)
                {
                    float gain = ArrBandValue[i].Gain;
                    UpdateEQVlc(i, gain);
                }

               // m_handle = handle;
                m_bandValue.Handle = -1;
            }

        }

        public void UpdateEQPresets(int preset)
        {
            ArrBandValue[11].PreSet = preset;
            AppSettings.Set("AudioEQPreset", preset.ToString());
        }

        public void UpdateEQBassPreamp(float gain)
        {

            if (m_dsp_gain == null)
            {
                AppSettings.Set("AudioEQPreamp", gain.ToString());
                return;
            }

            // BASS_BFX_VOLUME preamp = new BASS_BFX_VOLUME();

            ArrBandValue[10].PreAmp = gain;
            m_dsp_gain.Gain_dBV = gain / 10;
            // Bass.BASS_FXGetParameters(m_preampHandle, preamp);
            //  preamp.fVolume = (float)Math.Pow(10, (gain / 10) / 20);
            //  Bass.BASS_FXSetParameters(m_preampHandle, preamp);
            //  double dB = 20.0 * Math.Log10((gain / 10) / 1.0);
            AppSettings.Set("AudioEQPreamp", gain.ToString());
            Console.WriteLine(m_dsp_gain.Gain_dBV);

        }

        public void UpdateEQBass(int band, float gain)
        {
            BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();


            ArrBandValue[band].Gain = gain;

            if (m_bandValue.Handle == -1)
            {
                AppSettings.Set("AudioEQBand" + band, gain.ToString());
                return;
            }
            
            //m_bandValue.Gain = gain;

            // get values of the selected band
            eq.lBand = band;
            Bass.BASS_FXGetParameters(m_bandValue.Handle, eq);
            eq.fGain = gain / 10;
            Bass.BASS_FXSetParameters(m_bandValue.Handle, eq);
            Console.WriteLine(m_bandValue.Handle + " : " + eq.fCenter + " : " + eq.fGain + " : " + ArrBandValue[band].Gain);
            AppSettings.Set("AudioEQBand" + band, gain.ToString("0.0"));
        }

        public Implementation.Equalizer UpdateEQVlcPreamp(float gain)
        {
            ArrBandValue[10].PreAmp = gain;

            if (m_equalizer == null)
            {
                AppSettings.Set("AudioEQPreamp", gain.ToString());
                return null;
            }

            m_equalizer.Preamp = gain / 10;
            AppSettings.Set("AudioEQPreamp", gain.ToString());
            return m_equalizer;

        }

        public Implementation.Equalizer UpdateEQVlc(int band, float gain)
        {
            if (m_equalizer == null)
            {
                AppSettings.Set("AudioEQBand" + band, gain.ToString());
                return null;
            }
            m_equalizer.Bands[band].Amplitude = gain / 10;
            AppSettings.Set("AudioEQBand" + band, gain.ToString("0.0"));
            Console.WriteLine(gain + " : " + m_equalizer.Bands[band].Amplitude);

            return m_equalizer;
        }

        /*  public void SetGain(int band, float gain)
          {
              var eq = new BASS_DX8_PARAMEQ();
              BandValue bandValue = m_bands[band];
              bandValue.Gain = gain;

              if (!m_isEnabled) return;

              int handle = bandValue.Handle;

              if (!Bass.BASS_FXGetParameters(handle, eq)) return;

              eq.fGain = gain;
              Bass.BASS_FXSetParameters(handle, eq);
          }

          public float GetGain(int band)
          {
              BandValue bandValue = m_bands[band];

              if (!m_isEnabled) return bandValue.Gain;

              var eq = new BASS_DX8_PARAMEQ();

              int handle = bandValue.Handle;

              return Bass.BASS_FXGetParameters(handle, eq) ? eq.fGain : bandValue.Gain;
          }*/

        private void Deactivate()
        {
            if (m_bandValue.Handle == -1) return;

              // Bass.BASS_ChannelRemoveFX(m_bandValue.Handle, t.Handle);

        }



        public class BandValue
        {
            public int Handle { get; set; }
            public float Gain { get; set; }
            public float PreAmp { get; set; }
            public int PreSet { get; set; }
        }
    }
}
