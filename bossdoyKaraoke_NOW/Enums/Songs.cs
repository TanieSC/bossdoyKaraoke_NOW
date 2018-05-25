using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bossdoyKaraoke_NOW.Enums
{
    public class Songs
    {
        private static SearchAndLoad m_SEARCHDIRorTEXTState;

        /// <summary>
        /// 
        /// </summary>
        public enum SearchAndLoad
        {
            SEARCH_DIRECTORY,
            SEARCH_TEXTFILE,
            ADD_FAVORITES_TO_QUEUE,
            LOAD_QUEUE_SONGS,
            LOAD_FROM_FILE_TO_QUEUE,
            SEARCH_LISTVIEW,
            SHUFFLE_SONGS,
            SORT_SONGS,
            LOAD_FAVORITES,
            LOAD_ADDED_SONGS,
            ADD_SELECTED_SONG_TO_QUEUE,
            WRITE_TO_QUEUE_LIST
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SEARCHState"></param>
        public static void setSEARCHDIRorTEXTState(SearchAndLoad SEARCHState)
        {
            m_SEARCHDIRorTEXTState = SEARCHState;
        }

        /// <summary>
        /// 
        /// </summary>
        public static SearchAndLoad getSEARCHDIRorTEXTState
        {
            get
            {
                return m_SEARCHDIRorTEXTState;
            }
        }
      
    }
        
}
