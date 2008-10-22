using System;
using System.Collections.Generic;
using System.Text;

namespace HS.Controls.Tree.Base
{
	public class TreeViewAdvEventArgs: EventArgs
	{
		private List<TreeNodeAdv> _nodes;
        public List<TreeNodeAdv> Nodes { get { return _nodes; } }

		public TreeNodeAdv Node
		{
			get 
            {
                if (Nodes.Count > 0)
                    return Nodes[0];
                else
                    return null;
            }
		}

		public TreeViewAdvEventArgs(TreeNodeAdv node)
		{
            _nodes = new List<TreeNodeAdv>(new TreeNodeAdv[] { node });
		}

        public TreeViewAdvEventArgs(TreeNodeAdv[] nodes)
        {
            _nodes = new List<TreeNodeAdv>(nodes);
        }
	}
}
