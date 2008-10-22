using System;
using System.Collections.Generic;
using System.Text;
using HS.Controls.Tree.NodeControls;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree
{
	public interface IToolTipProvider
	{
		string GetToolTip(TreeNodeAdv node, NodeControl nodeControl);
	}
}
