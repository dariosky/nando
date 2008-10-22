using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace HS.Controls.Tree
{
    class ColumnState: InputState
    {
        private TreeColumn _column;
        public TreeColumn Column { get { return _column; } }

        public ColumnState(TreeViewAdv tree, TreeColumn column)
            : base(tree)
        {
            _column = column;
        }

        public override void KeyDown(KeyEventArgs args)
        {
        }

        public override void MouseDown(TreeNodeAdvMouseEventArgs args)
        {
        }

        public override void MouseUp(TreeNodeAdvMouseEventArgs args)
        {
        }
    }
}
