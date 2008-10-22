using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;
using HS.Controls.Tree.Properties;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree.NodeControls
{
	public class NodeCheckBox : BindableControl
	{
		public const int ImageSize = 13;

		private Bitmap _check;
		private Bitmap _uncheck;
		private Bitmap _unknown;

		#region Properties

		private bool _threeState;
		[DefaultValue(false)]
		public bool ThreeState
		{
			get { return _threeState; }
			set { _threeState = value; }
		}

		private bool _editEnabled = true;
		[DefaultValue(true)]
		public bool EditEnabled
		{
			get { return _editEnabled; }
			set { _editEnabled = value; }
		}

		#endregion

		public NodeCheckBox()
			: this(string.Empty)
		{
		}

		public NodeCheckBox(string propertyName)
		{
			_check = Resources.check;
			_uncheck = Resources.uncheck;
			_unknown = Resources.unknown;
			DataPropertyName = propertyName;
		}

		public override Size MeasureSize(TreeNodeAdv node, DrawContext context)
		{
			return new Size(ImageSize, ImageSize);
		}

		public override void Draw(TreeNodeAdv node, DrawContext context)
		{
			Rectangle bounds = GetBounds(node, context);
            CheckState state = GetCheckState(node);
            if (System.Windows.Forms.Application.RenderWithVisualStyles)
			{
                VisualStyleElement element = VisualStyleElement.Button.CheckBox.UncheckedNormal;
                if (state == CheckState.Indeterminate)
                    element = VisualStyleElement.Button.CheckBox.MixedNormal;
                else if (state == CheckState.Checked)
                    element = VisualStyleElement.Button.CheckBox.CheckedNormal;
                else if (state == CheckState.Disabled)
                    element = VisualStyleElement.Button.CheckBox.UncheckedDisabled;
                else
                    element = VisualStyleElement.Button.CheckBox.UncheckedNormal;
                VisualStyleRenderer renderer = new VisualStyleRenderer(element);
				renderer.DrawBackground(context.Graphics, new Rectangle(bounds.X, bounds.Y, ImageSize, ImageSize));
			}
			else
			{
				Image img;
                if (state == CheckState.Indeterminate)
					img = _unknown;
                else if (state == CheckState.Checked)
					img = _check;
				else
					img = _uncheck;
				context.Graphics.DrawImage(img, bounds.Location);
			}
		}

        protected virtual CheckState GetCheckState(TreeNodeAdv node)
		{
			object obj = GetValue(node);
            if (obj is CheckState)
                return (CheckState)obj;
			else if (obj is bool)
                return (bool)obj ? CheckState.Checked : CheckState.Unchecked;
			else
                return CheckState.Unchecked;
		}

        protected virtual void SetCheckState(TreeNodeAdv node, CheckState value)
		{
			Type type = GetPropertyType(node);
            if (type == typeof(CheckState))
			{
				SetValue(node, value);
				OnCheckStateChanged(node);
			}
			else if (type == typeof(bool))
			{
                SetValue(node, value != CheckState.Unchecked);
				OnCheckStateChanged(node);
			}
		}

		public override void MouseDown(TreeNodeAdvMouseEventArgs args)
		{
            if (args.Button == System.Windows.Forms.MouseButtons.Left && EditEnabled)
			{
				DrawContext context = new DrawContext();
				context.Bounds = args.ControlBounds;
				Rectangle rect = GetBounds(args.Node, context);
				if (rect.Contains(args.ViewLocation))
				{
                    CheckState state = GetCheckState(args.Node);
					state = GetNewState(state);
					SetCheckState(args.Node, state);
					args.Handled = true;
				}
			}
		}

		public override void MouseDoubleClick(TreeNodeAdvMouseEventArgs args)
		{
			args.Handled = true;
		}

        protected CheckState GetNewState(CheckState state)
		{
            if (state == CheckState.Disabled)
                return CheckState.Disabled;
            if (state == CheckState.Indeterminate)
                return CheckState.Unchecked;
            else if (state == CheckState.Unchecked)
                return CheckState.Checked;
			else
                return CheckState.Unchecked;
		}

        public override void KeyDown(System.Windows.Forms.KeyEventArgs args)
		{
            if (args.KeyCode == System.Windows.Forms.Keys.Space && EditEnabled)
			{
				if (Parent.SelectedNode != null)
				{
                    CheckState value = CheckState.Unchecked;
                    foreach (TreeNodeAdv node in Parent.SelectedNodes)
                    {
                        value = GetNewState(GetCheckState(node));
                        SetCheckState(node, value);
                    }
				}
				args.Handled = true;
			}
		}

		public event EventHandler<TreePathEventArgs> CheckStateChanged;
		protected void OnCheckStateChanged(TreePathEventArgs args)
		{
			if (CheckStateChanged != null)
				CheckStateChanged(this, args);
		}

		protected void OnCheckStateChanged(TreeNodeAdv node)
		{
            TreePath path = node.GetPath();
			OnCheckStateChanged(new TreePathEventArgs(path));
		}

	}
}