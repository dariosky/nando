using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace HS.Controls.Tree
{
    public partial class TreeViewAdv
    {
        [Category("Action")]
        public event ItemDragEventHandler ItemDrag;
        private void OnItemDrag(MouseButtons buttons, object item)
        {
            if (ItemDrag != null)
                ItemDrag(this, new ItemDragEventArgs(buttons, item));
        }

        [Category("Behavior")]
        public event EventHandler<TreeNodeAdvMouseEventArgs> NodeMouseDoubleClick;
        private void OnNodeMouseDoubleClick(TreeNodeAdvMouseEventArgs args)
        {
            if (NodeMouseDoubleClick != null)
                NodeMouseDoubleClick(this, args);
        }

        [Category("Behavior")]
        public event EventHandler<TreeColumnEventArgs> ColumnReordered;
        internal void OnColumnReordered(TreeColumn column)
        {
            if (ColumnReordered != null)
                ColumnReordered(this, new TreeColumnEventArgs(column));
        }

        [Category("Behavior")]
        public event EventHandler<TreeColumnEventArgs> ColumnClicked;
        internal void OnColumnClicked(TreeColumn column)
        {
            if (ColumnClicked != null)
                ColumnClicked(this, new TreeColumnEventArgs(column));
        }

        [Category("Behavior")]
        public event EventHandler SelectionChanged;
        internal void OnSelectionChanged()
        {
            if (_updateHoldLevel > 0)
            {
                _selectionChanged = true;
                return;
            }
            _selectionChanged = false;
            if (SelectionChanged != null)
                SelectionChanged(this, EventArgs.Empty);
        }
    }
}
