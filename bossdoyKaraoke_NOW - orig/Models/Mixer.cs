using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;

namespace bossdoyKaraoke_NOW.Models
{
    public class Mixer
    {
        private static int m_mixerChannel = 0;

        public static int MixerStreamCreate(int samplerate) {

            if(Player.IsBassInitialized)
                m_mixerChannel = BassMix.BASS_Mixer_StreamCreate(samplerate, 2, BASSFlag.BASS_SAMPLE_FLOAT );

            if (Player.IsWasapiInitialized)
                 m_mixerChannel = BassMix.BASS_Mixer_StreamCreate(samplerate, 2, BASSFlag.BASS_SAMPLE_FLOAT );

            if(Player.IsAsioInitialized )
                m_mixerChannel = BassMix.BASS_Mixer_StreamCreate(samplerate, 2, BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_STREAM_DECODE);

            Console.WriteLine("Player.IsBassInitialized " + Player.IsBassInitialized);
            Console.WriteLine("Player.IsAsioInitialized " + Player.IsAsioInitialized);


            return m_mixerChannel;

        }

        public void StreamAddChannel(int channel, SYNCPROC trackSync) {

            BassMix.BASS_Mixer_StreamAddChannel(Player.Mixer, channel, BASSFlag.BASS_MIXER_PAUSE | BASSFlag.BASS_MIXER_DOWNMIX | BASSFlag.BASS_STREAM_AUTOFREE);

            // an BASS_SYNC_END is used to trigger the next track in the playlist (if no POS sync was set)
            BassMix.BASS_Mixer_ChannelSetSync(channel, BASSSync.BASS_SYNC_END, 0L, trackSync, new IntPtr(1));
        }

    }
}
