using System;
using System.Collections.Generic;
using System.Collections;

namespace HS.Controls.Tree.Base
{
    public class BaseTreeModel : ITreeModel
    {
        public virtual IEnumerable GetChildren(TreePath treePath)
        {
            yield break;
        }

        public virtual int GetChildrenCount(TreePath treePath)
        {
            return -1; //automatically determine count by traversing the GetChildren enumeration.
        }

        public event EventHandler<TreeModelEventArgs> NodesChanged;
        protected void OnNodesChanged(TreeModelEventArgs args)
        {
            if (NodesChanged != null)
                NodesChanged(this, args);
        }

        public event EventHandler<TreePathEventArgs> StructureChanged;
        protected void OnStructureChanged(TreePathEventArgs args)
        {
            if (StructureChanged != null)
                StructureChanged(this, args);
        }
    }
}
