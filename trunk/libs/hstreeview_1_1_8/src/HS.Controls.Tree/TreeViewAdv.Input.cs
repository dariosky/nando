using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using HS.Controls.Tree.NodeControls;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree
{
	public partial class TreeViewAdv
	{

		private void SetDropPosition(Point pt)
		{
			TreeNodeAdv node = GetNodeAt(pt);
			_dropPosition.Node = node;
			if (node != null)
			{
                int rowOfNode = GetRowOfNode(node);
                int rowHeight = GetRowBounds(rowOfNode).Height;
                float pos = (pt.Y - Columns.Height - ((rowOfNode - FirstVisibleRow) * rowHeight)) / (float)rowHeight;
				if (pos < TopEdgeSensivity)
					_dropPosition.Position = NodePosition.Before;
				else if (pos > (1 - BottomEdgeSensivity))
					_dropPosition.Position = NodePosition.After;
				else
					_dropPosition.Position = NodePosition.Inside;
			}
		}

		#region Keys

		protected override bool IsInputKey(Keys keyData)
		{
			if (((keyData & Keys.Up) == Keys.Up)
				|| ((keyData & Keys.Down) == Keys.Down)
				|| ((keyData & Keys.Left) == Keys.Left)
				|| ((keyData & Keys.Right) == Keys.Right))
				return true;
			else
				return base.IsInputKey(keyData);
		}

		internal void ChangeInput()
		{
			if ((ModifierKeys & Keys.Shift) == Keys.Shift)
			{
				if (!(Input is InputWithShift))
					Input = new InputWithShift(this);
			}
			else if ((ModifierKeys & Keys.Control) == Keys.Control)
			{
				if (!(Input is InputWithControl))
					Input = new InputWithControl(this);
			}
			else
			{
				if (!(Input.GetType() == typeof(NormalInputState)))
					Input = new NormalInputState(this);
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
            BeginUpdate();
			base.OnKeyDown(e);
			if (!e.Handled)
			{
				if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
					ChangeInput();
				Input.KeyDown(e);
				if (!e.Handled)
				{
					foreach (NodeControlInfo item in GetNodeControls(FocusedNode))
					{
						item.Control.KeyDown(e);
						if (e.Handled)
							break;
					}
				}
			}
            EndUpdate();
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
            BeginUpdate();
			base.OnKeyUp(e);
			if (!e.Handled)
			{
				if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.ControlKey)
					ChangeInput();
				if (!e.Handled)
				{
					foreach (NodeControlInfo item in GetNodeControls(FocusedNode))
					{
						item.Control.KeyUp(e);
						if (e.Handled)
							return;
					}
				}
			}
            EndUpdate();
		}

		#endregion

		#region Mouse

		private TreeNodeAdvMouseEventArgs CreateMouseArgs(MouseEventArgs e)
		{
			TreeNodeAdvMouseEventArgs args = new TreeNodeAdvMouseEventArgs(e);
			args.ViewLocation = new Point(e.X + OffsetX,
                e.Y + GetRowBounds(FirstVisibleRow).Y - Columns.Height);
			args.ModifierKeys = ModifierKeys;
			args.Node = GetNodeAt(e.Location);
			NodeControlInfo info = GetNodeControlInfoAt(args.Node, e.Location);
			args.ControlBounds = info.Bounds;
			args.Control = info.Control;
			return args;
		}

        private void _DoMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeColumn c = GetColumnAt(e.Location, true);
                if (c != null)
                {
                    Input = new ResizeColumnState(this, c, e.Location);
                    return;
                }
                c = GetColumnAt(e.Location, false);
                if (c != null)
                {
                    Input = new ClickedColumnState(this, c, e.Location);
                    return;
                }
            }
            ChangeInput();
            TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);

            if (args.Node != null && args.Control != null)
                args.Control.MouseDown(args);

            if (!args.Handled)
                Input.MouseDown(args);
        }

        private void _DoMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeColumn c;
                c = GetColumnAt(e.Location, true);
                if (c != null)
                {
                    AutoSizeColumn(c);
                    return;
                }
            }
            TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);

            if (args.Node != null && args.Control != null)
                args.Control.MouseDoubleClick(args);

            args.Handled = false;
            if (args.Node != null)
                OnNodeMouseDoubleClick(args);

            if (!args.Handled)
            {
                if (args.Node != null && args.Button == MouseButtons.Left)
                    args.Node.IsExpanded = !args.Node.IsExpanded;
            }
        }

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (SystemInformation.MouseWheelScrollLines > 0)
			{
				int lines = e.Delta / 120 * SystemInformation.MouseWheelScrollLines;
				int newValue = _vScrollBar.Value - lines;
				_vScrollBar.Value = Math.Max(_vScrollBar.Minimum,
					Math.Min(_vScrollBar.Maximum - _vScrollBar.LargeChange + 1, newValue));
			}
			base.OnMouseWheel(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
            BeginUpdate();
			if (!Focused)
				Focus();
            _DoMouseDown(e);
            base.OnMouseDown(e);
            EndUpdate();
		}

		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
            BeginUpdate();
            _DoMouseDoubleClick(e);
			base.OnMouseDoubleClick(e);
            EndUpdate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
            BeginUpdate();
			TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);
			if ((Input is ColumnState))
				Input.MouseUp(args);
			else
			{
				if (args.Node != null && args.Control != null)
					args.Control.MouseUp(args);
				if (!args.Handled)
					Input.MouseUp(args);

				base.OnMouseUp(e);
			}
            EndUpdate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (Input.MouseMove(e))
				return;

			base.OnMouseMove(e);
			SetCursor(e);
            if (e.Location.Y <= Columns.Height)
			{
				_toolTip.Active = false;
			}
			else
			{
				UpdateToolTip(e);
				if (ItemDragMode && Dist(e.Location, ItemDragStart) > ItemDragSensivity
					&& FocusedRow >= 0 && IsSelected(FocusedRow))
				{
					ItemDragMode = false;
					_toolTip.Active = false;
					OnItemDrag(e.Button, new List<TreeNodeAdv>(SelectedNodes).ToArray());
				}
			}
		}

		private void SetCursor(MouseEventArgs e)
		{
			if (GetColumnAt(e.Location,true) == null)
				_innerCursor = null;
			else
				_innerCursor = Cursors.VSplit;
		}

		TreeNodeAdv _hoverNode;
		NodeControl _hoverControl;
		private void UpdateToolTip(MouseEventArgs e)
		{
			if (ShowNodeToolTips)
			{
				TreeNodeAdvMouseEventArgs args = CreateMouseArgs(e);
				if ((args.Node != null) && (args.Control != null))
				{
					if (args.Node != _hoverNode || args.Control != _hoverControl)
					{
						string msg = GetNodeToolTip(args);
						if (!String.IsNullOrEmpty(msg))
						{
							_toolTip.SetToolTip(this, msg);
							_toolTip.Active = true;
						}
						else
							_toolTip.SetToolTip(this, null);
					}
				}
				else
					_toolTip.SetToolTip(this, null);

				_hoverControl = args.Control;
				_hoverNode = args.Node;
			}
			else
				_toolTip.SetToolTip(this, null);
		}

		private string GetNodeToolTip(TreeNodeAdvMouseEventArgs args)
		{
			string msg = args.Control.GetToolTip(args.Node);

			BaseTextControl btc = args.Control as BaseTextControl;
			if (btc != null && btc.DisplayHiddenContentInToolTip && String.IsNullOrEmpty(msg))
			{
				Size ms = btc.MeasureSize(args.Node, _measureContext);
				if (ms.Width > args.ControlBounds.Size.Width || ms.Height > args.ControlBounds.Size.Height
					|| args.ControlBounds.Right - OffsetX > DisplayRectangle.Width)
					msg = btc.GetLabel(args.Node);
			}

			if (String.IsNullOrEmpty(msg) && DefaultToolTipProvider != null)
				msg = DefaultToolTipProvider.GetToolTip(args.Node, args.Control);

			return msg;
		}

		#endregion

		#region DragDrop

		private bool _dragAutoScrollFlag = false;
		private Bitmap _dragBitmap = null;

		private void DragTimerTick(object sender, EventArgs e)
		{
			_dragAutoScrollFlag = true;
		}

		private void DragAutoScroll()
		{
			_dragAutoScrollFlag = false;
			Point pt = PointToClient(MousePosition);
			if (pt.Y < 20 && _vScrollBar.Value > 0)
				_vScrollBar.Value--;
			else if (pt.Y > Height - 20 && _vScrollBar.Value <= _vScrollBar.Maximum - _vScrollBar.LargeChange)
				_vScrollBar.Value++;
		}

		public void DoDragDropSelectedNodes(DragDropEffects allowedEffects)
		{
            List<TreeNodeAdv> sel = new List<TreeNodeAdv>(SelectedNodes);
			if (sel.Count > 0)
			{
				TreeNodeAdv[] nodes = new TreeNodeAdv[sel.Count];
				sel.CopyTo(nodes, 0);
				DoDragDrop(nodes, allowedEffects);
			}
		}

		private void CreateDragBitmap(IDataObject data)
		{
            if (Columns.Count > 0 || !DisplayDraggingNodes)
				return;
			
			TreeNodeAdv[] nodes = data.GetData(typeof(TreeNodeAdv[])) as TreeNodeAdv[];
			if (nodes != null && nodes.Length > 0)
			{
				Rectangle rect = DisplayRectangle;
				Bitmap bitmap = new Bitmap(rect.Width, rect.Height);
				using (Graphics gr = Graphics.FromImage(bitmap))
				{
					gr.Clear(BackColor);
					DrawContext context = new DrawContext();
					context.Graphics = gr;
					context.Font = Font;
					context.Enabled = true;
					int y = 0;
					int maxWidth = 0;
					foreach (TreeNodeAdv node in nodes)
					{
						if (IsMyNode(node))
						{
							int x = 0;
                            int rowOfNode = GetRowOfNode(node);
							int height = GetRowBounds(rowOfNode).Height;
							foreach (NodeControl c in NodeControls)
							{
								int width = c.MeasureSize(node, context).Width;
								rect = new Rectangle(x, y, width, height);
								x += (width + 1);
								context.Bounds = rect;
								c.Draw(node, context);
							}
							y += height;
							maxWidth = Math.Max(maxWidth, x);
						}
					}

					if (maxWidth > 0 && y > 0)
					{
						_dragBitmap = new Bitmap(maxWidth, y, PixelFormat.Format32bppArgb);
						using (Graphics tgr = Graphics.FromImage(_dragBitmap))
							tgr.DrawImage(bitmap, Point.Empty);
						BitmapHelper.SetAlphaChanelValue(_dragBitmap, 150);
					}
					else
						_dragBitmap = null;
				}
			}
		}

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			ItemDragMode = false;
			Point pt = PointToClient(new Point(drgevent.X, drgevent.Y));
			if (_dragAutoScrollFlag)
				DragAutoScroll();
			SetDropPosition(pt);
			UpdateView();
			base.OnDragOver(drgevent);
		}

		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			try
			{
				DragMode = true;
				CreateDragBitmap(drgevent.Data);
				base.OnDragEnter(drgevent);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		protected override void OnDragLeave(EventArgs e)
		{
			DragMode = false;
			UpdateView();
			base.OnDragLeave(e);
		}

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			DragMode = false;
			UpdateView();
			base.OnDragDrop(drgevent);
		}

		#endregion

	}
}
