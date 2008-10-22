using System;
using HS.Controls.Tree.Base;
using HS.Controls.Tree.NodeControls;
using NUnit.Framework;

namespace HS.Controls.Tree.Tests
{
    [TestFixture]
    public class BindableControlTest
    {
        class TestDataHolder
        {
            private string _data = "";
            public string Data
            {
                get { return _data; }
                set { _data = value; }
            }

            private CheckState _checked = CheckState.Indeterminate;
            public CheckState Checked
            {
                get { return _checked; }
                set { _checked = value; }
            }
        }

        [Test]
        public void TestStandardSetAndGet()
        {
            BindableControl ctrl = new NodeTextBox();
            ctrl.DataPropertyName = "Data";
            TestDataHolder dataHolder = new TestDataHolder();
            TreeNodeAdv node = new TreeNodeAdv(null, dataHolder);
            ctrl.SetValue(node, "foobar");
            Assert.AreEqual("foobar", dataHolder.Data);
            Assert.AreEqual("foobar", ctrl.GetValue(node));
        }

        [Test]
        public void TestNodeTagIsNull()
        {
            //We shouldn't get an exception thrown, just an aborted Set, and a null returning Get.
            BindableControl ctrl = new NodeTextBox();
            ctrl.DataPropertyName = "Data";
            TreeNodeAdv node = new TreeNodeAdv(null, null);
            ctrl.SetValue(node, "foobar");
            Assert.IsNull(ctrl.GetValue(node));
        }

        [Test]
        public void TestInvalidType()
        {
            //We should get an ArgumentException out of SetValue
            BindableControl ctrl = new NodeTextBox();
            ctrl.DataPropertyName = "Checked";
            TestDataHolder dataHolder = new TestDataHolder();
            TreeNodeAdv node = new TreeNodeAdv(null, dataHolder);
            try
            {
                ctrl.SetValue(node, "foobar");
                Assert.Fail("No ArgumentException thrown");
            }
            catch (ArgumentException) { }
            Assert.AreEqual(CheckState.Indeterminate, dataHolder.Checked);
            Assert.AreEqual(CheckState.Indeterminate, ctrl.GetValue(node));
        }
    }
}
