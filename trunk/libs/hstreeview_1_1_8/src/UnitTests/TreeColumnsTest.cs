using System;
using System.Windows.Forms;
using System.Drawing;
using HS.Controls.Tree;
using HS.Controls.Tree.Base;
using NUnit.Framework;

namespace HS.Controls.Tree.Tests
{
    [TestFixture]
    public class TreeColumnsTest
    {
        [Test]
        public void TestBoundsAfterAdd()
        {
            TreeColumns columns = new TreeColumns();
            TreeColumn c1 = new TreeColumn("foo", 69);
            TreeColumn c2 = new TreeColumn("bar", 42);
            columns.Add(c1);
            columns.Add(c2);
            Assert.AreEqual(0, c1.Bounds.Left);
            Assert.AreEqual(69, c1.Bounds.Right);
            Assert.AreEqual(69, c2.Bounds.Left);
            Assert.AreEqual(69 + 42, c2.Bounds.Right);
        }

        [Test]
        public void TestBoundsAfterRemove()
        {
            TreeColumns columns = new TreeColumns();
            TreeColumn c1 = new TreeColumn("foo", 69);
            TreeColumn c2 = new TreeColumn("bar", 42);
            columns.Add(c1);
            columns.Add(c2);
            columns.RemoveAt(0);
            Assert.AreEqual(0, c2.Bounds.Left);
            Assert.AreEqual(42, c2.Bounds.Right);
        }

        [Test]
        public void TestBoundsAfterSet()
        {
            TreeColumns columns = new TreeColumns();
            TreeColumn c1 = new TreeColumn("foo", 69);
            TreeColumn c2 = new TreeColumn("bar", 42);
            TreeColumn c3 = new TreeColumn("baz", 1984);
            columns.Add(c1);
            columns.Add(c2);
            columns[0] = c3;
            Assert.AreEqual(1984, c2.Bounds.Left);
            Assert.AreEqual(1984 + 42, c2.Bounds.Right);
        }

        [Test]
        public void TestThatBoundsIgnoreInvisibleColumns()
        {
            TreeColumns columns = new TreeColumns();
            TreeColumn c1 = new TreeColumn("foo", 69);
            TreeColumn c2 = new TreeColumn("bar", 42);
            c2.Visible = false;
            TreeColumn c3 = new TreeColumn("baz", 1984);
            columns.Add(c1);
            columns.Add(c2);
            columns.Add(c3);
            Assert.AreEqual(69, c3.Bounds.Left);
            Assert.AreEqual(69 + 1984, c3.Bounds.Right);
        }

        [Test]
        public void TestThatVisibilityToggledEventUpdatesBounds()
        {
            TreeColumns columns = new TreeColumns();
            TreeColumn c1 = new TreeColumn("foo", 69);
            TreeColumn c2 = new TreeColumn("bar", 42);
            TreeColumn c3 = new TreeColumn("baz", 1984);
            columns.Add(c1);
            columns.Add(c2);
            columns.Add(c3);
            c2.Visible = false;
            Assert.AreEqual(69, c3.Bounds.Left);
            Assert.AreEqual(69 + 1984, c3.Bounds.Right);
        }

        [Test]
        public void TestThatWidthChangedEventUpdatesBounds()
        {
            TreeColumns columns = new TreeColumns();
            TreeColumn c1 = new TreeColumn("foo", 69);
            TreeColumn c2 = new TreeColumn("bar", 42);
            TreeColumn c3 = new TreeColumn("baz", 1984);
            columns.Add(c1);
            columns.Add(c2);
            columns.Add(c3);
            c2.Width = 911;
            Assert.AreEqual(69 + 911, c3.Bounds.Left);
            Assert.AreEqual(69 + 911 + 1984, c3.Bounds.Right);
        }

        [Test]
        public void TestHeaderWidth()
        {
            TreeColumns columns = new TreeColumns();
            TreeColumn c1 = new TreeColumn("foo", 69);
            TreeColumn c2 = new TreeColumn("bar", 42);
            TreeColumn c3 = new TreeColumn("baz", 1984);
            columns.Add(c1);
            Assert.AreEqual(69, columns.Width);
            columns.Add(c2);
            Assert.AreEqual(69 + 42, columns.Width);
            columns.Add(c3);
            Assert.AreEqual(69 + 42 + 1984, columns.Width);
        }

        [Test]
        public void TestHeaderHeight()
        {
            Application.EnableVisualStyles();
            Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.ClientAndNonClientAreasEnabled;
            TreeColumns columns = new TreeColumns();
            Assert.AreEqual(0, columns.Height);
            TreeColumn c1 = new TreeColumn("foo", 69);
            columns.Add(c1);
            Assert.AreEqual(20, columns.Height);
            Application.VisualStyleState = System.Windows.Forms.VisualStyles.VisualStyleState.NoneEnabled;
            //Let's be fair here: The treeview shouldn't have to handle the case where the visual style state is
            //changed in the middle of the run. Therefore, we will force a refresh here.
            columns.Remove(c1);
            columns.Add(c1);
            Assert.AreEqual(17, columns.Height);
        }

        [Test]
        public void TestColumnAt()
        {
            TreeColumns columns = new TreeColumns();
            TreeColumn c1 = new TreeColumn("foo", 69);
            TreeColumn c2 = new TreeColumn("bar", 42);
            TreeColumn c3 = new TreeColumn("baz", 1984);
            columns.Add(c1);
            columns.Add(c2);
            columns.Add(c3);
            Point p;
            p = new Point(0, 0);
            Assert.AreSame(c1, columns.GetColumnAt(p));
            p = new Point(68, TreeColumn.Height - 1);
            Assert.AreSame(c1, columns.GetColumnAt(p));
            p = new Point(69, TreeColumn.Height - 1);
            Assert.AreSame(c2, columns.GetColumnAt(p));
            p = new Point(-1, 0);
            Assert.IsNull(columns.GetColumnAt(p));
            p = new Point(0, -1);
            Assert.IsNull(columns.GetColumnAt(p));
            p = new Point(69 + 42 + 1984, 0);
            Assert.IsNull(columns.GetColumnAt(p));
            p = new Point(0, TreeColumn.Height);
            Assert.IsNull(columns.GetColumnAt(p));
        }
    }
}