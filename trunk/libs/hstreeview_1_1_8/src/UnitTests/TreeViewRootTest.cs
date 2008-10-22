using System;
using HS.Controls.Tree;
using HS.Controls.Tree.Base;
using NUnit.Framework;

namespace HS.Controls.Tree.Tests
{
    [TestFixture]
    public class TreeViewRootTest
    {
        private bool _calledOnDataChanged;
        private bool _calledOnStructureChanged;

        TreeModel model;
        TreeViewRoot root;

        private void root_DataChanged(object sender, EventArgs e)
        {
            _calledOnDataChanged = true;
        }

        private void root_StructureChanged(object sender, EventArgs e)
        {
            _calledOnStructureChanged = true;
        }

        private void AddSomeNodes()
        {
            //Add 3 root nodes, and then 2 sub nodes to the 3rd node.
            Node node = new Node();
            model.Nodes.Add(node);
            node = new Node();
            model.Nodes.Add(node);
            node = new Node();
            model.Nodes.Add(node);
            node.Nodes.Add(new Node());
            node.Nodes.Add(new Node());
        }

        [SetUp]
        public void SetUp()
        {
            _calledOnDataChanged = false;
            _calledOnStructureChanged = false;
            model = new TreeModel();
            root = new TreeViewRoot(model);
            root.StructureChanged += root_StructureChanged;
            root.DataChanged += root_DataChanged;
        }

        [Test]
        public void CallOnStructureChangedOnModelStructureChanged()
        {
            model.OnStructureChanged(new TreePathEventArgs());
            Assert.IsTrue(_calledOnStructureChanged);
        }

        [Test]
        public void CallOnStructureChangedOnModelInsertAndRemoveNode()
        {
            model.Nodes.Add(new Node());
            Assert.IsTrue(_calledOnStructureChanged);
            _calledOnStructureChanged = false;
            model.Nodes.RemoveAt(0);
            Assert.IsTrue(_calledOnStructureChanged);
        }

        [Test]
        public void CallOnDataChangedOnModelNodeChanged()
        {
            Node node = new Node();
            model.Nodes.Add(node);
            node.Text = "foobar";
            Assert.IsTrue(_calledOnDataChanged);
        }

        [Test]
        public void InvalidateNodesOnStructureChanged()
        {
            Node node = new Node();
            model.Nodes.Add(node);
            Assert.AreEqual(1, root.Nodes.Count); // IsExpandedOnce is now true.
            model.OnStructureChanged(new TreePathEventArgs());
            Assert.IsFalse(root.IsExpandedOnce);
            Assert.AreEqual(0, root.Nodes[0].Nodes.Count); // IsExpandedOnce is now true.
            model.OnStructureChanged(new TreePathEventArgs(new TreePath(node)));
            Assert.IsTrue(root.IsExpandedOnce); // We only want the subnode to be invalidated.
            Assert.IsFalse(root.Nodes[0].IsExpandedOnce);
        }

        [Test]
        public void IgnoreInvalidPathInOnStructureChanged()
        {
            Assert.AreEqual(0, root.Nodes.Count);
            model.OnStructureChanged(new TreePathEventArgs(new TreePath(new object())));
            Assert.IsTrue(root.IsExpandedOnce);
            Assert.IsFalse(_calledOnStructureChanged);
        }

        [Test]
        public void AddNodesOnModelInsertNodesWithoutIndices()
        {
            Assert.AreEqual(0, root.Nodes.Count);
            Node node = new Node();
            model.Nodes.Add(node);
            Assert.AreEqual(1, root.Nodes.Count);
            Assert.AreSame(node, root.Nodes[0].Tag);
            Assert.AreEqual(0, root.Nodes[0].Nodes.Count);
            Node subnode = new Node();
            node.Nodes.Add(subnode);
            Assert.AreEqual(1, root.Nodes[0].Nodes.Count);
            Assert.AreSame(subnode, root.Nodes[0].Nodes[0].Tag);
        }

        [Test]
        public void InsertNodesOnModelInsertNodesWithIndices()
        {
            Assert.AreEqual(0, root.Nodes.Count);
            Node node1 = new Node();
            Node node2 = new Node();
            model.Nodes.Add(node1);
            model.Nodes.Insert(0, node2);
            Assert.AreEqual(2, root.Nodes.Count);
            Assert.AreSame(node2, root.Nodes[0].Tag);
            Assert.AreSame(node1, root.Nodes[1].Tag);
        }

        [Test]
        public void DontInsertTheSameNodeTwiceIfATreeNodeIsNotInitialized()
        {
            //What used to happen here is that an InsertNode would be applied no an uninitialized node, and it
            //resulted in the node being added twice (Once when Nodes was initialized, and the a second time when
            //the inserted node was added manually with AddNode().
            Node node = new Node();
            model.Nodes.Add(node);
            Assert.AreEqual(1, root.Nodes.Count);
        }

        [Test]
        public void RemoveNodesOnModelRemoveNodes()
        {
            Node node = new Node();
            model.Nodes.Add(node);
            Assert.AreEqual(1, root.Nodes.Count);
            model.Nodes.Remove(node);
            Assert.AreEqual(0, root.Nodes.Count);
            model.Nodes.Add(node);
            node.Nodes.Add(new Node());
            Assert.AreEqual(1, root.Nodes[0].Nodes.Count);
            node.Nodes.RemoveAt(0);
            Assert.AreEqual(0, root.Nodes[0].Nodes.Count);
        }

        [Test]
        public void RememberExpandedNodes()
        {
            model.Nodes.Add(new Node());
            model.Nodes.Add(new Node());
            root.Nodes[0].IsExpanded = true;
            root.InvalidateNodes();
            Assert.IsTrue(root.Nodes[0].IsExpanded);
            Assert.IsFalse(root.Nodes[1].IsExpanded);
            root.Nodes[0].IsExpanded = false;
            root.InvalidateNodes();
            Assert.IsFalse(root.Nodes[0].IsExpanded);
        }

        [Test]
        public void RowCount()
        {
            AddSomeNodes();
            Assert.AreEqual(3, root.RowCount);
            root.Nodes[2].IsExpanded = true;
            Assert.AreEqual(5, root.RowCount);
            root.Nodes[2].IsExpanded = false;
            Assert.AreEqual(3, root.RowCount);
            root.Nodes[2].IsExpanded = true;
            Assert.AreEqual(5, root.RowCount);
            root.IsExpanded = true; //It shouldn't mess things up.
            Assert.AreEqual(5, root.RowCount);
        }

        [Test]
        public void RowCount_reset_on_invalidate()
        {
            AddSomeNodes();
            root.Nodes[2].IsExpanded = true;
            Assert.AreEqual(5, root.RowCount);
            root.InvalidateNodes();
            Assert.AreEqual(3, root.RowCount);
        }

        [Test]
        public void ExpandAll()
        {
            AddSomeNodes();
            root.ExpandAll();
            foreach (TreeNodeAdv node in root.AllNodes)
                Assert.IsTrue(node.IsExpanded);
        }

        [Test]
        public void ExpandAll_dont_fetch()
        {
            AddSomeNodes();
            root.ExpandAll();
            Assert.AreEqual(0, root.Nodes.Fetched.Count);
        }

        [Test]
        public void ExpandAll_can_collapse_after()
        {
            AddSomeNodes();
            root.ExpandAll();
            root.Nodes[2].IsExpanded = false;
            Assert.AreEqual(3, root.RowCount);
        }

        [Test]
        public void ExpandAll_expand_nodes_that_are_already_fetched()
        {
            AddSomeNodes();
            root.Nodes[0].IsExpanded = false;
            root.ExpandAll();
            Assert.IsTrue(root.Nodes[0].IsExpanded);
        }

        [Test]
        public void ExpandAll_clear_expand_flags()
        {
            AddSomeNodes();
            root.Nodes[0].IsExpanded = true;
            root.ExpandAll();
            root.InvalidateNodes();
            Assert.IsTrue(root.Nodes[0].IsExpanded);
        }

        [Test]
        public void CollapseAll()
        {
            AddSomeNodes();
            root.ExpandAll();
            Assert.IsTrue(root.Nodes[0].IsExpanded);
            root.CollapseAll();
            Assert.IsFalse(root.Nodes[0].IsExpanded);
            root.InvalidateNodes();
            Assert.IsFalse(root.Nodes[0].IsExpanded);
        }
    }
}
