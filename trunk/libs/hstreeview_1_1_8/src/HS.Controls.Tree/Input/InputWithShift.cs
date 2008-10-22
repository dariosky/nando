using System;
using System.Collections.Generic;
using System.Text;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree
{
	internal class InputWithShift: NormalInputState
	{
		public InputWithShift(TreeViewAdv tree): base(tree)
		{
		}

		protected override void FocusRow(int row)
		{
		    if (Tree.AllowMultipleSelection && Tree.FocusedRow >= 0)
		    {
                SelectAllFromStart(row);
			    Tree.ScrollTo(row);
		    }
            else
                base.FocusRow(row);
		}

		protected override void DoMouseOperation(TreeNodeAdvMouseEventArgs args)
		{
            if (Tree.AllowMultipleSelection && Tree.FocusedRow >= 0)
                SelectAllFromStart(Tree.GetRowOfNode(args.Node));
			else
                base.DoMouseOperation(args);
			    
		}

		protected override void MouseDownAtEmptySpace(TreeNodeAdvMouseEventArgs args)
		{
		}

		private void SelectAllFromStart(int row)
		{
            Tree.ClearSelection();
            Tree.SelectRangeFromAnchorToRow(row);
		}
	}
}
