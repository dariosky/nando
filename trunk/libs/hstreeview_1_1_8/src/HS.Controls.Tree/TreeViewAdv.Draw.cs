using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using HS.Controls.Tree.NodeControls;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree
{
	public partial class TreeViewAdv
	{

		private void CreatePens()
		{
			CreateLinePen();
			CreateMarkPen();
		}

		private void CreateMarkPen()
		{
			GraphicsPath path = new GraphicsPath();
			path.AddLines(new Point[] { new Point(0, 0), new Point(1, 1), new Point(-1, 1), new Point(0, 0) });
			CustomLineCap cap = new CustomLineCap(null, path);
			cap.WidthScale = 1.0f;

			_markPen = new Pen(_dragDropMarkColor, _dragDropMarkWidth);
			_markPen.CustomStartCap = cap;
			_markPen.CustomEndCap = cap;
		}

		private void CreateLinePen()
		{
			_linePen = new Pen(_lineColor);
			_linePen.DashStyle = DashStyle.Dot;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			DrawContext context = new DrawContext();
			context.Graphics = e.Graphics;
			context.Font = this.Font;
			context.Enabled = Enabled;

			int y = 0;
			if (Columns.Count > 0)
			{
				DrawColumnHeaders(e.Graphics);
				y += Columns.Height;
				if (Columns.Count == 0)
					return;
			}
			y -= GetRowBounds(FirstVisibleRow).Y;

			e.Graphics.ResetTransform();
			e.Graphics.TranslateTransform(-OffsetX, y);
			Rectangle displayRect = DisplayRectangle;
            int row = FirstVisibleRow;
            TreeNodeAdv node = GetNodeOfRow(row);
            while (node != null)
            {
                Rectangle rowRect = GetRowBounds(row);
                if (rowRect.Y + y > displayRect.Bottom)
                    break;
                else
                    DrawRow(e, ref context, row, rowRect);
                row++;
                node = GetNodeOfRow(row);
            }

			if (_dropPosition.Node != null && DragMode)
				DrawDropMark(e.Graphics);

			e.Graphics.ResetTransform();
			DrawScrollBarsBox(e.Graphics);

			if (DragMode && _dragBitmap != null)
				e.Graphics.DrawImage(_dragBitmap, PointToClient(MousePosition));
		}

		private void DrawRow(PaintEventArgs e, ref DrawContext context, int row, Rectangle rowRect)
		{
            TreeNodeAdv node = GetNodeOfRow(row);
			context.DrawSelection = DrawSelectionMode.None;
			context.CurrentEditorOwner = _currentEditorOwner;
			if (DragMode)
			{
				if ((_dropPosition.Node == node) && _dropPosition.Position == NodePosition.Inside)
					context.DrawSelection = DrawSelectionMode.Active;
			}
			else
			{
                if (IsSelected(row))
                {
                    if (Focused)
                        context.DrawSelection = DrawSelectionMode.Active;
                    else
                    if (!HideSelection)
                        context.DrawSelection = DrawSelectionMode.Inactive;
                }
			}
			context.DrawFocus = Focused && FocusedRow == row;

			if (FullRowSelect)
			{
				context.DrawFocus = false;
				if (context.DrawSelection == DrawSelectionMode.Active || context.DrawSelection == DrawSelectionMode.Inactive)
				{
					Rectangle focusRect = new Rectangle(OffsetX, rowRect.Y, ClientRectangle.Width, rowRect.Height);
					if (context.DrawSelection == DrawSelectionMode.Active)
					{
						e.Graphics.FillRectangle(SystemBrushes.Highlight, focusRect);
						context.DrawSelection = DrawSelectionMode.FullRowSelect;
					}
					else
					{
						e.Graphics.FillRectangle(SystemBrushes.InactiveBorder, focusRect);
						context.DrawSelection = DrawSelectionMode.None;
					}
				}
			}

			if (ShowLines)
				DrawLines(e.Graphics, node, rowRect);

			DrawNode(node, context);
		}

        private void DrawColumnHeaders(Graphics gr)
        {
            ClickedColumnState clickInput = Input as ClickedColumnState;
            ReorderColumnState reorderInput = Input as ReorderColumnState;
            int x = 0;
            int dropMarkX = -1;
            TreeColumn.DrawBackground(gr, new Rectangle(0, 0, ClientRectangle.Width + 10, Columns.Height), false);
            gr.TranslateTransform(-OffsetX, 0);
            foreach (TreeColumn c in VisibleColumns)
            {
                Rectangle rect = new Rectangle(x, 0, c.Width, Columns.Height);
                bool pressed = (clickInput != null) && (clickInput.Column == c);
                SortOrder so = (c == SortedColumn) ? SortedOrder : SortOrder.None;
                c.Draw(gr, rect, pressed, so, Font);
                if (reorderInput != null)
                {
                    ColumnLocation relativeLocation = reorderInput.GetRelativeMousePosition(rect);
                    if (relativeLocation == ColumnLocation.FirstHalf)
                        dropMarkX = rect.Left;
                    if (relativeLocation == ColumnLocation.SecondHalf)
                        dropMarkX = rect.Right;
                }
                x += c.Width;
            }

            if (reorderInput != null)
            {
                if (dropMarkX < 0) //The user is hovering the no-columns zone
                    dropMarkX = x;
                gr.FillRectangle(SystemBrushes.HotTrack, dropMarkX - 1, 0, 2, Columns.Height);
                gr.DrawImage(reorderInput.GhostImage, reorderInput.Location);
            }
        }

		public void DrawNode(TreeNodeAdv node, DrawContext context)
		{
			foreach (NodeControlInfo item in GetNodeControls(node))
			{
				context.Bounds = item.Bounds;
				context.Graphics.SetClip(context.Bounds);
				item.Control.Draw(node, context);
				context.Graphics.ResetClip();
			}
		}

		private void DrawScrollBarsBox(Graphics gr)
		{
			Rectangle r1 = DisplayRectangle;
			Rectangle r2 = ClientRectangle;
			gr.FillRectangle(SystemBrushes.Control,
				new Rectangle(r1.Right, r1.Bottom, r2.Width - r1.Width, r2.Height - r1.Height));
		}

		private void DrawDropMark(Graphics gr)
		{
			if (_dropPosition.Position == NodePosition.Inside)
				return;

			Rectangle rect = GetNodeBounds(_dropPosition.Node);
			int right = DisplayRectangle.Right - LeftMargin + OffsetX;
			int y = rect.Y;
			if (_dropPosition.Position == NodePosition.After)
				y = rect.Bottom;
			gr.DrawLine(_markPen, rect.X, y, right, y);
		}

		private void DrawLines(Graphics gr, TreeNodeAdv node, Rectangle rowRect)
		{
			if (Columns.Count > 0)
				gr.SetClip(new Rectangle(0, rowRect.Y, Columns[0].Width, rowRect.Bottom));

			TreeNodeAdv curNode = node;
			while (curNode != _root && curNode != null)
			{
				int level = curNode.Level;
				int x = (level - 1) * _indent + NodePlusMinus.ImageSize / 2 + LeftMargin;
				int width = NodePlusMinus.Width - NodePlusMinus.ImageSize / 2;
				int y = rowRect.Y;
				int y2 = y + rowRect.Height;

				if (curNode == node)
				{
					int midy = y + rowRect.Height / 2;
					gr.DrawLine(_linePen, x, midy, x + width, midy);
					if (curNode.NextSibling == null)
						y2 = y + rowRect.Height / 2;
				}
				if (GetRowOfNode(node) == 0)
					y = rowRect.Height / 2;
				if (curNode.NextSibling != null || curNode == node)
					gr.DrawLine(_linePen, x, y, x, y2);

				curNode = curNode.Parent;
			}

			gr.ResetClip();
		}

	}
}
