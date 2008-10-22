using System;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace HS.Controls.Tree.Base
{
	public class TreePath
	{
		public static readonly TreePath Empty = new TreePath();

		private object[] _path;
		public object[] FullPath
		{
			get { return _path; }
		}

		public object LastNode
		{
			get
			{
				if (_path.Length > 0)
					return _path[_path.Length - 1];
				else
					return null;
			}
		}

		public object FirstNode
		{
			get
			{
				if (_path.Length > 0)
					return _path[0];
				else
					return null;
			}
		}

		public TreePath()
		{
			_path = new object[0];
		}

		public TreePath(object node)
		{
			_path = new object[] { node };
		}

		public TreePath(object[] path)
		{
			_path = path;
		}

        public TreePath(TreePath base_path, object to_add)
        {
            List<object> path = new List<object>(base_path.FullPath);
            path.Add(to_add);
            _path = path.ToArray();
        }

		public bool IsEmpty()
		{
			return (_path.Length == 0);
		}
	}
}
