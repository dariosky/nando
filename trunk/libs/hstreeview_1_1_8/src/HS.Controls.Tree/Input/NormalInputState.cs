using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree
{
	internal class NormalInputState : InputState
	{
		private bool _mouseDownFlag = false;

		public NormalInputState(TreeViewAdv tree) : base(tree)
		{
		}

		public override void KeyDown(KeyEventArgs args)
		{
            if (Tree.FocusedRow < 0 && Tree.Root.Nodes.Count > 0)
                FocusRow(0);

			if (Tree.FocusedRow >= 0)
			{
                TreeNodeAdv focusedNode = Tree.FocusedNode;
                TreeNodeAdv node;
				switch (args.KeyCode)
				{
					case Keys.Right:
                        if (!focusedNode.IsExpanded)
                            focusedNode.IsExpanded = true;
                        else if (focusedNode.Nodes.Count > 0)
                            Tree.SelectedNode = focusedNode.Nodes[0];
						args.Handled = true;
						break;
					case Keys.Left:
                        if (focusedNode.IsExpanded)
                            focusedNode.IsExpanded = false;
                        else if (focusedNode.Parent != Tree.Root)
                            Tree.SelectedNode = focusedNode.Parent;
						args.Handled = true;
						break;
					case Keys.Down:
						NavigateForward(1);
						args.Handled = true;
						break;
					case Keys.Up:
						NavigateBackward(1);
						args.Handled = true;
						break;
					case Keys.PageDown:
						NavigateForward(Tree.RowCountPerPage);
						args.Handled = true;
						break;
					case Keys.PageUp:
                        NavigateBackward(Tree.RowCountPerPage);
						args.Handled = true;
						break;
					case Keys.Home:
						FocusRow(0);
						args.Handled = true;
						break;
					case Keys.End:
                        node = Tree.GetLastVisibleNode();
                        if (node != null)
                            FocusRow(Tree.GetRowOfNode(node));
						args.Handled = true;
						break;
				}
			}
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
			if (args.Node != null)
			{
				Tree.ItemDragMode = true;
				Tree.ItemDragStart = args.Location;

				if (args.Button == MouseButtons.Left || args.Button == MouseButtons.Right)
				{
					if (Tree.IsSelected(args.Node))
						_mouseDownFlag = true;
					else
					{
						_mouseDownFlag = false;
						DoMouseOperation(args);
					}
				}

			}
			else
			{
				Tree.ItemDragMode = false;
				MouseDownAtEmptySpace(args);
			}
		}

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			Tree.ItemDragMode = false;
			if (_mouseDownFlag)
			{
                if (args.Button == MouseButtons.Left)
                    DoMouseOperation(args);
                else if (args.Button == MouseButtons.Right)
                    FocusRow(Tree.GetRowOfNode(args.Node));
			}
			_mouseDownFlag = false;
		}


		private void NavigateBackward(int n)
		{
            int rowOfNode = Tree.FocusedRow;
			int row = Math.Max(rowOfNode - n, 0);
			if (row != rowOfNode)
				FocusRow(row);
		}

		private void NavigateForward(int n)
		{
            int row = Tree.FocusedRow;
            row += n;
            TreeNodeAdv node = Tree.GetNodeOfRow(row);
            if (node == null)
                node = Tree.GetLastVisibleNode();
            if (node != Tree.FocusedNode)
				FocusRow(Tree.GetRowOfNode(node));
		}

		protected virtual void MouseDownAtEmptySpace(TreeNodeAdvMouseEventArgs args)
		{
			Tree.ClearSelection();
		}

		protected virtual void FocusRow(int row)
		{
            Tree.ClearSelection();
            Tree.Select(row);
            Tree.ScrollTo(row);
		}

		protected virtual void DoMouseOperation(TreeNodeAdvMouseEventArgs args)
		{
            Tree.ClearSelection();
            Tree.Select(args.Node);
		}
	}
}
