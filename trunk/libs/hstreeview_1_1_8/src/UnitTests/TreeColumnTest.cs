using System;
using HS.Controls.Tree;
using HS.Controls.Tree.Base;
using NUnit.Framework;

namespace HS.Controls.Tree.Tests
{
    [TestFixture]
    public class TreeColumnTest
    {
        [Test]
        public void TestUpdateBounds()
        {
            TreeColumn c1 = new TreeColumn("foo", 69);
            c1.UpdateBounds(null); // c1 is the first column
            Assert.AreEqual(0, c1.Bounds.Left);
            Assert.AreEqual(69, c1.Bounds.Right);
            TreeColumn c2 = new TreeColumn("bar", 42);
            c2.UpdateBounds(c1);
            Assert.AreEqual(69, c2.Bounds.Left);
            Assert.AreEqual(69 + 42, c2.Bounds.Right);
        }
    }
}
