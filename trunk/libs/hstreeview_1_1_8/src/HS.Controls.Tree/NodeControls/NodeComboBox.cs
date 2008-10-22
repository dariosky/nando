using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.VisualStyles;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree.NodeControls
{
	public class NodeComboBox : BaseTextControl
	{
		#region Properties

		private int _editorWidth = 100;
		[DefaultValue(100)]
		public int EditorWidth
		{
			get { return _editorWidth; }
			set { _editorWidth = value; }
		}

		private List<object> _dropDownItems;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public List<object> DropDownItems
		{
			get { return _dropDownItems; }
            set { _dropDownItems = value; }
		}

        private bool _drawDropDownArrow = true;
        [DefaultValue(true)]
        public bool DrawDropDownArrow
        {
            get { return _drawDropDownArrow; }
            set { _drawDropDownArrow = value; }
        }

		#endregion

		public NodeComboBox()
		{
			_dropDownItems = new List<object>();
		}

		protected override Size CalculateEditorSize(EditorContext context)
		{
            if (Parent.Columns.Count > 0)
				return context.Bounds.Size;
			else
				return new Size(EditorWidth, context.Bounds.Height);
		}

		protected override Control CreateEditor(TreeNodeAdv node)
		{
			ComboBox comboBox = new ComboBox();
			if (DropDownItems != null)
				comboBox.Items.AddRange(DropDownItems.ToArray());
            object value = GetValue(node);
            if ((value is int) || (value is Enum))
                comboBox.SelectedIndex = (int)value;
            else
                comboBox.SelectedItem = value;
			comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			comboBox.DropDownClosed += new EventHandler(EditorDropDownClosed);
			return comboBox;
		}

        public override void Draw(TreeNodeAdv node, DrawContext context)
        {
            if (context.CurrentEditorOwner == this && node == Parent.FocusedNode)
                return;

            if (DrawDropDownArrow && System.Windows.Forms.Application.RenderWithVisualStyles)
            {
                Rectangle bounds = GetBounds(node, context);
                VisualStyleRenderer renderer = new VisualStyleRenderer(VisualStyleElement.ComboBox.DropDownButton.Normal);
                renderer.DrawBackground(context.Graphics, new Rectangle(bounds.Right - bounds.Height, bounds.Top, bounds.Height, bounds.Height));
            }

            base.Draw(node, context);
        }

		void EditorDropDownClosed(object sender, EventArgs e)
		{
			EndEdit(true);
		}

		public override void UpdateEditor(Control control)
		{
			(control as ComboBox).DroppedDown = true;
		}

		protected override void DoApplyChanges(TreeNodeAdv node, Control editor)
		{
            object value = GetValue(node);
            if ((value is int) || (value is Enum))
                SetValue(node, (editor as ComboBox).SelectedIndex);
            else
			    SetValue(node, (editor as ComboBox).SelectedItem);
		}
	}
}
