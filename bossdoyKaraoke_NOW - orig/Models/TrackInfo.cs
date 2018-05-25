using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;

namespace bossdoyKaraoke_NOW.Models
{
    [Serializable]
    public class TrackInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Duration { get; set; }
        public string FilePath { get; set; }
        public TAG_INFO Tags { get; set; }

        public TrackInfo()
        {

        }

        public TrackInfo(string filename)
        {
            Tags = BassTags.BASS_TAG_GetFromFile(filename);
            Duration = Tags.duration.ToString();

            if (Tags == null)
                throw new ArgumentException("File not valid!");
        }

        public ListViewItem toListViewItem()
        {
            ListViewItem track = new ListViewItem(new[] {

                     ID,
                     Name,
                     Artist,
                     Duration,
                     FilePath
                 });
            track.Tag = Tags;
            return track;
        }
    }
}
