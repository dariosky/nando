using System;
using HS.Controls.Tree;
using HS.Controls.Tree.Base;
using NUnit.Framework;

namespace HS.Controls.Tree.Tests
{
	[TestFixture]
	public class NodeTest
	{
		[Test]
		public void ParentTest()
		{
			Node r1 = new Node("");
			Node node = new Node("");

			r1.Nodes.Add(node);
			Assert.AreEqual(1, r1.Nodes.Count);
			Assert.AreEqual(r1, node.Parent);

			r1.Nodes.Remove(node);
			Assert.AreEqual(0, r1.Nodes.Count);
			Assert.AreEqual(null, node.Parent);

			node.Parent = r1;
			Assert.AreEqual(1, r1.Nodes.Count);
			Assert.AreEqual(r1, node.Parent);

			node.Parent = null;
			Assert.AreEqual(0, r1.Nodes.Count);
			Assert.AreEqual(null, node.Parent);


			Node r2 = new Node("");
			node.Parent = r1;

			node.Parent = r2;
			Assert.AreEqual(1, r2.Nodes.Count);
			Assert.AreEqual(0, r1.Nodes.Count);
			Assert.AreEqual(r2, node.Parent);

			r1.Nodes.Add(node);
			Assert.AreEqual(1, r1.Nodes.Count);
			Assert.AreEqual(0, r2.Nodes.Count);
			Assert.AreEqual(r1, node.Parent);



			Node node2 = new Node("");
			r1.Nodes[0] = node2;
			Assert.AreEqual(null, node.Parent);
			Assert.AreEqual(r1, node2.Parent);
		}

	}
}
