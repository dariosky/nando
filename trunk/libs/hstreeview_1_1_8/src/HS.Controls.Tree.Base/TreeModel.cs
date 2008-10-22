using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Collections.ObjectModel;

namespace HS.Controls.Tree.Base
{
	public class TreeModel : ITreeModel
	{
		private Node _root;
		public Node Root
		{
			get { return _root; }
		}

		public Collection<Node> Nodes
		{
			get { return _root.Nodes; }
		}

		public TreeModel()
		{
			_root = new Node();
			_root.Model = this;
		}

		public TreePath GetPath(Node node)
		{
			if (node == _root)
				return TreePath.Empty;
			else
			{
				Stack<object> stack = new Stack<object>();
				while (node != _root)
				{
					stack.Push(node);
					node = node.Parent;
				}
				return new TreePath(stack.ToArray());
			}
		}

		public Node FindNode(TreePath path)
		{
			if (path.IsEmpty())
				return _root;
			else
				return FindNode(_root, path, 0);
		}

		private Node FindNode(Node root, TreePath path, int level)
		{
			foreach (Node node in root.Nodes)
				if (node == path.FullPath[level])
				{
					if (level == path.FullPath.Length - 1)
						return node;
					else
						return FindNode(node, path, level + 1);
				}
			return null;
		}

		#region ITreeModel Members

		public IEnumerable GetChildren(TreePath treePath)
		{
			Node node = FindNode(treePath);
            if (node != null)
                return node.Nodes;
            else
                return new List<object>();
		}

        public int GetChildrenCount(TreePath treePath)
        {
            Node node = FindNode(treePath);
            if (node != null)
                return node.Nodes.Count;
            else
                return 0;
        }

		public bool IsLeaf(TreePath treePath)
		{
			Node node = FindNode(treePath);
			if (node != null)
				return node.IsLeaf;
			else
				throw new ArgumentException("treePath");
		}

		public event EventHandler<TreeModelEventArgs> NodesChanged;
		internal void OnNodesChanged(TreeModelEventArgs args)
		{
			if (NodesChanged != null)
				NodesChanged(this, args);
		}

		public event EventHandler<TreePathEventArgs> StructureChanged;
		public void OnStructureChanged(TreePathEventArgs args)
		{
			if (StructureChanged != null)
				StructureChanged(this, args);
		}

		internal void OnNodeInserted(Node parent, int index, Node node)
		{
            OnStructureChanged(new TreePathEventArgs(GetPath(parent)));
		}

		internal void OnNodeRemoved(Node parent, int index, Node node)
		{
            OnStructureChanged(new TreePathEventArgs(GetPath(parent)));
		}

		#endregion
	}
}
