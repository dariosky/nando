using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

namespace HS.Controls.Tree
{
	public class TreeColumns : Collection<TreeColumn>
	{
		public TreeColumns()
		{
        }

        #region Properties and fields

        public IEnumerable<TreeColumn> VisibleColumns
        {
            get
            {
                foreach (TreeColumn column in this)
                    if (column.Visible)
                        yield return column;
            }
        }

        private int _height = -1;
        public int Height
        {
            get
            {
                if (_height < 0)
                {
                    if (Count > 0)
                        _height = TreeColumn.Height;
                    else
                        _height = 0;
                }
                return _height;
            }
        }

        private int _width = 0;
        public int Width
        {
            get { return _width; }
        }

        #endregion

        private void _PlugItem(TreeColumn item)
        {
            item.HeaderChanged += Column_HeaderChanged;
            item.VisibilityToggled += Column_VisibilityToggled;
            item.WidthChanged += Column_WidthChanged;
        }

        private void _UpdateBounds()
        {
            TreeColumn lastColumn = null;
            foreach (TreeColumn c in VisibleColumns)
            {
                c.UpdateBounds(lastColumn);
                lastColumn = c;
                _width = c.Bounds.Right;
            }
            _height = -1;
        }

        public TreeColumn GetColumnAt(Point p)
        {
            if (p.Y > Height)
                return null;

            foreach (TreeColumn c in VisibleColumns)
            {
                if (c.Bounds.Contains(p))
                    return c;
            }
            return null;
        }

        #region Collection overrides

        protected override void InsertItem(int index, TreeColumn item)
		{
			base.InsertItem(index, item);
            _UpdateBounds();
            _PlugItem(item);
            OnStructureChanged();
		}

		protected override void RemoveItem(int index)
		{
			base.RemoveItem(index);
            _UpdateBounds();
            OnStructureChanged();
		}

		protected override void SetItem(int index, TreeColumn item)
		{
			base.SetItem(index, item);
            _UpdateBounds();
            _PlugItem(item);
            OnStructureChanged();
		}

		protected override void ClearItems()
		{
			Items.Clear();
            OnStructureChanged();
        }

        #endregion

        #region Event Handlers

        private void Column_HeaderChanged(object sender, EventArgs e)
        {
            OnColumnTextChanged();
        }

        private void Column_VisibilityToggled(object sender, EventArgs e)
        {
            _UpdateBounds();
            OnStructureChanged();
        }

        private void Column_WidthChanged(object sender, EventArgs e)
        {
            _UpdateBounds();
            OnStructureChanged();
        }

        #endregion

        #region Events

        public event EventHandler StructureChanged;
        private void OnStructureChanged()
        {
            if (StructureChanged != null)
                StructureChanged(this, EventArgs.Empty);
        }

        public event EventHandler ColumnTextChanged;
        private void OnColumnTextChanged()
        {
            if (ColumnTextChanged != null)
                ColumnTextChanged(this, EventArgs.Empty);
        }

        #endregion
    }
}
