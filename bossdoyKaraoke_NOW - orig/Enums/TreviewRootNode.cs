using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bossdoyKaraoke_NOW.Enums
{
    class TreviewNode
    {

        //private static RootNode rootNode;

        public enum RootNode
        {
            SONG_QUEUE,
            MY_FAVORITES,
            ADD_FAVORITES,
            MY_COMPUTER,
            ADD_FOLDER

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static IEnumerable<TreeNode> Collect(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                yield return node;

                foreach (var child in Collect(node.Nodes))
                    yield return child;
            }
        }
    }
}
