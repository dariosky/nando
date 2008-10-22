using System;
using System.Collections.Generic;
using HS.Controls.Tree.Base;
using NUnit.Framework;

namespace HS.Controls.Tree.Tests
{
    [TestFixture]
    public class RangeTest
    {
        [Test]
        public void TestInitialization()
        {
            Range r = new Range(1, 3);
            Assert.AreEqual(1, r.Start);
            Assert.AreEqual(3, r.End);
            r = new Range(3, 1);
            Assert.AreEqual(3, r.Start);
            Assert.AreEqual(3, r.End);
        }

        [Test]
        public void TestContains()
        {
            Range r = new Range(2, 10);
            Assert.IsTrue(r.Contains(2));
            Assert.IsTrue(r.Contains(5));
            Assert.IsTrue(r.Contains(10));
            Assert.IsFalse(r.Contains(11));
            Assert.IsFalse(r.Contains(1));
        }

        [Test]
        public void TestContainsRange()
        {
            Range r = new Range(2, 10);
            Assert.IsTrue(r.Contains(new Range(2, 2)));
            Assert.IsTrue(r.Contains(new Range(3, 3)));
            Assert.IsFalse(r.Contains(new Range(11, 11)));
            Assert.IsFalse(r.Contains(new Range(3, 11)));
        }

        [Test]
        public void TestTouches()
        {
            Range r = new Range(2, 10);
            Assert.IsTrue(r.Touches(5));
            Assert.IsTrue(r.Touches(1));
            Assert.IsTrue(r.Touches(11));
            //Just on the edges
            Assert.IsFalse(r.Touches(0));
            Assert.IsFalse(r.Touches(12));
        }

        [Test]
        public void TestTouchesRange()
        {
            Range r = new Range(2, 10);
            Assert.IsTrue(r.Touches(new Range(2, 2)));
            Assert.IsTrue(r.Touches(new Range(3, 3)));
            Assert.IsFalse(r.Touches(new Range(12, 12)));
            Assert.IsFalse(r.Touches(new Range(0, 0)));
            Assert.IsTrue(r.Touches(new Range(3, 11)));
            Assert.IsTrue(r.Touches(new Range(1, 11)));
            //Just on the edges
            Assert.IsTrue(r.Touches(new Range(11, 11)));
            Assert.IsTrue(r.Touches(new Range(1, 1)));
        }

        [Test]
        public void TestUnion()
        {
            Range r = new Range(2, 10);
            Range r2 = r.Union(5);
            Assert.AreEqual(2, r2.Start);
            Assert.AreEqual(10, r2.End);
            r2 = r.Union(11);
            Assert.AreEqual(2, r2.Start);
            Assert.AreEqual(11, r2.End);
            r2 = r.Union(1);
            Assert.AreEqual(1, r2.Start);
            Assert.AreEqual(10, r2.End);
            //We don't care if the ranges are not "unionable". It is the responsibility of the
            //caller to check if the ranges touch before calling Union()
            r2 = r.Union(12);
            Assert.AreEqual(2, r2.Start);
            Assert.AreEqual(12, r2.End);
        }

        [Test]
        public void TestUnionRange()
        {
            Range r = new Range(2, 10);
            Range r2 = r.Union(new Range(2, 10));
            Assert.AreEqual(2, r2.Start);
            Assert.AreEqual(10, r2.End);
            r2 = r.Union(new Range(3, 11));
            Assert.AreEqual(2, r2.Start);
            Assert.AreEqual(11, r2.End);
            r2 = r.Union(new Range(1, 9));
            Assert.AreEqual(1, r2.Start);
            Assert.AreEqual(10, r2.End);
            //We don't care if the ranges are not "unionable". It is the responsibility of the
            //caller to check if the ranges touch before calling Union()
            r2 = r.Union(new Range(12, 12));
            Assert.AreEqual(2, r2.Start);
            Assert.AreEqual(12, r2.End);
        }
    }

    [TestFixture]
    public class RangesTest
    {
        [Test]
        public void TestAddIndexesAndCheck()
        {
            Ranges ranges = new Ranges();
            ranges.Add(2);
            ranges.Add(5);
            ranges.Add(6);
            ranges.Add(8);
            Assert.IsTrue(ranges.Contains(2));
            Assert.IsTrue(ranges.Contains(5));
            Assert.IsTrue(ranges.Contains(6));
            Assert.IsTrue(ranges.Contains(8));
            Assert.IsFalse(ranges.Contains(7));
            Assert.IsFalse(ranges.Contains(12));
            Assert.IsFalse(ranges.Contains(0));
        }

        [Test]
        public void TestAddRangesAndCheck()
        {
            Ranges ranges = new Ranges();
            ranges.Add(new Range(2,5));
            ranges.Add(new Range(7,9));
            Assert.IsTrue(ranges.Contains(2));
            Assert.IsTrue(ranges.Contains(5));
            Assert.IsTrue(ranges.Contains(7));
            Assert.IsTrue(ranges.Contains(8));
            Assert.IsFalse(ranges.Contains(6));
            Assert.IsFalse(ranges.Contains(12));
            Assert.IsFalse(ranges.Contains(0));
        }

        [Test]
        public void RemoveIndex_Simple()
        {
            Ranges ranges = new Ranges();
            ranges.Add(2);
            Assert.IsTrue(ranges.Contains(2));
            ranges.Remove(2);
            Assert.IsFalse(ranges.Contains(2));
        }

        [Test]
        public void RemoveIndex_ReturnValue()
        {
            Ranges ranges = new Ranges();
            ranges.Add(2);
            Assert.IsTrue(ranges.Remove(2));
            Assert.IsFalse(ranges.Remove(2));
        }

        [Test]
        public void RemoveIndex_CutRange()
        {
            Ranges ranges = new Ranges();
            ranges.Add(new Range(1, 3));
            ranges.Add(new Range(5, 7));
            ranges.Add(new Range(9, 11));
            Assert.IsTrue(ranges.Remove(2));
            Assert.IsTrue(ranges.Contains(1));
            Assert.IsTrue(ranges.Contains(3));
            Assert.IsFalse(ranges.Contains(2));
            Assert.IsTrue(ranges.Remove(5));
            Assert.IsTrue(ranges.Contains(6));
            Assert.IsTrue(ranges.Contains(7));
            Assert.IsFalse(ranges.Contains(5));
            Assert.IsTrue(ranges.Remove(11));
            Assert.IsTrue(ranges.Contains(9));
            Assert.IsTrue(ranges.Contains(10));
            Assert.IsFalse(ranges.Contains(11));
        }

        [Test]
        public void AddIndexTwiceThenRemove()
        {
            Ranges ranges = new Ranges();
            ranges.Add(2);
            ranges.Add(2);
            ranges.Remove(2);
            Assert.IsFalse(ranges.Contains(2));
        }

        [Test]
        public void AddIndexThenRangeThenRemove()
        {
            Ranges ranges = new Ranges();
            ranges.Add(1);
            ranges.Add(2);
            ranges.Add(3);
            ranges.Add(new Range(1, 3));
            ranges.Remove(2);
            Assert.IsFalse(ranges.Contains(2));
        }

        [Test]
        public void AddRangesThenBiggerRangeThenRemove()
        {
            Ranges ranges = new Ranges();
            ranges.Add(3);
            ranges.Add(5);
            ranges.Add(new Range(1,8));
            ranges.Remove(5);
            Assert.IsFalse(ranges.Contains(5));
        }

        [Test]
        public void TestEnumerator()
        {
            Ranges ranges = new Ranges();
            ranges.Add(3);
            ranges.Add(5);
            ranges.Add(2);
            ranges.Add(6);
            ranges.Add(1);
            IEnumerator<int> e = ranges.GetEnumerator();
            e.MoveNext();
            Assert.AreEqual(1, e.Current);
            e.MoveNext();
            Assert.AreEqual(2, e.Current);
            e.MoveNext();
            Assert.AreEqual(3, e.Current);
            e.MoveNext();
            Assert.AreEqual(5, e.Current);
            e.MoveNext();
            Assert.AreEqual(6, e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void TestCollapseAllInASingleRange()
        {
            Ranges ranges = new Ranges();
            ranges.Add(new Range(3, 8));
            ranges.Add(new Range(10, 12));
            ranges.Add(0);
            ranges.Add(21);
            ranges.Collapse(new Range(4, 6));
            Assert.IsTrue(ranges.Contains(4));
            Assert.IsTrue(ranges.Contains(5));
            Assert.IsFalse(ranges.Contains(6));
            Assert.IsTrue(ranges.Contains(7));
            Assert.IsTrue(ranges.Contains(8));
            Assert.IsTrue(ranges.Contains(9));
            Assert.IsFalse(ranges.Contains(10));
            Assert.IsFalse(ranges.Contains(11));
            Assert.IsTrue(ranges.Contains(18));
            Assert.IsFalse(ranges.Contains(19));
            Assert.IsFalse(ranges.Contains(20));
            Assert.IsFalse(ranges.Contains(21));
            Assert.IsTrue(ranges.Contains(0));
        }

        [Test]
        public void TestCollapseOverlapse()
        {
            Ranges ranges = new Ranges();
            ranges.Add(new Range(3, 8));
            ranges.Add(new Range(10, 12));
            ranges.Add(0);
            ranges.Add(21);
            ranges.Collapse(new Range(7, 11));
            Assert.IsTrue(ranges.Contains(7));
            Assert.IsFalse(ranges.Contains(8));
        }

        [Test]
        public void TestCollapseOverlapse2()
        {
            Ranges ranges = new Ranges();
            ranges.Add(new Range(10, 12));
            ranges.Collapse(new Range(9, 11));
            Assert.IsFalse(ranges.Contains(7));
            Assert.IsFalse(ranges.Contains(8));
            Assert.IsTrue(ranges.Contains(9)); // 12 being brought back at 9
            Assert.IsFalse(ranges.Contains(10));
            Assert.IsFalse(ranges.Contains(11));
            Assert.IsFalse(ranges.Contains(12));
        }

        [Test]
        public void TestCollapseOverlapseEdges()
        {
            Ranges ranges = new Ranges();
            ranges.Add(new Range(3, 8));
            ranges.Add(new Range(10, 12));
            ranges.Add(0);
            ranges.Add(21);
            ranges.Collapse(new Range(8, 10));
            Assert.IsTrue(ranges.Contains(7));
            Assert.IsTrue(ranges.Contains(8));
            Assert.IsTrue(ranges.Contains(9));
            Assert.IsFalse(ranges.Contains(10));
        }

        [Test]
        public void TestCollapseContainsWholeRanges()
        {
            Ranges ranges = new Ranges();
            ranges.Add(new Range(3, 8));
            ranges.Add(new Range(10, 12));
            ranges.Add(0);
            ranges.Add(21);
            ranges.Collapse(new Range(3, 12));
            Assert.IsFalse(ranges.Contains(3));
            Assert.IsFalse(ranges.Contains(8));
            Assert.IsFalse(ranges.Contains(10));
            Assert.IsTrue(ranges.Contains(11)); // 21 - (12 - 3 + 1)
            Assert.IsFalse(ranges.Contains(12));
        }

        [Test]
        public void TestExpandUnselected()
        {
            Ranges ranges = new Ranges();
            ranges.Add(1);
            ranges.Add(4);
            ranges.Expand(new Range(2, 22));
            Assert.IsFalse(ranges.Contains(4));
            Assert.IsTrue(ranges.Contains(25));
        }

        [Test]
        public void TestExpandSelected()
        {
            Ranges ranges = new Ranges();
            ranges.Add(new Range(1, 4));
            ranges.Expand(new Range(2, 22));
            Assert.IsTrue(ranges.Contains(1));
            Assert.IsTrue(ranges.Contains(4));
            Assert.IsTrue(ranges.Contains(24));
            Assert.IsTrue(ranges.Contains(25));
            Assert.IsFalse(ranges.Contains(26));
        }

        [Test]
        public void TestExpandUnselectedBecauseItsAtTheEdge()
        {
            Ranges ranges = new Ranges();
            ranges.Add(3);
            ranges.Add(4);
            ranges.Expand(new Range(2, 22));
            Assert.IsFalse(ranges.Contains(2));
            Assert.IsFalse(ranges.Contains(3));
            Assert.IsFalse(ranges.Contains(4));
            Assert.IsFalse(ranges.Contains(23));
            Assert.IsTrue(ranges.Contains(24));
            Assert.IsTrue(ranges.Contains(25));
            Assert.IsFalse(ranges.Contains(26));
        }
    }
}
