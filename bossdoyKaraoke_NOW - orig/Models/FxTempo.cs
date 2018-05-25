using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Fx;

namespace bossdoyKaraoke_NOW.Models
{
    class FxTempo : Mixer
    {
        private float m_key = 0f;
        private float m_tempo = 0f;
        private static int m_channelTempo = 0;

        public int TempoCreate()
        {
            m_channelTempo = BassFx.BASS_FX_TempoCreate(Channel, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_FX_FREESOURCE | BASSFlag.BASS_SAMPLE_FLOAT);
            return m_channelTempo;
        }

        public int Channel { private get; set; }

        public float Key
        {
            get
            {
                return this.m_key;
            }
            set
            {
                this.m_key = value;
                Bass.BASS_ChannelSetAttribute(m_channelTempo, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, value);
            }
        }

        public float Tempo
        {
            get
            {
                return this.m_tempo;
            }
            set
            {
                this.m_tempo = value;
                Bass.BASS_ChannelSetAttribute(m_channelTempo, BASSAttribute.BASS_ATTRIB_TEMPO, value);
            }
        }



    }
}
