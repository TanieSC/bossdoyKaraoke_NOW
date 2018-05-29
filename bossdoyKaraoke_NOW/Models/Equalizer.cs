using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace bossdoyKaraoke_NOW.Models
{
    class Equalizer
    {
        public static BandValue[] bandValue = new BandValue[10];
        private BandValue m_bandValue;
        private int m_handle = -1;
        private bool m_isEnabled;
       // 60Hz, 170, 310, 600, 1K, 3K, 6K,12K,14K,16KHz
        private float[] m_centers =
       {
          /*  31.0f,
            63.0f,
            125.0f,
            250.0f,
            500.0f,
            1000.0f,
            2000.0f,
            4000.0f,
            8000.0f,
            16000.0f*/

            60.0f,
            170.0f,
            310.0f,
            600.0f,
            1000.0f,
            3000.0f,
            6000.0f,
            12000.0f,
            14000.0f,
            16000.0f
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
            for (int i = 0; i < bandValue.Length; i++)
            {
                bandValue[i]  = new BandValue();
                bandValue[i].Gain = AppSettings.Get<float>("AudioEQBand" + i);
            }

            m_bandValue = new BandValue();
            m_bandValue.Handle = m_handle;

        }

        public void Init(int handle)
        {
            BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();

            m_bandValue = new BandValue();

            m_bandValue.Handle = Bass.BASS_ChannelSetFX(handle, BASSFXType.BASS_FX_BFX_PEAKEQ, 0);

            eq.fQ = 0f;
            eq.fBandwidth = 2.5f;
            eq.lChannel = BASSFXChan.BASS_BFX_CHANALL;

            for (int i = 0; i < m_centers.Length; i++)
            {
                eq.lBand = i;
                eq.fCenter = m_centers[i];
                Bass.BASS_FXSetParameters(m_bandValue.Handle, eq);
                float gain = bandValue[i].Gain;
                UpdateEQ(i, gain);
                Console.WriteLine(m_bandValue.Handle + " : " + gain + " : " + i);

            }

        }

        public void UpdateEQ(int band, float gain)
        {
            BASS_BFX_PEAKEQ eq = new BASS_BFX_PEAKEQ();


            bandValue[band].Gain = gain;

            if (m_bandValue.Handle == m_handle)
            {
                AppSettings.Set("AudioEQBand" + band, gain.ToString());
                return;
            }
            
            m_bandValue.Gain = gain;

            // get values of the selected band
            eq.lBand = band;
            Bass.BASS_FXGetParameters(m_bandValue.Handle, eq);
            eq.fGain = gain;
            Bass.BASS_FXSetParameters(m_bandValue.Handle, eq);
            Console.WriteLine(m_bandValue.Handle + " : " + eq.fCenter + " : " + band);
            AppSettings.Set("AudioEQBand" + band, gain.ToString());
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


        public class BandValue
        {
            public int Handle { get; set; }
            public float Gain { get; set; }
        }
    }
}
