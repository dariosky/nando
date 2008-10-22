using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using HS.Controls.Tree.Base;

namespace HS.Controls.Tree.NodeControls
{
	public abstract class EditableControl : BindableControl
	{
		#region Properties

		private TreeNodeAdv _editNode;
		protected TreeNodeAdv EditNode
		{
			get { return _editNode; }
		}

		private Control _editor;
		protected Control CurrentEditor
		{
			get { return _editor; }
		}

		private bool _editEnabled = false;
		[DefaultValue(false)]
		public bool EditEnabled
		{
			get { return _editEnabled; }
			set { _editEnabled = value; }
		}

		private bool _editOnClick = false;
		[DefaultValue(false)]
		public bool EditOnClick
		{
			get { return _editOnClick; }
			set { _editOnClick = value; }
		}
		
		#endregion

		protected EditableControl()
		{
		}

		public void SetEditorBounds(EditorContext context)
		{
			Size size = CalculateEditorSize(context);
			context.Editor.Bounds = new Rectangle(context.Bounds.X, context.Bounds.Y,
				Math.Min(size.Width, context.Bounds.Width), context.Bounds.Height);
		}

		protected abstract Size CalculateEditorSize(EditorContext context);

		protected virtual bool CanEdit(TreeNodeAdv node)
		{
			return (node.Tag != null);
		}

		public void BeginEdit()
		{
			if (EditEnabled && Parent.FocusedRow >= 0 && CanEdit(Parent.FocusedNode))
			{
				CancelEventArgs args = new CancelEventArgs();
				OnEditorShowing(args);
				if (!args.Cancel)
				{
					_editor = CreateEditor(Parent.FocusedNode);
					_editor.Validating += new CancelEventHandler(EditorValidating);
					_editor.KeyDown += new KeyEventHandler(EditorKeyDown);
					_editNode = Parent.FocusedNode;
					Parent.DisplayEditor(_editor, this);
				}
			}
		}

		private void EditorKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
				EndEdit(false);
			else if (e.KeyCode == Keys.Enter)
				EndEdit(true);
		}

		private void EditorValidating(object sender, CancelEventArgs e)
		{
			ApplyChanges();
		}

		internal void HideEditor(Control editor)
		{
			editor.Validating -= new CancelEventHandler(EditorValidating);
			editor.Parent = null;
			editor.Dispose();
			_editNode = null;
			OnEditorHided();
		}

		public void EndEdit(bool applyChanges)
		{
			if (!applyChanges)
				_editor.Validating -= new CancelEventHandler(EditorValidating);
			Parent.Focus();
		}

		public virtual void UpdateEditor(Control control)
		{
		}

		protected virtual void ApplyChanges()
		{
			try
			{
				DoApplyChanges(_editNode, _editor);
			}
			catch (ArgumentException ex)
			{
				MessageBox.Show(ex.Message, "Value is not valid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		protected abstract void DoApplyChanges(TreeNodeAdv node, Control editor);

		protected abstract Control CreateEditor(TreeNodeAdv node);

		public override void MouseUp(TreeNodeAdvMouseEventArgs args)
		{
			if (EditOnClick && args.Button == MouseButtons.Left && args.ModifierKeys == Keys.None)
			{
				Parent.ItemDragMode = false;
				BeginEdit();
				args.Handled = true;
			}
		}

		#region Events

		public event CancelEventHandler EditorShowing;
		protected void OnEditorShowing(CancelEventArgs args)
		{
			if (EditorShowing != null)
				EditorShowing(this, args);
		}

		public event EventHandler EditorHided;
		protected void OnEditorHided()
		{
			if (EditorHided != null)
				EditorHided(this, EventArgs.Empty);
		}

		#endregion
	}
}
