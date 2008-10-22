using System;
using System.Collections.Generic;
using System.Collections;

namespace HS.Controls.Tree.Base
{
	/// <summary>
	/// Converts ICollection interface to ITreeModel. 
	/// Allows to display a plain list in the TreeView
	/// </summary>
	public class TreeListAdapter : ITreeModel
	{
		private ICollection _list;

		public TreeListAdapter(ICollection list)
		{
			_list = list;
		}

		#region ITreeModel Members

		public IEnumerable GetChildren(TreePath treePath)
		{
			if (treePath.IsEmpty())
				return _list;
			else
				return null;
		}

        public int GetChildrenCount(TreePath treePath)
        {
            if (treePath.IsEmpty())
                return _list.Count;
            else
                return 0;
        }

		public event EventHandler<TreeModelEventArgs> NodesChanged;
		public void OnNodesChanged(TreeModelEventArgs args)
		{
			if (NodesChanged != null)
				NodesChanged(this, args);
		}

		public event EventHandler<TreePathEventArgs> StructureChanged;
		public void OnStructureChanged(TreePathEventArgs args)
		{
			if (StructureChanged != null)
				StructureChanged(this, args);
		}
		#endregion
	}
}
