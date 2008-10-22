using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.Drawing.Design;
using HS.Controls.Tree.NodeControls;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree
{
	public partial class TreeViewAdv
	{
		#region Internal Properties

		private bool _dragMode;
		private bool DragMode
		{
			get { return _dragMode; }
			set
			{
				_dragMode = value;
				_dragTimer.Enabled = value;
				if (!value)
				{
					if (_dragBitmap != null)
						_dragBitmap.Dispose();
					_dragBitmap = null;
				}
			}
		}

		private IEnumerable<TreeNodeAdv> VisibleNodes
		{
            get { return Root.VisibleNodes; }
		}

		private InputState _input;
		internal InputState Input
		{
			get { return _input; }
			set
			{
				_input = value;
			}
		}

		private bool _itemDragMode;
		internal bool ItemDragMode
		{
			get { return _itemDragMode; }
			set { _itemDragMode = value; }
		}

		private Point _itemDragStart;
		internal Point ItemDragStart
		{
			get { return _itemDragStart; }
			set { _itemDragStart = value; }
		}

		private int _contentWidth = 0;
		private int ContentWidth
		{
			get
			{
                if (Columns.Count == 0)
                    return _contentWidth;
                else
                    return Columns.Width;
			}
		}

		private int _firstVisibleRow;
		internal int FirstVisibleRow
		{
			get { return _firstVisibleRow; }
			set
			{
				HideEditor();
				_firstVisibleRow = value;
				UpdateView();
			}
		}

		private int _offsetX;
		internal int OffsetX
		{
			get { return _offsetX; }
			set
			{
				HideEditor();
				_offsetX = value;
				UpdateView();
			}
		}

        private Rectangle NodesRectangle
        {
            get
            {
                Rectangle r = DisplayRectangle;
                return new Rectangle(r.X, r.Y + Columns.Height, r.Width, r.Height - Columns.Height);
            }
        }

        internal int RowCountPerPage
        {
            get { return Math.Max(NodesRectangle.Height / RowHeight,0); }
        }

		#endregion

        #region Public Properties

        #region DesignTime

        private bool _displayDraggingNodes;
		[DefaultValue(false), Category("Behavior")]
		public bool DisplayDraggingNodes
		{
			get { return _displayDraggingNodes; }
			set { _displayDraggingNodes = value; }
		}

		private bool _fullRowSelect;
		[DefaultValue(false), Category("Behavior")]
		public bool FullRowSelect
		{
			get { return _fullRowSelect; }
			set
			{
				_fullRowSelect = value;
				UpdateView();
			}
		}

        private bool _allowColumnReorder;
        [DefaultValue(false), Category("Behavior")]
        public bool AllowColumnReorder
        {
            get { return _allowColumnReorder; }
            set { _allowColumnReorder = value; }
        }

		private bool _showLines = true;
		[DefaultValue(true), Category("Behavior")]
		public bool ShowLines
		{
			get { return _showLines; }
			set
			{
				_showLines = value;
				UpdateView();
			}
		}

		private bool _showPlusMinus = true;
		[DefaultValue(true), Category("Behavior")]
		public bool ShowPlusMinus
		{
			get { return _showPlusMinus; }
			set
			{
				_showPlusMinus = value;
				FullUpdate();
			}
		}

		private bool _showNodeToolTips = false;
		[DefaultValue(false), Category("Behavior")]
		public bool ShowNodeToolTips
		{
			get { return _showNodeToolTips; }
			set { _showNodeToolTips = value; }
		}

		private BorderStyle _borderStyle;
		[DefaultValue(BorderStyle.Fixed3D), Category("Appearance")]
		public BorderStyle BorderStyle
		{
			get
			{
				return this._borderStyle;
			}
			set
			{
				if (_borderStyle != value)
				{
					_borderStyle = value;
					base.UpdateStyles();
				}
			}
		}

		private int _rowHeight = 16;
		[DefaultValue(16), Category("Appearance")]
		public int RowHeight
		{
			get
			{
				return _rowHeight;
			}
			set
			{
				if (value <= 0)
					throw new ArgumentOutOfRangeException("value");

				_rowHeight = value;
				FullUpdate();
			}
		}

		private bool _allowMultipleSelection = true;
		[DefaultValue(true), Category("Behavior")]
		public bool AllowMultipleSelection
		{
            get { return _allowMultipleSelection; }
            set { _allowMultipleSelection = value; }
		}

		private bool _hideSelection;
		[DefaultValue(false), Category("Behavior")]
		public bool HideSelection
		{
			get { return _hideSelection; }
			set
			{
				_hideSelection = value;
				UpdateView();
			}
		}

		private float _topEdgeSensivity = 0.3f;
		[DefaultValue(0.3f), Category("Behavior")]
		public float TopEdgeSensivity
		{
			get { return _topEdgeSensivity; }
			set
			{
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException();
				_topEdgeSensivity = value;
			}
		}

		private float _bottomEdgeSensivity = 0.3f;
		[DefaultValue(0.3f), Category("Behavior")]
		public float BottomEdgeSensivity
		{
			get { return _bottomEdgeSensivity; }
			set
			{
				if (value < 0 || value > 1)
					throw new ArgumentOutOfRangeException("value should be from 0 to 1");
				_bottomEdgeSensivity = value;
			}
		}

		private int _indent = 19;
		[DefaultValue(19), Category("Behavior")]
		public int Indent
		{
			get { return _indent; }
			set
			{
				_indent = value;
				UpdateView();
			}
		}

		private Color _lineColor = SystemColors.ControlDark;
		[Category("Behavior")]
		public Color LineColor
		{
			get { return _lineColor; }
			set
			{
				_lineColor = value;
				CreateLinePen();
				UpdateView();
			}
		}

		private Color _dragDropMarkColor = Color.Black;
		[Category("Behavior")]
		public Color DragDropMarkColor
		{
			get { return _dragDropMarkColor; }
			set
			{
				_dragDropMarkColor = value;
				CreateMarkPen();
			}
		}

		private float _dragDropMarkWidth = 3.0f;
		[DefaultValue(3.0f), Category("Behavior")]
		public float DragDropMarkWidth
		{
			get { return _dragDropMarkWidth; }
			set
			{
				_dragDropMarkWidth = value;
				CreateMarkPen();
			}
		}

		private TreeColumns _columns;
		[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TreeColumns Columns
		{
			get { return _columns; }
		}

		private NodeControlsCollection _controls;
		[Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(NodeControlCollectionEditor), typeof(UITypeEditor))]
		public Collection<NodeControl> NodeControls
		{
			get
			{
				return _controls;
			}
		}

		#endregion

		#region RunTime

        [Browsable(false)]
        public ITreeModel Model
        {
            get { return Root.Model; }
            set
            {
                if (Root.Model != value)
                {
                    BindModel(value);
                    FullUpdate();
                }
            }
        }

		private IToolTipProvider _defaultToolTipProvider = null;
		[Browsable(false)]
		public IToolTipProvider DefaultToolTipProvider
		{
			get { return _defaultToolTipProvider; }
			set { _defaultToolTipProvider = value; }
		}

		[Browsable(false)]
		public IEnumerable<TreeNodeAdv> AllNodes
		{
			get { return Root.AllNodes; }
		}

		private DropPosition _dropPosition;
		[Browsable(false)]
		public DropPosition DropPosition
		{
			get { return _dropPosition; }
			set { _dropPosition = value; }
		}

		private TreeViewRoot _root;
		[Browsable(false)]
		public TreeViewRoot Root
		{
			get { return _root; }
		}

        private int _selectionAnchor = -1;
        private int _focusedRow = -1;
        [Browsable(false)]
        public int FocusedRow
        {
            get { return _focusedRow; }
            private set
            {
                _focusedRow = value;
                _selectionAnchor = value;
            }
        }

        [Browsable(false)]
        public TreeNodeAdv FocusedNode
        {
            get { return GetNodeOfRow(_focusedRow); }
        }

		[Browsable(false)]
		public TreeNodeAdv SelectedNode
		{
			get
			{
                if (FocusedRow >= 0 && IsSelected(FocusedRow))
                    return FocusedNode;
                else
                {
                    int first = _selection.First;
                    if (first >= 0)
                        return GetNodeOfRow(first);
                }
				return null;
			}
			set
			{
                ClearSelection();
                if (value == null)
                    return;
                int row = GetRowOfNode(value);
                _selection.Add(row);
                FocusedRow = row;
                EnsureVisible(value);
			}
		}

        [Browsable(false)]
        public IEnumerable<int> SelectedRows
        {
            get
            {
                foreach (int row in _selection)
                    yield return row;
            }
        }

        [Browsable(false)]
        public IEnumerable<TreeNodeAdv> SelectedNodes
        {
            get
            {
                TreeNodeAdv node;
                foreach (int row in _selection)
                {
                    node = GetNodeOfRow(row);
                    if (node != null)
                        yield return node;
                }
            }
        }

        [Browsable(false)]
        public IEnumerable<TreeColumn> VisibleColumns
        {
            get { return Columns.VisibleColumns; }
        }

        private TreeColumn _sortedColumn = null;
        [Browsable(false)]
        public TreeColumn SortedColumn
        {
            get { return _sortedColumn; }
        }

        private SortOrder _sortedOrder = SortOrder.None;
        [Browsable(false)]
        public SortOrder SortedOrder
        {
            get { return _sortedOrder; }
        }

		#endregion

        #region Overrides

        private Cursor _innerCursor = null;
        public override Cursor Cursor
        {
            get
            {
                if (_innerCursor != null)
                    return _innerCursor;
                else
                    return base.Cursor;
            }
            set
            {
                base.Cursor = value;
            }
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle r = ClientRectangle;
                int w = _vScrollBar.Visible ? _vScrollBar.Width : 0;
                int h = _hScrollBar.Visible ? _hScrollBar.Height : 0;
                return new Rectangle(r.X, r.Y, r.Width - w, r.Height - h);
            }
        }

        #endregion

        #endregion

    }
}
