using System;
using System.Collections.Generic;
using System.Text;

namespace HS.Controls.Tree.Base
{
	public class TreeModelEventArgs: TreePathEventArgs
	{
		private object[] _children;
		public object[] Children
		{
			get { return _children; }
		}

		private int[] _indices;
		public int[] Indices
		{
			get { return _indices; }
		}

		public TreeModelEventArgs(TreePath parent, object[] children)
			: this(parent, null, children)
		{
		}

		public TreeModelEventArgs(TreePath parent, int[] indices, object[] children)
			: base(parent)
		{
			if (children == null)
				throw new ArgumentNullException();

			if (indices != null && indices.Length != children.Length)
				throw new ArgumentException("indices and children arrays must have the same length");

			_indices = indices;
			_children = children;
		}
	}
}
