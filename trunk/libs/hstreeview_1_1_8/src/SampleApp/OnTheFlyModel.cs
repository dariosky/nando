using System;
using System.Collections.Generic;
using System.Collections;
using HS.Controls.Tree.Base;

namespace SampleApp
{
    class OnTheFlyItem
    {
        private int _value;
        public int Value { get { return _value; } }
        public string Text { get { return _value.ToString(); } }
        public OnTheFlyItem(int value)
        {
            _value = value;
        }
    }

    class OnTheFlyModel : BaseTreeModel
    {
        private const int ENUMERATE_COUNT = 1000;
        private const int SUB_ENUMERATE_COUNT = 5;
        public int Current = 0;

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                for (int i = 0; i < ENUMERATE_COUNT; i++)
                {
                    Current = i;
                    Enumerate(this, EventArgs.Empty);
                    yield return new OnTheFlyItem(i);
                }
            }
            else
            {
                for (int i = 0; i < SUB_ENUMERATE_COUNT; i++)
                    yield return new OnTheFlyItem(i);
            }
        }

        public override int GetChildrenCount(TreePath treePath)
        {
            if (treePath.IsEmpty())
                return ENUMERATE_COUNT;
            else if (treePath.FullPath.Length <= 2)
                return SUB_ENUMERATE_COUNT;
            else
                return 0;
        }

        public event EventHandler Enumerate;
    }
}
