using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree
{
	internal class InputWithControl: NormalInputState
	{
		public InputWithControl(TreeViewAdv tree): base(tree)
		{
		}

		protected override void DoMouseOperation(TreeNodeAdvMouseEventArgs args)
		{
			if (Tree.AllowMultipleSelection)
			{
                if (Tree.IsSelected(args.Node))
                    Tree.Deselect(args.Node);
                else
                    Tree.Select(args.Node);
			}
            else
                base.DoMouseOperation(args);
		}

        public override void KeyDown(KeyEventArgs args)
        {
            if (args.KeyCode == Keys.Add /* NumPlus */)
            {
                Tree.AutoSizeAllColumns();
                args.Handled = true;
            }
        }

		protected override void MouseDownAtEmptySpace(TreeNodeAdvMouseEventArgs args)
		{
		}
	}
}
