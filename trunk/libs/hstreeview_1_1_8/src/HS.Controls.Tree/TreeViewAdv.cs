using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Collections;
using System.Drawing.Design;
using System.Security.Permissions;
using HS.Controls.Tree.NodeControls;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree
{
	public partial class TreeViewAdv : Control
	{
		public const int LeftMargin = 7;
        public const int ItemDragSensivity = 4;
        public const int DividerWidth = 9;
        public const int NodeControlsPadding = 1;

		private Pen _linePen;
		private Pen _markPen;
        private int _updateHoldLevel;
		private bool _needsFullUpdate;
        private bool _selectionChanged;
		private NodePlusMinus _plusMinus;
		private Control _currentEditor;
		private EditableControl _currentEditorOwner;
		private ToolTip _toolTip;
		private Timer _dragTimer;
		private DrawContext _measureContext;
        private List<TreeNodeAdv> _rowMap;
        private Dictionary<TreeNodeAdv, int> _nodeMap;
        private IEnumerator<TreeNodeAdv> _rowEnumerator;
        private Ranges _selection;

		public TreeViewAdv()
		{
			InitializeComponent();
			SetStyle(ControlStyles.AllPaintingInWmPaint
				| ControlStyles.UserPaint
				| ControlStyles.OptimizedDoubleBuffer
				| ControlStyles.ResizeRedraw
				| ControlStyles.Selectable
				, true);

			BorderStyle = BorderStyle.Fixed3D;
			_hScrollBar.Height = SystemInformation.HorizontalScrollBarHeight;
            _vScrollBar.Width = SystemInformation.VerticalScrollBarWidth;
			_rowMap = new List<TreeNodeAdv>();
            _nodeMap = new Dictionary<TreeNodeAdv, int>();
            _selection = new Ranges();
			_columns = new TreeColumns();
            _columns.StructureChanged += Columns_StructureChanged;
            _columns.ColumnTextChanged += Columns_ColumnTextChanged;
			_toolTip = new ToolTip();
			_dragTimer = new Timer();
			_dragTimer.Interval = 100;
			_dragTimer.Tick += new EventHandler(DragTimerTick);
            _updateHoldLevel = 0;
			
			_measureContext = new DrawContext();
			_measureContext.Font = Font;
			_measureContext.Graphics = Graphics.FromImage(new Bitmap(1, 1));

			Input = new NormalInputState(this);
			BindModel(null);
			CreatePens();

			ArrangeControls();

			_plusMinus = new NodePlusMinus();
			_controls = new NodeControlsCollection(this);
        }

        #region Overrides

        protected override void OnSizeChanged(EventArgs e)
        {
            ArrangeControls();
            SafeUpdateScrollBars();
            base.OnSizeChanged(e);
        }

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams res = base.CreateParams;
                switch (BorderStyle)
                {
                    case BorderStyle.FixedSingle:
                        res.Style |= 0x800000;
                        break;
                    case BorderStyle.Fixed3D:
                        res.ExStyle |= 0x200;
                        break;
                }
                return res;
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            HideEditor();
            UpdateView();
            ChangeInput();
            base.OnGotFocus(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            HideEditor();
            UpdateView();
            base.OnLeave(e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            _measureContext.Font = Font;
            FullUpdate();
        }

        #endregion

        #region Node

        public TreeNodeAdv GetNodeAt(Point point)
		{
            if (point.X < 0 || point.Y < 0)
                return null;
            int row = GetRowAt(point.Y - Columns.Height);
            return GetNodeOfRow(row);
		}

		public NodeControlInfo GetNodeControlInfoAt(Point point)
		{
            TreeNodeAdv node = GetNodeAt(point);
            if (node == null)
                return NodeControlInfo.Empty;
            else
                return GetNodeControlInfoAt(node, point);
        }

        public void ExpandAll()
        {
            BeginUpdate();
            Root.ExpandAll();
            EndUpdate();
        }

        public void CollapseAll()
        {
            BeginUpdate();
            Root.CollapseAll();
            EndUpdate();
        }

        public void EnsureVisible(TreeNodeAdv node)
        {
            TreeNodeAdv parent = node.Parent;
            while (parent != _root)
            {
                parent.IsExpanded = true;
                parent = parent.Parent;
            }
            ScrollTo(node);
        }

        public void ScrollTo(TreeNodeAdv node)
        {
            ScrollTo(GetRowOfNode(node));
        }

        public void ScrollTo(int row)
        {
            int targetRow = -1;

            if (row < FirstVisibleRow)
                targetRow = row;
            else
            {
                int currentTop = GetRowBounds(FirstVisibleRow).Top;
                int rowBottom = GetRowBounds(row).Bottom;
                int scrollNeeded = rowBottom - currentTop - NodesRectangle.Height;
                if (scrollNeeded <= 0) // No need to scroll
                    return;
                targetRow = GetRowAt(scrollNeeded);
                targetRow++; //To be sure that we see the row entirely.
            }

            if (targetRow >= _vScrollBar.Minimum && row <= _vScrollBar.Maximum)
                _vScrollBar.Value = targetRow;
        }

        public TreeNodeAdv FindNode(TreePath path)
        {
            return _root.FindNode(path);
        }

        public IEnumerable<NodeControlInfo> GetNodeControls(TreeNodeAdv node)
        {
            if (node == null)
                yield break;

            int rowOfNode = GetRowOfNode(node);
            Rectangle rowRect = GetRowBounds(rowOfNode);
            int y = rowRect.Y;
            int x = (node.Level - 1) * _indent + LeftMargin;
            int width = 0;
            Rectangle rect = Rectangle.Empty;

            if (ShowPlusMinus)
            {
                width = _plusMinus.MeasureSize(node, _measureContext).Width;
                rect = new Rectangle(x, y, width, rowRect.Height);
                if (Columns.Count > 0 && Columns[0].Width < rect.Right)
                    rect.Width = Columns[0].Width - x;

                yield return new NodeControlInfo(_plusMinus, rect, node);
                x += (width + NodeControlsPadding);
            }

            if (Columns.Count == 0)
            {
                foreach (NodeControl c in NodeControls)
                {
                    width = c.MeasureSize(node, _measureContext).Width;
                    rect = new Rectangle(x, y, width, rowRect.Height);
                    x += (width + NodeControlsPadding);
                    yield return new NodeControlInfo(c, rect, node);
                }
            }
            else
            {
                int right = 0;
                foreach (TreeColumn col in VisibleColumns)
                {
                    right += col.Width;
                    List<NodeControl> nodeControls = new List<NodeControl>(GetControlsOfColumn(col));
                    if (nodeControls.Count > 0)
                    {
                        NodeControl lastNC = nodeControls[nodeControls.Count - 1];
                        nodeControls.Remove(lastNC);
                        foreach (NodeControl nc in nodeControls)
                        {
                            //What we want here is to have a width that is ideally the result of MeasureSize,
                            //but it can't overflow 'right' and it can't be less than 0.
                            width = Math.Min(right - x, nc.MeasureSize(node, _measureContext).Width);
                            if (width < 0)
                                width = 0;
                            rect = new Rectangle(x, y, width, rowRect.Height);
                            x += (width + NodeControlsPadding);
                            yield return new NodeControlInfo(nc, rect, node);
                        }
                        width = Math.Max(0, right - x);
                        rect = new Rectangle(x, y, width, rowRect.Height);
                        yield return new NodeControlInfo(lastNC, rect, node);
                    }
                    x = right;
                }
            }
        }

        private NodeControlInfo GetNodeControlInfoAt(TreeNodeAdv node, Point point)
        {
            Rectangle rect = GetRowBounds(FirstVisibleRow);
            point.Y += (rect.Y - Columns.Height);
            point.X += OffsetX;
            foreach (NodeControlInfo info in GetNodeControls(node))
                if (info.Bounds.Contains(point))
                    return info;

            return NodeControlInfo.Empty;
        }

        private Rectangle GetNodeBounds(TreeNodeAdv node)
        {
            Rectangle res = Rectangle.Empty;
            foreach (NodeControlInfo info in GetNodeControls(node))
            {
                if (res == Rectangle.Empty)
                    res = info.Bounds;
                else
                    res = Rectangle.Union(res, info.Bounds);
            }
            return res;
        }

        public int GetRowAt(int y)
        {
            return (y + (FirstVisibleRow * _rowHeight)) / _rowHeight;
        }

        public Rectangle GetRowBounds(int rowNo)
        {
            return new Rectangle(0, rowNo * _rowHeight, 0, _rowHeight);
        }

        internal bool IsMyNode(TreeNodeAdv node)
        {
            if (node == null)
                return false;

            return node.Root == _root;
        }

        #endregion

        #region Selection

        public void ClearSelection()
		{
            _selection.Clear();
            OnSelectionChanged();
		}

        public bool IsSelected(int row)
        {
            return _selection.Contains(row);
        }

        public bool IsSelected(TreeNodeAdv node)
        {
            return IsSelected(GetRowOfNode(node));
        }

        public void Select(int row)
        {
            _selection.Add(row);
            FocusedRow = row;
            OnSelectionChanged();
        }

        public void Select(TreeNodeAdv node)
        {
            if (node != null)
                Select(GetRowOfNode(node));
        }

        public void SelectRangeFromAnchorToRow(int row)
        {
            int from = Math.Min(_selectionAnchor, row);
            int to = Math.Max(_selectionAnchor, row);
            _selection.Add(new Range(from, to));
            _focusedRow = row; //Here, we want to update FocusedRow, but not _selectionAnchor
        }

        public void Deselect(int row)
        {
            _selection.Remove(row);
            FocusedRow = row;
            OnSelectionChanged();
        }

        public void Deselect(TreeNodeAdv node)
        {
            if (node != null)
                Deselect(GetRowOfNode(node));
        }

        #endregion

        #region Column

        internal TreeColumn GetColumnAt(Point p, bool divider)
        {
            p = new Point(p.X + OffsetX, p.Y);
            if (divider)
                p = new Point(p.X - (DividerWidth / 2), p.Y);
            TreeColumn result = Columns.GetColumnAt(p);
            if ((divider) && (result != null))
            {
                int distance = Math.Abs(p.X - result.Bounds.Right);
                if (distance > DividerWidth)
                    result = null;
            }
            return result;
        }

        public void AutoSizeColumn(TreeColumn column)
        {
            if (Root.Nodes.Count == 0)
                return;
            bool columnIsFirst = column.Bounds.Left == 0;
            List<NodeControl> ncs = new List<NodeControl>(GetControlsOfColumn(column));
            if (ShowPlusMinus && columnIsFirst)
                ncs.Insert(0, _plusMinus);
            int result = 0;
            foreach (TreeNodeAdv node in VisibleNodes)
            {
                int width = columnIsFirst ? (LeftMargin + ((node.Level - 1) * Indent)) : 0;
                foreach (NodeControl nc in ncs)
                    width += nc.MeasureSize(node, _measureContext).Width + NodeControlsPadding;
                result = Math.Max(result, width);
            }
            int columnHeaderPadding = 8;
            if ((SortedColumn == column) && (SortedOrder != SortOrder.None))
                columnHeaderPadding += 24;
            int minColWidth = Math.Max(10, (int)_measureContext.Graphics.MeasureString(column.Header, _measureContext.Font).Width + columnHeaderPadding);
            column.Width = Math.Max(minColWidth,result);
        }

        public void AutoSizeAllColumns()
        {
            foreach (TreeColumn column in VisibleColumns)
                AutoSizeColumn(column);
        }

        public void ChangeSort(TreeColumn column)
        {
            if (column == SortedColumn)
                _sortedOrder = (SortedOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            else
            {
                _sortedColumn = column;
                _sortedOrder = SortOrder.Ascending;
            }
        }

        public void ChangeSort(TreeColumn column, SortOrder order)
        {
            _sortedColumn = column;
            _sortedOrder = order;
        }

        public IEnumerable<NodeControl> GetControlsOfColumn(TreeColumn column)
        {
            foreach (NodeControl nc in NodeControls)
            {
                if (nc.ParentColumn == column)
                    yield return nc;
            }
        }

        #endregion

        #region Update

        public void BeginUpdate()
        {
            _updateHoldLevel++;
        }

        public void EndUpdate()
        {
            _updateHoldLevel--;
            if (_updateHoldLevel > 0)
                return;
            _updateHoldLevel = 0;
            if (_selectionChanged)
                OnSelectionChanged();
            if (_needsFullUpdate)
                FullUpdate();
            else
                UpdateView();
        }

        private void ArrangeControls()
		{
			int hBarSize = _hScrollBar.Height;
			int vBarSize = _vScrollBar.Width;
			Rectangle clientRect = ClientRectangle;
			
			_hScrollBar.SetBounds(clientRect.X, clientRect.Bottom - hBarSize,
				clientRect.Width - vBarSize, hBarSize);

			_vScrollBar.SetBounds(clientRect.Right - vBarSize, clientRect.Y,
				vBarSize, clientRect.Height - hBarSize);
		}

		private void SafeUpdateScrollBars()
		{
			if (InvokeRequired)
				Invoke(new MethodInvoker(UpdateScrollBars));
			else
				UpdateScrollBars();
		}

		private void UpdateScrollBars()
		{
            if (_updateHoldLevel > 0)
            {
                _needsFullUpdate = true;
                return;
            }
			UpdateVScrollBar();
			UpdateHScrollBar();
			UpdateVScrollBar();
			UpdateHScrollBar();
			_hScrollBar.Width = DisplayRectangle.Width;
			_vScrollBar.Height = DisplayRectangle.Height;
		}

		private void UpdateHScrollBar()
		{
			_hScrollBar.Maximum = ContentWidth;
			_hScrollBar.LargeChange = Math.Max(DisplayRectangle.Width, 0);
			_hScrollBar.SmallChange = 5;
			_hScrollBar.Visible = _hScrollBar.LargeChange < _hScrollBar.Maximum;
			_hScrollBar.Value = Math.Min(_hScrollBar.Value, _hScrollBar.Maximum - _hScrollBar.LargeChange + 1);
		}

		private void UpdateVScrollBar()
		{
			_vScrollBar.Maximum = Math.Max(Root.RowCount - 1, 0);
            _vScrollBar.LargeChange = RowCountPerPage;
			_vScrollBar.Visible = (Root.RowCount > 0) && (_vScrollBar.LargeChange <= _vScrollBar.Maximum);
			_vScrollBar.Value = Math.Min(_vScrollBar.Value, _vScrollBar.Maximum - _vScrollBar.LargeChange + 1);
        }

        internal void FullUpdate()
        {
            if (_updateHoldLevel > 0)
            {
                _needsFullUpdate = true;
                return;
            }
            ResetRowMap();
            SafeUpdateScrollBars();
            UpdateView();
            _needsFullUpdate = false;
        }

        internal void UpdateView()
        {
            if (_updateHoldLevel == 0)
                Invalidate(false);
        }

        #endregion

        #region Row Map

        private TreeNodeAdv _AddNextRowToRowMap()
        {
            if ((_rowEnumerator == null) || (!_rowEnumerator.MoveNext()))
                return null;
            TreeNodeAdv node = _rowEnumerator.Current;
            _rowMap.Add(node);
            _nodeMap[node] = _rowMap.Count - 1;
            if (Columns.Count == 0)
            {
                Rectangle rect = GetNodeBounds(node);
                _contentWidth = Math.Max(_contentWidth, rect.Right);
            }
            return node;
        }

        internal TreeNodeAdv GetNodeOfRow(int row)
        {
            if (row < 0)
                return null;
            while (_rowMap.Count <= row)
                if (_AddNextRowToRowMap() == null)
                    return null;
            return _rowMap[row];
        }

        internal int GetRowOfNode(TreeNodeAdv wantedNode)
        {
            return GetRowOfNode(wantedNode, true);
        }

        internal int GetRowOfNode(TreeNodeAdv wantedNode, bool lookForIt)
        {
            if (wantedNode == null)
                return -1;
            if (_nodeMap.ContainsKey(wantedNode))
                return _nodeMap[wantedNode];
            if (!lookForIt)
                return -1;
            TreeNodeAdv node = _AddNextRowToRowMap();
            while ((node != null) && (node != wantedNode))
                node = _AddNextRowToRowMap();
            return _rowMap.Count - 1;
        }

        internal TreeNodeAdv GetLastVisibleNode()
        {
            BeginUpdate();
            while (_AddNextRowToRowMap() != null) ;
            EndUpdate();
            if (_rowMap.Count > 0)
                return _rowMap[_rowMap.Count - 1];
            else
                return null;
        }

        private void ResetRowMap()
        {
            _rowMap.Clear();
            _nodeMap.Clear();
            _rowEnumerator = Root.VisibleNodes.GetEnumerator();
            _contentWidth = 0;
        }

        #endregion

        internal static double Dist(Point p1, Point p2)
		{
			return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
		}

		private void BindModel(ITreeModel model)
		{
			_selection.Clear();
            FocusedRow = -1;
            if (_root != null)
                _UnbindRootEvents();
			_root = new TreeViewRoot(model);
            _BindRootEvents();
			_root.IsExpanded = true;
            if (_root.Nodes.Count > 0)
                FocusedRow = 0;
            else
                FocusedRow = -1;
		}

		#region Editor

		public void DisplayEditor(Control control, EditableControl owner)
		{
			if (control == null || owner == null)
				throw new ArgumentNullException();

			if (FocusedRow >= 0)
			{
				HideEditor();
				_currentEditor = control;
				_currentEditorOwner = owner;
				UpdateEditorBounds();

				UpdateView();
				control.Parent = this;
				control.Focus();
				owner.UpdateEditor(control);
			}
		}

		public void UpdateEditorBounds()
		{
			if (_currentEditor != null)
			{
				EditorContext context = new EditorContext();
				context.Owner = _currentEditorOwner;
                context.CurrentNode = FocusedNode;
				context.Editor = _currentEditor;
				context.DrawContext = _measureContext;

				SetEditorBounds(context);
			}
		}

		public void HideEditor()
		{
			if (_currentEditorOwner != null)
			{
				_currentEditorOwner.HideEditor(_currentEditor);
				_currentEditor = null;
				_currentEditorOwner = null;
			}
		}

		private void SetEditorBounds(EditorContext context)
		{
			foreach (NodeControlInfo info in GetNodeControls(context.CurrentNode))
			{
				if (context.Owner == info.Control && info.Control is EditableControl)
				{
					Point p = info.Bounds.Location;
					p.X -= OffsetX;
                    p.Y -= (GetRowBounds(FirstVisibleRow).Y - Columns.Height);
					int width = DisplayRectangle.Width - p.X;
                    if (Columns.Count > 0 && info.Control.ParentColumn != null && Columns.Contains(info.Control.ParentColumn))
					{
                        Rectangle rect = info.Control.ParentColumn.Bounds;
						width = rect.Right - OffsetX - p.X;
					}
					context.Bounds = new Rectangle(p.X, p.Y, width, info.Bounds.Height);
					((EditableControl)info.Control).SetEditorBounds(context);
					return;
				}
			}
		}

		#endregion

		#region Event handler

        private void _BindRootEvents()
        {
            _root.RowCountChanged += Root_RowCountChanged;
            _root.ExpansionStateChanged += Root_ExpansionStateChanged;
            _root.DataChanged += Root_NodesChanged;
            _root.StructureChanged += Root_StructureChanged;
        }

        private void _UnbindRootEvents()
        {
            _root.RowCountChanged -= Root_RowCountChanged;
            _root.ExpansionStateChanged -= Root_ExpansionStateChanged;
            _root.DataChanged -= Root_NodesChanged;
            _root.StructureChanged -= Root_StructureChanged;
        }

        private void Root_StructureChanged(object sender, EventArgs e)
        {
            FullUpdate();
        }

		private void Root_NodesChanged(object sender, TreeModelEventArgs e)
		{
			TreeNodeAdv parent = FindNode(e.Path);
			if (parent != null)
			{
				if (e.Indices != null)
				{
					foreach (int index in e.Indices)
					{
						if (index >= 0 && index < parent.Nodes.Count)
						{
							TreeNodeAdv node = parent.Nodes[index];
							Rectangle rect = GetNodeBounds(node);
							_contentWidth = Math.Max(_contentWidth, rect.Right);
						}
						else
							throw new ArgumentOutOfRangeException("Index out of range");
					}
				}
				else
				{
					foreach (TreeNodeAdv node in parent.Nodes)
					{
						foreach (object obj in e.Children)
							if (node.Tag == obj)
							{
								Rectangle rect = GetNodeBounds(node);
								_contentWidth = Math.Max(_contentWidth, rect.Right);
							}
					}
				}
			}
			SafeUpdateScrollBars();
			UpdateView();
		}

        private void Root_RowCountChanged(object sender, EventArgs e)
        {
            
            SafeUpdateScrollBars();
        }

        private void Root_ExpansionStateChanged(object sender, EventArgs e)
        {
            TreeNodeAdv node = sender as TreeNodeAdv;
            int row = GetRowOfNode(node,false);
            if (row < 0)
                return;
            int count = node.Nodes.Count;
            if (node.IsExpanded)
                _selection.Expand(new Range(row + 1, row + count));
            else
                _selection.Collapse(new Range(row + 1, row + count));
            FullUpdate();
        }

        private void Columns_StructureChanged(object sender, EventArgs e)
        {
            FullUpdate();
        }

        private void Columns_ColumnTextChanged(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void _vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if ((_vScrollBar.Value * 100) / _vScrollBar.Maximum > 95)
                GetLastVisibleNode();
            FirstVisibleRow = _vScrollBar.Value;
        }

        private void _hScrollBar_ValueChanged(object sender, EventArgs e)
        {
            OffsetX = _hScrollBar.Value;
        }

        #endregion

    }
}