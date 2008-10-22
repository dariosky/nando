using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using HS.Controls.Tree.Properties;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree.NodeControls
{
	public class NodeStateIcon: NodeIcon
	{
		private Image _leaf;
		private Image _opened;
		private Image _closed;

		public NodeStateIcon()
		{
			_leaf = MakeTransparent(Resources.Leaf);
			_opened = MakeTransparent(Resources.Folder);
			_closed = MakeTransparent(Resources.FolderClosed);
		}

		private static Image MakeTransparent(Bitmap bitmap)
		{
			bitmap.MakeTransparent(bitmap.GetPixel(0,0));
			return bitmap;
		}

		protected override Image GetIcon(TreeNodeAdv node)
		{
			Image icon = base.GetIcon(node);
			if (icon != null)
				return icon;
			else if (!node.HasChildren)
				return _leaf;
			else if (node.IsExpanded)
				return _opened;
			else
				return _closed;
		}
	}
}
