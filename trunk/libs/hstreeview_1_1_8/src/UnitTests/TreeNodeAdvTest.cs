using System;
using System.Collections.Generic;
using System.Collections;
using HS.Controls.Tree;
using HS.Controls.Tree.Base;
using NUnit.Framework;

namespace HS.Controls.Tree.Tests
{
    [TestFixture]
    public class TreeNodeAdvTest
    {
        TreeModel model;
        Node foo,bar,baz,bleh;
        TreeViewRoot root;

        [SetUp]
        public void setUp()
        {
            model = new TreeModel();
            foo = new Node("foo");
            bar = new Node("bar");
            baz = new Node("baz");
            bleh = new Node("bleh");
            model.Root.Nodes.Add(foo);
            foo.Nodes.Add(bar);
            bar.Nodes.Add(baz);
            model.Root.Nodes.Add(bleh);
            root = new TreeViewRoot(model);
        }

        [Test]
        public void AllNodes()
        {
            List<TreeNodeAdv> result = new List<TreeNodeAdv>();
            result.AddRange(root.AllNodes);
            Assert.AreEqual(4, result.Count);
            Assert.AreSame(root.Nodes[0], result[0]);
            Assert.AreSame(root.Nodes[0].Nodes[0], result[1]);
            Assert.AreSame(root.Nodes[0].Nodes[0].Nodes[0], result[2]);
            Assert.AreSame(root.Nodes[1], result[3]);
        }

        [Test]
        public void VisibleNodes()
        {
            //baz will not be visible because we will not expand bar.
            TreeViewRoot root = new TreeViewRoot(model);
            TreeNodeAdv node = root.Nodes[0]; //foo
            node.IsExpanded = true; // Expanding foo;
            IEnumerator<TreeNodeAdv> e = root.VisibleNodes.GetEnumerator();
            e.MoveNext();
            Assert.AreSame(foo, e.Current.Tag);
            e.MoveNext();
            Assert.AreSame(bar, e.Current.Tag);
            e.MoveNext();
            Assert.AreSame(bleh, e.Current.Tag);
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void FindNode()
        {
            Assert.AreSame(root, root.FindNode(TreePath.Empty));
            Assert.AreSame(root.Nodes[0], root.FindNode(new TreePath(foo)));
            Assert.AreSame(root.Nodes[0].Nodes[0], root.FindNode(new TreePath(new object[] { foo, bar })));
            Assert.IsNull(root.FindNode(new TreePath(new object[] { bar, foo })));
        }

        [Test]
        public void FindNode_fetchedOnly()
        {
            Assert.AreSame(null, root.FindNode(new TreePath(foo),true));
            Assert.AreEqual(0, root.Nodes.Fetched.Count);
            Assert.AreSame(root.Nodes[0], root.FindNode(new TreePath(foo)));
            Assert.AreSame(root.Nodes[0], root.FindNode(new TreePath(foo),true));
            Assert.AreEqual(1, root.Nodes.Fetched.Count);
        }

        class IntModel : BaseTreeModel
        {
            public int current = -1;

            public override IEnumerable GetChildren(TreePath treePath)
            {
                current = -1;
                for (int i = 0; i < 10; i++)
                {
                    current = i;
                    yield return i;
                }
            }

            public override int GetChildrenCount(TreePath treePath)
            {
                return 10;
            }
        }

        [Test]
        public void TestLoadNodesOnTheFly()
        {
            IntModel model = new IntModel();
            TreeViewRoot root = new TreeViewRoot(model);
            Assert.AreEqual(10, root.Nodes.Count);
            Assert.AreEqual(-1, model.current);
        }
    }
}
