using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace HS.Controls.Tree
{
    class ClickedColumnState: ColumnState
    {
        private Point _location;
        public Point Location
        {
            get { return _location; }
            private set { _location = value; }
        }

        public ClickedColumnState(TreeViewAdv tree, TreeColumn column, Point location)
            : base(tree,column)
        {
            _location = location;
        }

        public override bool MouseMove(MouseEventArgs args)
        {
            if (TreeViewAdv.Dist(_location, args.Location) > TreeViewAdv.ItemDragSensivity
                && Tree.AllowColumnReorder)
            {
                Tree.Input = new ReorderColumnState(Tree, Column, args.Location);
                Tree.UpdateView();
            }
            return true;
        }

        public override void MouseUp(TreeNodeAdvMouseEventArgs args)
        {
            Tree.ChangeInput();
            Tree.UpdateView();
            Tree.OnColumnClicked(Column);
        }
    }
}
