using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bossdoyKaraoke_NOW.Enums
{
    public class RemoveVocal
    {
        private static ChannelSelected channelSelected;
        public enum ChannelSelected
        {
            None,
            Right,
            Left
        }

        public static ChannelSelected Channel
        {
            get
            {
                return channelSelected;
            }
            set
            {
                channelSelected = value;
            }
        }
    }
}
