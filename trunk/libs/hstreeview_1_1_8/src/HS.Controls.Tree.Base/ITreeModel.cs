using System;
using System.Collections;

namespace HS.Controls.Tree.Base
{
	public interface ITreeModel
	{
		IEnumerable GetChildren(TreePath treePath);
        //you can return -1 to iterate through all the GetChildren() result right away, but in this case, 
        //tree nodes won't be created on-the-fly.
        int GetChildrenCount(TreePath treePath);

		event EventHandler<TreeModelEventArgs> NodesChanged; 
		event EventHandler<TreePathEventArgs> StructureChanged;
	}
}
