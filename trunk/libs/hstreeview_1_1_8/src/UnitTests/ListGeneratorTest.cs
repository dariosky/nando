using System;
using System.Collections.Generic;
using HS.Controls.Tree.Base;
using NUnit.Framework;

namespace HS.Controls.Tree.Tests
{
    class IntEnumerator : IEnumerator<object>
    {
        private int _current;
        private int _count; 

        public IntEnumerator(int count)
        {
            _current = -1;
            _count = count;
        }

        public object Current
        {
            get { return _current; }
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (_current < _count - 1)
            {
                _current++;
                return true;
            }
            else
                return false;
        }

        public void Reset()
        {
            _current = -1;
        }
    }

    [TestFixture]
    public class ListGeneratorTest
    {
        private  void _Generator(int source, out int result)
        {
            result = source + 42;
        }

        private bool _enumerationCompleteCalled = false;
        private void List_EnumerationComplete(object sender, EventArgs e)
        {
            _enumerationCompleteCalled = true;
        }

        private List<int> _itemGeneratedCalls = new List<int>();
        private void List_ItemGenerated(object sender, ItemGeneratedArgs<int> e)
        {
            _itemGeneratedCalls.Add(e.Generated);
        }

        [Test]
        public void TestValues()
        {
            ListGenerator<int,int> lg = new ListGenerator<int,int>(new IntEnumerator(10), 10, _Generator);
            int expected = 42;
            foreach (int value in lg)
            {
                Assert.AreEqual(expected, value);
                expected++;
            }
            Assert.AreEqual(42 + 10, expected);
        }

        [Test]
        public void TestThatItGeneratesValuesOnTheFly()
        {
            IntEnumerator e = new IntEnumerator(10);
            ListGenerator<int, int> lg = new ListGenerator<int, int>(e, 10, _Generator);
            Assert.AreEqual(-1, e.Current);
            Assert.AreEqual(42, lg[0]);
            Assert.AreEqual(0, e.Current);
            Assert.AreEqual(45, lg[3]);
            Assert.AreEqual(3, e.Current);
            Assert.AreEqual(51, lg[9]);
            Assert.AreEqual(9, e.Current);
        }

        [Test]
        public void TestIndexBounds()
        {
            ListGenerator<int, int> lg = new ListGenerator<int, int>(new IntEnumerator(10), 10, _Generator);
            int foo;
            try
            {
                foo = lg[-1];
                Assert.Fail("Must throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException) {}
            try
            {
                foo = lg[10];
                Assert.Fail("Must throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException) { }
        }

        [Test]
        public void TestNegativeCount()
        {
            //If the count is smaller than 0, the list generator will simply be completely loaded right away.
            ListGenerator<int, int> lg = new ListGenerator<int, int>(new IntEnumerator(10), -1, _Generator);
            Assert.AreEqual(10, lg.Count);
            Assert.AreEqual(51, lg[9]);
        }

        [Test]
        public void TestIndexOf()
        {
            ListGenerator<int, int> lg = new ListGenerator<int, int>(new IntEnumerator(10), 10, _Generator);
            Assert.AreEqual(-1, lg.IndexOf(42)); //Only check index of fetched items.
            Assert.AreEqual(0, lg.IndexOf(lg[0]));
        }

        [Test]
        public void TestTriggerEventOnEnumerationComplete()
        {
            ListGenerator<int, int> lg = new ListGenerator<int, int>(new IntEnumerator(10), 10, _Generator);
            lg.EnumerationComplete += List_EnumerationComplete;
            Assert.AreEqual(51, lg[9]);
            Assert.IsTrue(_enumerationCompleteCalled);
            //But just once
            _enumerationCompleteCalled = false;
            Assert.AreEqual(51, lg[9]);
            Assert.IsFalse(_enumerationCompleteCalled);
        }

        [Test]
        public void TestTriggerEventOnItemGenerated()
        {
            ListGenerator<int, int> lg = new ListGenerator<int, int>(new IntEnumerator(10), 10, _Generator);
            lg.ItemGenerated += List_ItemGenerated;
            Assert.AreEqual(46, lg[4]);
            Assert.AreEqual(5, _itemGeneratedCalls.Count);
            Assert.AreEqual(42, _itemGeneratedCalls[0]);
            Assert.AreEqual(46, _itemGeneratedCalls[4]);
        }
    }
}
