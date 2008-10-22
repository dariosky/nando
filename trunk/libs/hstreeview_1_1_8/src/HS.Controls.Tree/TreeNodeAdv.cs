using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree
{
	public class TreeNodeAdv
    {
        public TreeNodeAdv(TreeNodeAdv parent, object tag)
        {
            _parent = parent;
            _tag = tag;
            _isExpanded = false;
            _childrenRowCount = -1;
            _childrenCount = -2;
            _nodes = null;
        }

        #region Internal Properties

        private TreeViewRoot _root = null;
        internal TreeViewRoot Root
        {
            get
            {
                if (_root == null)
                    _root = _GetRoot();
                return _root;
            }
        }

        internal ITreeModel Model { get { return Root.Model; } }

        private int _childrenCount;
        internal int ChildrenCount
        {
            get
            {
                if (_nodes != null)
                    return _nodes.Count;
                if (_childrenCount == -2)
                {
                    if (Model != null)
                    {
                        TreePath path = GetPath();
                        _childrenCount = Model.GetChildrenCount(path);
                    }
                    else
                        _childrenCount = 0;
                }
                return _childrenCount;
            }
        }

        #endregion

        #region Public Properties

		public int Index
		{
			get
			{
				if (_parent == null)
					return -1;
				else
					return _parent.Nodes.IndexOf(this);
			}
		}

		public bool IsExpandedOnce
		{
			get { return _nodes != null; }
		}

		protected bool _isExpanded;
		public bool IsExpanded
		{
			get { return _isExpanded; }
			set 
			{
                if (_isExpanded != value)
                    _SetExpanded(value, true);
			}
		}

		private TreeNodeAdv _parent;
		public TreeNodeAdv Parent
		{
			get { return _parent; }
		}

		public int Level
		{
			get
			{
				if (_parent == null)
					return 0;
				else
					return _parent.Level + 1;
			}
		}

        public bool HasChildren
        {
            get
            {
                return ChildrenCount != 0;
            }
        }

		public TreeNodeAdv NextSibling
		{
			get
			{
                if (Parent == null)
                    return null;
                int index = Parent.Nodes.IndexOf(this);
                if (index < Parent.Nodes.Count - 1)
                    return Parent.Nodes[index + 1];
                else
                    return null;
			}
		}

		private object _tag;
		public object Tag { get { return _tag; } }

        private int _childrenRowCount;
        public int RowCount // We don't count self in the rowcount, only children
        {
            get
            {
                if (IsExpanded)
                {
                    if (_childrenRowCount < 0)
                    {
                        _childrenRowCount = 0;
                        foreach (TreeNodeAdv node in Nodes.Fetched)
                            _childrenRowCount += node.RowCount;
                    }
                    return Nodes.Count + _childrenRowCount;
                }
                else
                    return 0;
            }
        }

        private ListGenerator<object, TreeNodeAdv> _nodes = null;
        public ListGenerator<object, TreeNodeAdv> Nodes
		{
			get 
            {
                if (_nodes == null)
                {
                    IEnumerator source = null;
                    if (Model != null)
                    {
                        TreePath path = GetPath();
                        IEnumerable items = Model.GetChildren(path);
                        if (items != null)
                            source = items.GetEnumerator();
                    }
                    if (source == null)
                        source = new ArrayList().GetEnumerator();
                    _nodes = new ListGenerator<object, TreeNodeAdv>(source, ChildrenCount, _GenerateNodeFromTag);
                    _nodes.ItemGenerated += Nodes_ItemGenerated;
                }
                return _nodes; 
            }
		}

        public IEnumerable<TreeNodeAdv> AllNodes
        {
            get
            {
                foreach (TreeNodeAdv node in Nodes)
                {
                    yield return node;
                    foreach (TreeNodeAdv subNode in node.AllNodes)
                        yield return subNode;
                }
            }
        }

        public IEnumerable<TreeNodeAdv> AllFetchedNodes
        {
            get
            {
                foreach (TreeNodeAdv node in Nodes.Fetched)
                {
                    yield return node;
                    foreach (TreeNodeAdv subNode in node.AllFetchedNodes)
                        yield return subNode;
                }
            }
        }

        public IEnumerable<TreeNodeAdv> VisibleNodes
        {
            get
            {
                foreach (TreeNodeAdv node in Nodes)
                {
                    yield return node;
                    if (node.IsExpanded)
                    {
                        foreach (TreeNodeAdv subNode in node.VisibleNodes)
                            yield return subNode;
                    }
                }
            }
        }

		#endregion

        #region Private

        private void _GenerateNodeFromTag(object source, out TreeNodeAdv result)
        {
            result = new TreeNodeAdv(this, source);
            result.RowCountChanged += Child_RowCountChanged;
            result.ExpansionStateChanged += Child_ExpansionStateChanged;
        }

        protected virtual TreeViewRoot _GetRoot()
        {
            if (Parent != null)
                return Parent._GetRoot();
            else
                return null;
        }

        private void _SetExpanded(bool expanded, bool changedFromOutside)
        {
            _isExpanded = expanded;
            OnRowCountChanged();
            if (changedFromOutside)
            {
                Root._SetTagIsExpanded(Tag, expanded);
                OnExpansionStateChanged(this);
            }
        }

        #endregion

        public TreeNodeAdv FindNode(TreePath path)
        {
            return FindNode(path, false);
        }

        public TreeNodeAdv FindNode(TreePath path, bool fetchedOnly)
        {
            if (path.IsEmpty())
                return this;
            IEnumerable lookIn = Nodes;
            if (fetchedOnly)
                lookIn = Nodes.Fetched;
            foreach (TreeNodeAdv node in lookIn)
            {
                if (node.Tag == path.FirstNode)
                {
                    List<object> newPath = new List<object>(path.FullPath);
                    newPath.RemoveAt(0);
                    return node.FindNode(new TreePath(newPath.ToArray()));
                }
            }
            return null;
        }

        public virtual TreePath GetPath()
        {
            TreePath path = Parent.GetPath();
            path = new TreePath(path, Tag);
            return path;
        }

        public virtual void InvalidateNodes()
        {
            //We use Nodes.Fetched because we don't want to fetch children, we only want to invalidate
            //those that have already been fetched.
            foreach (TreeNodeAdv node in Nodes.Fetched)
            {
                node._parent = null;
                node._root = null;
            }
            _nodes = null;
            _childrenCount = -2;
            _childrenRowCount = -1;
        }

        public override string ToString()
        {
            if (Tag != null)
                return Tag.ToString();
            else
                return base.ToString();
        }

        #region Event Handlers

        private void Child_ExpansionStateChanged(object sender, EventArgs e)
        {
            OnExpansionStateChanged(sender);
        }

        private void Child_RowCountChanged(object sender, EventArgs e)
        {
            _childrenRowCount = -1;
            if (IsExpanded)
                OnRowCountChanged();
        }

        private void Nodes_ItemGenerated(object sender, ItemGeneratedArgs<TreeNodeAdv> e)
        {
            if (Root._IsTagExpanded(e.Generated.Tag))
                e.Generated._SetExpanded(true, false);
        }

        #endregion

        #region Events

        public event EventHandler ExpansionStateChanged;
        private void OnExpansionStateChanged(object sender)
        {
            if (ExpansionStateChanged != null)
                ExpansionStateChanged(sender, EventArgs.Empty);
        }

        public event EventHandler RowCountChanged;
        protected void OnRowCountChanged()
        {
            if (RowCountChanged != null)
                RowCountChanged(this, EventArgs.Empty);
        }

        #endregion
    }

    public class TreeViewRoot : TreeNodeAdv
    {
        public TreeViewRoot(ITreeModel model) : base(null, null)
        {
            _model = model;
            if (_model != null)
            {
                _model.StructureChanged += Model_StructureChanged;
                _model.NodesChanged += Model_NodesChanged;
            }
            _isExpanded = true;
        }

        #region Properties

        private bool _expandAll = false;

        private ITreeModel _model;
        public new ITreeModel Model { get { return _model; } }

        private List<object> _expandedTags = new List<object>();

        #endregion

        #region Private

        protected override TreeViewRoot _GetRoot()
        {
            return this;
        }

        internal bool _IsTagExpanded(object tag)
        {
            if (tag == null)
                return false;
            bool result = _expandedTags.Contains(tag);
            if (_expandAll)
                return !result;
            else
                return result;
        }

        internal void _SetTagIsExpanded(object tag, bool expand)
        {
            if (tag == null)
                return;
            if (_expandAll)
                expand = !expand;
            if (expand)
            {
                if (!_expandedTags.Contains(tag))
                    _expandedTags.Add(tag);
            }
            else
                _expandedTags.Remove(tag);
        }

        #endregion

        public void CollapseAll()
        {
            foreach (TreeNodeAdv node in AllFetchedNodes)
                node.IsExpanded = false;
            _expandAll = false;
            _expandedTags.Clear();
        }

        public void ExpandAll()
        {
            foreach (TreeNodeAdv node in AllFetchedNodes)
                node.IsExpanded = true;
            _expandAll = true;
            _expandedTags.Clear();
        }

        public override TreePath GetPath()
        {
            return TreePath.Empty;
        }

        #region Event Handlers

        private void Model_NodesChanged(object sender, TreeModelEventArgs e)
        {
            OnDataChanged(e);
        }

        private void Model_StructureChanged(object sender, TreePathEventArgs e)
        {
            TreeNodeAdv node = FindNode(e.Path,true);
            if (node == null)
                return;
            node.InvalidateNodes();
            OnStructureChanged();
        }

        #endregion

        #region Events

        public event EventHandler<TreeModelEventArgs> DataChanged;
        private void OnDataChanged(TreeModelEventArgs args)
        {
            if (DataChanged != null)
                DataChanged(this, args);
        }

        public event EventHandler StructureChanged;
        private void OnStructureChanged()
        {
            if (StructureChanged != null)
                StructureChanged(this, EventArgs.Empty);
        }

        #endregion
    }
}
