using System;
using HS.Controls.Tree.Base;
using HS.Controls.Tree.NodeControls;
using NUnit.Framework;

namespace HS.Controls.Tree.Tests
{
    [TestFixture]
    public class NodeCheckBoxTest
    {
        class TestNodeCheckBox : NodeCheckBox
        {
            internal CheckState Checked = CheckState.Unchecked;

            public void SimulateClickOrSpaceKey()
            {
                Checked = GetNewState(Checked);
            }
        }
        [Test]
        public void GetNewState()
        {
            /*
             * GetNewState should return Unchecked when it's Checked, Checked when it's Unchecked.
             * When it's Indeterminate, it should return Unchecked.
             * When it's Disabled, it should return Disabled.
            */
            TestNodeCheckBox cb = new TestNodeCheckBox();
            cb.Checked = CheckState.Unchecked;
            cb.SimulateClickOrSpaceKey();
            Assert.AreEqual(CheckState.Checked, cb.Checked);
            cb.SimulateClickOrSpaceKey();
            Assert.AreEqual(CheckState.Unchecked, cb.Checked);
            cb.Checked = CheckState.Disabled;
            cb.SimulateClickOrSpaceKey();
            Assert.AreEqual(CheckState.Disabled, cb.Checked);
            cb.Checked = CheckState.Indeterminate;
            cb.SimulateClickOrSpaceKey();
            Assert.AreEqual(CheckState.Unchecked, cb.Checked);
        }
    }
}
