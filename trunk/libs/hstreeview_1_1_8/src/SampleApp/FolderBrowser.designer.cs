namespace SampleApp
{
	partial class FolderBrowser
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FolderBrowser));
            this.colName = new HS.Controls.Tree.TreeColumn();
            this.colSize = new HS.Controls.Tree.TreeColumn();
            this.colFoobar = new HS.Controls.Tree.TreeColumn();
            this.colSelectable = new HS.Controls.Tree.TreeColumn();
            this._treeView = new HS.Controls.Tree.TreeViewAdv();
            this.nodeCheckBox1 = new HS.Controls.Tree.NodeControls.NodeCheckBox();
            this._icon = new HS.Controls.Tree.NodeControls.NodeStateIcon();
            this._name = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this._size = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this._date = new HS.Controls.Tree.NodeControls.NodeTextBox();
            this._selectable = new HS.Controls.Tree.NodeControls.NodeComboBox();
            this.SuspendLayout();
            // 
            // colName
            // 
            this.colName.Header = "Name";
            this.colName.Width = 180;
            // 
            // colSize
            // 
            this.colSize.Header = "Size";
            this.colSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colSize.Width = 100;
            // 
            // colFoobar
            // 
            this.colFoobar.Header = "Foobar";
            this.colFoobar.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colFoobar.Width = 150;
            // 
            // colSelectable
            // 
            this.colSelectable.Header = "Selectable";
            this.colSelectable.Width = 80;
            // 
            // _treeView
            // 
            this._treeView.AllowColumnReorder = true;
            this._treeView.BackColor = System.Drawing.SystemColors.Window;
            this._treeView.Columns.Add(this.colName);
            this._treeView.Columns.Add(this.colSize);
            this._treeView.Columns.Add(this.colFoobar);
            this._treeView.Columns.Add(this.colSelectable);
            this._treeView.Cursor = System.Windows.Forms.Cursors.Default;
            this._treeView.DefaultToolTipProvider = null;
            this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._treeView.DragDropMarkColor = System.Drawing.Color.Black;
            this._treeView.FullRowSelect = true;
            this._treeView.LineColor = System.Drawing.SystemColors.ControlDark;
            this._treeView.Location = new System.Drawing.Point(0, 0);
            this._treeView.Model = null;
            this._treeView.Name = "_treeView";
            this._treeView.NodeControls.Add(this.nodeCheckBox1);
            this._treeView.NodeControls.Add(this._icon);
            this._treeView.NodeControls.Add(this._name);
            this._treeView.NodeControls.Add(this._size);
            this._treeView.NodeControls.Add(this._date);
            this._treeView.NodeControls.Add(this._selectable);
            this._treeView.SelectedNode = null;
            this._treeView.ShowNodeToolTips = true;
            this._treeView.Size = new System.Drawing.Size(533, 327);
            this._treeView.TabIndex = 0;
            this._treeView.Text = "treeViewAdv1";
            this._treeView.MouseClick += new System.Windows.Forms.MouseEventHandler(this._treeView_MouseClick);
            this._treeView.ColumnClicked += new System.EventHandler<HS.Controls.Tree.TreeColumnEventArgs>(this._treeView_ColumnClicked);
            // 
            // nodeCheckBox1
            // 
            this.nodeCheckBox1.DataPropertyName = "IsChecked";
            this.nodeCheckBox1.ParentColumn = this.colName;
            // 
            // _icon
            // 
            this._icon.DataPropertyName = "Icon";
            this._icon.ParentColumn = this.colName;
            // 
            // _name
            // 
            this._name.DataPropertyName = "Name";
            this._name.EditEnabled = true;
            this._name.ParentColumn = this.colName;
            this._name.TextColor = System.Drawing.SystemColors.ControlText;
            this._name.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // _size
            // 
            this._size.DataPropertyName = "Size";
            this._size.ParentColumn = this.colSize;
            this._size.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._size.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // _date
            // 
            this._date.DataPropertyName = "Date";
            this._date.ParentColumn = this.colFoobar;
            this._date.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this._date.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // _selectable
            // 
            this._selectable.DataPropertyName = "SelectableValue";
            this._selectable.DropDownItems = ((System.Collections.Generic.List<object>)(resources.GetObject("_selectable.DropDownItems")));
            this._selectable.EditEnabled = true;
            this._selectable.EditOnClick = true;
            this._selectable.ParentColumn = this.colSelectable;
            this._selectable.TextColor = System.Drawing.SystemColors.ControlText;
            // 
            // FolderBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._treeView);
            this.Name = "FolderBrowser";
            this.Size = new System.Drawing.Size(533, 327);
            this.ResumeLayout(false);

		}

		#endregion

		private HS.Controls.Tree.TreeViewAdv _treeView;
		private HS.Controls.Tree.NodeControls.NodeStateIcon _icon;
		private HS.Controls.Tree.NodeControls.NodeTextBox _name;
		private HS.Controls.Tree.NodeControls.NodeTextBox _size;
		private HS.Controls.Tree.NodeControls.NodeTextBox _date;
        private HS.Controls.Tree.NodeControls.NodeCheckBox nodeCheckBox1;
        private HS.Controls.Tree.NodeControls.NodeComboBox _selectable;
        private HS.Controls.Tree.TreeColumn colName;
        private HS.Controls.Tree.TreeColumn colSize;
        private HS.Controls.Tree.TreeColumn colFoobar;
        private HS.Controls.Tree.TreeColumn colSelectable;
	}
}
