using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms.VisualStyles;
using HS.Controls.Tree.Properties;

namespace HS.Controls.Tree
{
    [DesignTimeVisible(false), ToolboxItem(false)]
	public class TreeColumn: Component
	{
        public TreeColumn()
            : this(string.Empty, 50)
        {
        }

        public TreeColumn(string header, int width)
        {
            _header = header;
            _width = width;
            _bounds = new Rectangle(0, 0, width, Height);

            _headerFormat = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.MeasureTrailingSpaces);
            _headerFormat.LineAlignment = StringAlignment.Center;
            _headerFormat.Trimming = StringTrimming.EllipsisCharacter;
        }

		#region Properties and fields

        private StringFormat _headerFormat;

		private string _header;
		[Localizable(true)]
		public string Header
		{
			get { return _header; }
			set 
			{ 
				_header = value;
                OnHeaderChanged();
			}
		}

		private int _width;
		[DefaultValue(50), Localizable(true)]
		public int Width
		{
			get { return _width; }
			set 
			{
				if (_width != value)
				{
					if (value < 0)
						throw new ArgumentOutOfRangeException("value");

					_width = value;
                    OnWidthChanged();
				}
			}
		}

        public static int Height
        {
            get
            {
                if (Application.RenderWithVisualStyles)
                    return 20;
                else
                    return 17;
            }
        }

        private Rectangle _bounds;
        [Browsable(false)]
        public Rectangle Bounds
        {
            get { return _bounds; }
        }

		private bool _visible = true;
		[DefaultValue(true)]
		public bool Visible
		{
			get { return _visible; }
			set 
			{ 
				_visible = value;
                OnVisibilityToggled();
			}
		}

		private HorizontalAlignment _textAlign = HorizontalAlignment.Left;
		[DefaultValue(HorizontalAlignment.Left)]
		public HorizontalAlignment TextAlign
		{
			get { return _textAlign; }
			set { _textAlign = value; }
		}

        private int _tag = 0;
        [DefaultValue(0)]
        public int Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        private static VisualStyleRenderer _normal_renderer = null;
        private static VisualStyleRenderer NormalRenderer
        {
            get
            {
                if (!Application.RenderWithVisualStyles)
                    return null;
                if (_normal_renderer == null)
                    _normal_renderer = new VisualStyleRenderer(VisualStyleElement.Header.Item.Normal);
                return _normal_renderer;
            }
        }

        private static VisualStyleRenderer _pressed_renderer = null;
        private static VisualStyleRenderer PressedRenderer
        {
            get
            {
                if (!Application.RenderWithVisualStyles)
                    return null;
                if (_pressed_renderer == null)
                    _pressed_renderer = new VisualStyleRenderer(VisualStyleElement.Header.Item.Pressed);
                return _pressed_renderer;
            }
        }

		#endregion

        #region General methods

        public void UpdateBounds(TreeColumn previousColumn)
        {
            int x = previousColumn != null ? previousColumn.Bounds.Right : 0;
            _bounds = new Rectangle(x, 0, Width, Height);
        }

        #endregion

        #region Draw

        internal Bitmap CreateGhostImage(Rectangle bounds, Font font)
        {
            Bitmap b = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            Graphics gr = Graphics.FromImage(b);
            gr.FillRectangle(SystemBrushes.ControlDark, bounds);
            DrawContent(gr, bounds, SortOrder.None, font);
            BitmapHelper.SetAlphaChanelValue(b, 150);
            return b;
        }

        internal void Draw(Graphics gr, Rectangle bounds, bool pressed, SortOrder sortOrder, Font font)
        {
            DrawBackground(gr, bounds, pressed);
            DrawContent(gr, bounds, sortOrder, font);
        }

        internal void DrawContent(Graphics gr, Rectangle bounds, SortOrder sortOrder, Font font)
        {
            if (sortOrder != SortOrder.None)
                bounds.Width -= 24;
            DrawText(gr, bounds, font);
            if (sortOrder != SortOrder.None)
            {
                Rectangle arrowRect = new Rectangle(bounds.Right, bounds.Top + 2, 16, 16);
                DrawSortArrow(gr, arrowRect, sortOrder == SortOrder.Ascending);
            }
        }

        internal static void DrawSortArrow(Graphics gr, Rectangle bounds, bool up)
        {
            Bitmap bmp = Resources.down;
            if (up)
                bmp = Resources.up;
            ImageAttributes attr = new ImageAttributes();
            attr.SetColorKey(Color.Magenta, Color.Magenta);
            gr.DrawImage(bmp, bounds, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, attr);
        }

        internal static void DrawBackground(Graphics gr, Rectangle bounds, bool pressed)
        {
            if (Application.RenderWithVisualStyles)
            {
                if (pressed)
                    PressedRenderer.DrawBackground(gr, bounds);
                else
                    NormalRenderer.DrawBackground(gr, bounds);
            }
            else
            {
                gr.FillRectangle(SystemBrushes.Control, bounds);

                if (pressed)
                {
                    bounds.Width--;
                    bounds.Height--;
                    gr.DrawRectangle(SystemPens.ControlDark, bounds);
                }
                else
                {
                    gr.DrawLine(SystemPens.ControlLightLight, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                    gr.DrawLine(SystemPens.ControlLightLight, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                    gr.DrawLine(SystemPens.ControlDarkDark, bounds.X, bounds.Bottom - 1, bounds.Right, bounds.Bottom - 1);
                    gr.DrawLine(SystemPens.ControlDarkDark, bounds.Right - 1, bounds.Y, bounds.Right - 1, bounds.Bottom - 1);
                    gr.DrawLine(SystemPens.ControlDark, bounds.X + 1, bounds.Bottom - 2, bounds.Right - 1, bounds.Bottom - 2);
                    gr.DrawLine(SystemPens.ControlDark, bounds.Right - 2, bounds.Y + 1, bounds.Right - 2, bounds.Bottom - 2);
                }
            }
        }

		internal void DrawText(Graphics gr, Rectangle bounds, Font font)
		{
            if (_headerFormat == null)
                return; //The column was disposed
			_headerFormat.Alignment = TextHelper.TranslateAligment(TextAlign);
			gr.DrawString(Header + " ", font, SystemBrushes.WindowText, bounds, _headerFormat);
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Header))
                return GetType().Name;
            else
                return Header;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _headerFormat.Dispose();
                _headerFormat = null;
            }
        }

		#endregion

        #region Events

        public event EventHandler HeaderChanged;
        private void OnHeaderChanged()
        {
            if (HeaderChanged != null)
                HeaderChanged(this, EventArgs.Empty);
        }

        public event EventHandler VisibilityToggled;
        private void OnVisibilityToggled()
        {
            if (VisibilityToggled != null)
                VisibilityToggled(this, EventArgs.Empty);
        }

        public event EventHandler WidthChanged;
        private void OnWidthChanged()
        {
            if (WidthChanged != null)
                WidthChanged(this, EventArgs.Empty);
        }

        #endregion
    }
}
