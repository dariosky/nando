using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace HS.Controls.Tree
{
    enum ColumnLocation
    {
        Before,
        FirstHalf,
        SecondHalf,
        After
    }

    class ReorderColumnState : ColumnState
	{
		#region Properties

        private Point _location;
        public Point Location
        {
            get { return _location; }
            private set { _location = value; }
        }

		private Bitmap _ghostImage;
		public Bitmap GhostImage
		{
			get { return _ghostImage; }
		}

		#endregion

		public ReorderColumnState(TreeViewAdv tree, TreeColumn column, Point location)
			: base(tree, column)
		{
            _location = location;
            _ghostImage = column.CreateGhostImage(new Rectangle(0, 0, column.Width, tree.Columns.Height), tree.Font);
		}

		public override void KeyDown(KeyEventArgs args)
		{
			args.Handled = true;
			if (args.KeyCode == Keys.Escape)
				FinishReordering();
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
            FinishReordering();
		}

		public override bool MouseMove(MouseEventArgs args)
		{
			Location = new Point(args.X + Tree.OffsetX, 0);
			Tree.UpdateView();
			return true;
		}

        private void FinishReordering()
		{
            TreeColumn dropColumn = null;
            foreach (TreeColumn c in Tree.VisibleColumns)
            {
                ColumnLocation relativeLocation = GetRelativeMousePosition(c.Bounds);
                if (relativeLocation == ColumnLocation.FirstHalf || relativeLocation == ColumnLocation.Before)
                {
                    dropColumn = c;
                    break;
                }
            }
			Tree.ChangeInput();
			if (Column == dropColumn)
				Tree.UpdateView();
			else
			{
				Tree.Columns.Remove(Column);
				if (dropColumn == null)
					Tree.Columns.Add(Column);
				else
					Tree.Columns.Insert(Tree.Columns.IndexOf(dropColumn), Column);
				Tree.OnColumnReordered(Column);
			}
		}

        internal ColumnLocation GetRelativeMousePosition(Rectangle columnRect)
        {
            int x = Location.X;
            if (x < columnRect.X)
                return ColumnLocation.Before;
            if (x < columnRect.X + (columnRect.Width / 2))
                return ColumnLocation.FirstHalf;
            if (x < columnRect.Right)
                return ColumnLocation.SecondHalf;
            return ColumnLocation.After;
        }
	}
}
