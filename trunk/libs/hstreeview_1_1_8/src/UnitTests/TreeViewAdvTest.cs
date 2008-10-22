using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using HS.Controls.Tree;
using HS.Controls.Tree.Base;
using HS.Controls.Tree.NodeControls;
using NUnit.Framework;

namespace HS.Controls.Tree.Tests
{
    [TestFixture]
    public class TreeViewAdvTest
    {
        [Test]
        public void AllNodes()
        {
            TreeModel model = new TreeModel();
            List<TreeNodeAdv> result = new List<TreeNodeAdv>();
            TreeViewAdv tree = new TreeViewAdv();
            tree.Model = model;
            result.AddRange(tree.AllNodes);
            Assert.AreEqual(0, result.Count);
            model.Root.Nodes.Add(new Node("foo"));
            result.Clear();
            result.AddRange(tree.AllNodes);
            Assert.AreEqual(1, result.Count);
            Assert.AreSame(tree.Root.Nodes[0], result[0]);
            model.Root.Nodes[0].Nodes.Add(new Node("bar"));
            result.Clear();
            result.AddRange(tree.AllNodes);
            Assert.AreEqual(2, result.Count);
            Assert.AreSame(tree.Root.Nodes[0], result[0]);
            Assert.AreSame(tree.Root.Nodes[0].Nodes[0], result[1]);
        }

        [Test]
        public void GetControlsOfColumn()
        {
            TreeViewAdv tree = new TreeViewAdv();
            TreeColumn col1 = new TreeColumn();
            TreeColumn col2 = new TreeColumn();
            tree.Columns.Add(col1);
            tree.Columns.Add(col2);
            NodeControl nc1 = new NodeTextBox();
            NodeControl nc2 = new NodeTextBox();
            NodeControl nc3 = new NodeTextBox();
            nc1.ParentColumn = col1;
            nc2.ParentColumn = col2;
            nc3.ParentColumn = col2;
            tree.NodeControls.Add(nc1);
            tree.NodeControls.Add(nc2);
            tree.NodeControls.Add(nc3);
            List<NodeControl> result = new List<NodeControl>(tree.GetControlsOfColumn(col1));
            Assert.AreEqual(1, result.Count);
            Assert.AreSame(nc1, result[0]);
            result = new List<NodeControl>(tree.GetControlsOfColumn(col2));
            Assert.AreEqual(2, result.Count);
            Assert.AreSame(nc2, result[0]);
            Assert.AreSame(nc3, result[1]);
        }

        [Test]
        public void VisibleColumns()
        {
            TreeViewAdv tree = new TreeViewAdv();
            TreeColumn col1 = new TreeColumn();
            TreeColumn col2 = new TreeColumn();
            TreeColumn col3 = new TreeColumn();
            tree.Columns.Add(col1);
            tree.Columns.Add(col2);
            tree.Columns.Add(col3);
            col2.Visible = false;
            List<TreeColumn> result = new List<TreeColumn>(tree.VisibleColumns);
            Assert.AreEqual(2, result.Count);
            Assert.AreSame(col1, result[0]);
            Assert.AreSame(col3, result[1]);
        }
    }

    [TestFixture]
    public class TreeViewAdv_GetNodeControlsTest
    {
        TreeViewAdv tree;
        TreeColumn col1;
        TreeColumn col2;
        NodeControl nc1;
        NodeControl nc2;
        NodeControl nc3;
        TreeModel model;
        TreeNodeAdv node;

        [SetUp]
        public void SetUp()
        {
            tree = new TreeViewAdv();
            col1 = new TreeColumn();
            col2 = new TreeColumn();
            tree.Columns.Add(col1);
            tree.Columns.Add(col2);
            col1.Width = 300;
            col2.Width = 200;
            nc1 = new NodeTextBox();
            nc2 = new NodeTextBox();
            nc3 = new NodeTextBox();
            nc1.ParentColumn = col1;
            nc2.ParentColumn = col2;
            nc3.ParentColumn = col2;
            tree.NodeControls.Add(nc1);
            tree.NodeControls.Add(nc2);
            tree.NodeControls.Add(nc3);

            model = new TreeModel();
            model.Nodes.Add(new Node(""));
            tree.Model = model;
            node = tree.Root.Nodes[0];
        }

        [Test]
        public void TestControlsWidthWithUseColumns()
        {
            tree.ShowPlusMinus = false;
            List<NodeControlInfo> result = new List<NodeControlInfo>(tree.GetNodeControls(node));
            Assert.AreSame(nc1, result[0].Control);
            Assert.AreSame(nc2, result[1].Control);
            Assert.AreSame(nc3, result[2].Control);
            Assert.AreEqual(col1.Width - 7 /* LeftMargin */, result[0].Bounds.Width);
            Assert.AreEqual(10, result[1].Bounds.Width); //BaseTextControl width is 10 when there is no text.
            Assert.AreEqual(189, result[2].Bounds.Width); //There is a 1 pixel padding between NCs.
        }

        [Test]
        public void TestWithAColumnWithoutNC()
        {
            tree.ShowPlusMinus = false;
            tree.NodeControls.Remove(nc1);
            List<NodeControlInfo> result = new List<NodeControlInfo>(tree.GetNodeControls(node));
            Assert.AreSame(nc2, result[0].Control);
            Assert.AreSame(nc3, result[1].Control);
            Assert.AreEqual(10, result[0].Bounds.Width);
            Assert.AreEqual(189, result[1].Bounds.Width);
        }

        [Test]
        public void TestWithControlsOverflow()
        {
            tree.ShowPlusMinus = false;
            col2.Width = 8; // 2 pixels smaller than nc2
            List<NodeControlInfo> result = new List<NodeControlInfo>(tree.GetNodeControls(node));
            Assert.AreEqual(8, result[1].Bounds.Width);
            Assert.AreEqual(0, result[2].Bounds.Width);
        }
    }

    [TestFixture]
    public class TreeViewAdv_AutoSizeColumns
    {
        TreeViewAdv tree;
        TreeColumn col;
        NodeCheckBox nc1;
        NodeTextBox nc2;
        TreeModel model;

        [SetUp]
        public void SetUp()
        {
            tree = new TreeViewAdv();
            tree.ShowPlusMinus = false;
            col = new TreeColumn();
            tree.Columns.Add(col);
            col.Width = 300;
            nc1 = new NodeCheckBox(); // size is 13x13
            nc2 = new NodeTextBox();
            nc1.ParentColumn = col;
            nc2.ParentColumn = col;
            tree.NodeControls.Add(nc1);
            tree.NodeControls.Add(nc2);

            model = new TreeModel();
            model.Nodes.Add(new Node(""));
            tree.Model = model;
        }

        [Test]
        public void WithoutDataVariableNCs()
        {
            tree.AutoSizeColumn(col);
            int expected = TreeViewAdv.LeftMargin + 13 + 10 + (TreeViewAdv.NodeControlsPadding * 2);
            Assert.AreEqual(expected, col.Width); 
        }

        [Test]
        public void WithoutAnyNode()
        {
            //The auto-resize operation should be aborted if there isn't any node.
            model.Nodes.RemoveAt(0);
            tree.AutoSizeColumn(col);
            Assert.AreEqual(300, col.Width); 
        }

        [Test]
        public void MinimumSize()
        {
            //The absolute minimum size is 10. If the column has a name, the minimum size is the width of the name.
            tree.NodeControls.RemoveAt(0);
            tree.NodeControls.RemoveAt(0);
            tree.AutoSizeColumn(col);
            Assert.AreEqual(10, col.Width);

            col.Header = "column name";
            tree.AutoSizeColumn(col);
            Graphics gr = Graphics.FromImage(new Bitmap(1, 1));
            SizeF s = gr.MeasureString(col.Header, tree.Font);
            Assert.AreEqual((int)s.Width + 8 /* Header padding */, col.Width);
            tree.ChangeSort(col);
            tree.AutoSizeColumn(col);
            Assert.AreEqual((int)s.Width + 8 /* Header padding */ + 24 /* Sort arrow */, col.Width);
        }

        [Test]
        public void WithDataVariableNCs()
        {
            //This time, we have 2 nodes with a different name, and nc2 is connected to "name".
            model.Nodes.Add(new Node("this string is longer"));
            nc2.DataPropertyName = "Text";

            tree.AutoSizeColumn(col);
            Graphics gr = Graphics.FromImage(new Bitmap(1, 1));
            SizeF s = gr.MeasureString("this string is longer", tree.Font);
            int expected = TreeViewAdv.LeftMargin + 13 + (int)s.Width + (TreeViewAdv.NodeControlsPadding * 2);
            Assert.AreEqual(expected, col.Width);
        }

        [Test]
        public void DontConsiderInvisibleNodes()
        {
            model.Nodes[0].Text = "foobar";
            model.Nodes[0].Nodes.Add(new Node("this string is longer"));
            nc2.DataPropertyName = "Text";

            tree.AutoSizeColumn(col);
            Graphics gr = Graphics.FromImage(new Bitmap(1, 1));
            SizeF s = gr.MeasureString("foobar", tree.Font);
            int expected = TreeViewAdv.LeftMargin + 13 + (int)s.Width + (TreeViewAdv.NodeControlsPadding * 2);
            Assert.AreEqual(expected, col.Width);
        }

        [Test]
        public void WithPlusMinus()
        {
            tree.ShowPlusMinus = true;
            tree.AutoSizeColumn(col);
            int expected = TreeViewAdv.LeftMargin + 16 + 13 + 10 + (TreeViewAdv.NodeControlsPadding * 3);
            Assert.AreEqual(expected, col.Width); 
            // But only on first column!
            TreeColumn col2 = new TreeColumn();
            tree.Columns.Add(col2);
            tree.AutoSizeColumn(col2);
            Assert.AreEqual(10, col2.Width); 
        }

        [Test]
        public void DontAddLeftMarginOnColumnsotherThanZero()
        {
            TreeColumn col2 = new TreeColumn();
            tree.Columns.Add(col2);
            nc1.ParentColumn = col2;
            tree.AutoSizeColumn(col2);
            Assert.AreEqual(13 + TreeViewAdv.NodeControlsPadding, col2.Width); 
        }

        [Test]
        public void ConsiderNodeLevel()
        {
            model.Nodes[0].Nodes.Add(new Node("this string is longer"));
            model.Nodes[0].Nodes[0].Nodes.Add(new Node("this string is the longest"));
            nc2.DataPropertyName = "Text";
            tree.NodeControls.Remove(nc1);
            tree.ExpandAll();
            tree.AutoSizeColumn(col);
            Graphics gr = Graphics.FromImage(new Bitmap(1, 1));
            SizeF s = gr.MeasureString("this string is the longest", tree.Font);
            int expected = TreeViewAdv.LeftMargin + (tree.Indent * 2) + (int)s.Width + TreeViewAdv.NodeControlsPadding;
            Assert.AreEqual(expected, col.Width);
            // But only in the first column!
            TreeColumn col2 = new TreeColumn();
            tree.Columns.Add(col2);
            nc2.ParentColumn = col2;
            tree.AutoSizeColumn(col2);
            Assert.AreEqual((int)s.Width + TreeViewAdv.NodeControlsPadding, col2.Width); 
        }

        [Test]
        public void AutoSizeAllColumns()
        {
            TreeColumn col2 = new TreeColumn();
            tree.Columns.Add(col2);
            tree.AutoSizeAllColumns();
            int expected = TreeViewAdv.LeftMargin + 13 + 10 + (TreeViewAdv.NodeControlsPadding * 2);
            Assert.AreEqual(expected, col.Width);
            Assert.AreEqual(10, col2.Width);
        }

        [Test]
        public void AutoSizeAllColumns_OnlyVisibleColumns()
        {
            col.Visible = false;
            tree.AutoSizeAllColumns();
            Assert.AreEqual(300, col.Width);
        }
    }

    [TestFixture]
    public class TreeViewAdv_Selection
    {
        TreeViewAdv tree;
        TreeModel model;
        private bool _selectionEventCalled;
        private void Tree_SelectionChanged(object sender, EventArgs e)
        {
            _selectionEventCalled = true;
        }

        [SetUp]
        public void SetUp()
        {
            tree = new TreeViewAdv();
            model = new TreeModel();
            model.Nodes.Add(new Node("1"));
            model.Nodes.Add(new Node("2"));
            model.Nodes.Add(new Node("3"));
            tree.Model = model;
            tree.SelectionChanged += Tree_SelectionChanged;
            _selectionEventCalled = false;
        }

        [Test]
        public void TestThatEventAreTriggered()
        {
            tree.Select(0);
            Assert.IsTrue(_selectionEventCalled);
            _selectionEventCalled = false;
            tree.Deselect(0);
            Assert.IsTrue(_selectionEventCalled);
            _selectionEventCalled = false;
            tree.ClearSelection();
            Assert.IsTrue(_selectionEventCalled);
        }

        [Test]
        public void TestThatEventAreTriggeredAtTheEndOfAnUpdate()
        {
            tree.BeginUpdate();
            tree.Select(0);
            Assert.IsFalse(_selectionEventCalled);
            tree.EndUpdate();
            Assert.IsTrue(_selectionEventCalled);
        }

        [Test]
        public void TestSelectedRows()
        {
            tree.Select(0);
            tree.Select(2);
            IEnumerator<int> e = tree.SelectedRows.GetEnumerator();
            e.MoveNext();
            Assert.AreEqual(0, e.Current);
            e.MoveNext();
            Assert.AreEqual(2, e.Current);
        }

        [Test]
        public void TestSelectedNodes()
        {
            tree.Select(0);
            tree.Select(2);
            IEnumerator<TreeNodeAdv> e = tree.SelectedNodes.GetEnumerator();
            e.MoveNext();
            Assert.AreEqual(tree.Root.Nodes[0], e.Current);
            e.MoveNext();
            Assert.AreEqual(tree.Root.Nodes[2], e.Current);
        }

        [Test]
        public void TestSelectRangeFromAnchorToRow()
        {
            tree.Select(2);
            tree.SelectRangeFromAnchorToRow(0);
            Assert.IsTrue(tree.IsSelected(1));
        }

        [Test]
        public void TestSelectedNodesWhenSelectionIsOutOfBounds()
        {
            tree.Select(0);
            tree.Select(3);
            //We should only have one selected node.
            IEnumerator<TreeNodeAdv> e = tree.SelectedNodes.GetEnumerator();
            e.MoveNext();
            Assert.AreEqual(tree.Root.Nodes[0], e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void TestFocusedRowAfterSelect()
        {
            tree.Select(2);
            Assert.AreEqual(2, tree.FocusedRow);
        }

        [Test]
        public void TestFocusedRowAfterDeselect()
        {
            tree.Select(2);
            tree.Select(1);
            tree.Deselect(2);
            Assert.AreEqual(2, tree.FocusedRow);
        }

        [Test]
        public void TestFocusedRowAfterSelectRangeFromAnchorToRow()
        {
            tree.Select(2);
            tree.SelectRangeFromAnchorToRow(0);
            Assert.AreEqual(0, tree.FocusedRow);
        }

        [Test]
        public void TestSelectionAnchorIsNotUpdated()
        {
            tree.Select(1);
            tree.SelectRangeFromAnchorToRow(0);
            tree.ClearSelection();
            tree.SelectRangeFromAnchorToRow(2);
            Assert.IsFalse(tree.IsSelected(0));
        }
    }
}
