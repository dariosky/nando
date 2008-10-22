using System;
using System.Collections.Generic;
using System.Text;

namespace HS.Controls.Tree.Base
{
    public struct Range
    {
        private int _start;
        public int Start { get { return _start; } }

        private int _end;
        public int End { get { return _end; } }

        public int Size { get { return _end - _start + 1; } }

        public Range(int start, int end)
        {
            if (end < start)
                end = start;
            _start = start;
            _end = end;
        }

        public bool Contains(int index)
        {
            return (index >= Start) && (index <= End);
        }

        public bool Contains(Range r)
        {
            return Contains(r.Start) && Contains(r.End);
        }

        public bool Touches(int index)
        {
            return Contains(index) || (index == Start - 1) || (index == End + 1);
        }

        public bool Touches(Range r)
        {
            if (Contains(r.Start) || Contains(r.End) || r.Contains(this))
                return true;
            if (r.Start == End + 1)
                return true;
            if (r.End == Start - 1)
                return true;
            return false;
        }

        public Range Union(int index)
        {
            return new Range(Math.Min(Start, index), Math.Max(End, index));
        }

        public Range Union(Range r)
        {
            return new Range(Math.Min(Start,r.Start), Math.Max(End, r.End));
        }
    }

    public class Ranges
    {
        private List<Range> _ranges;

        public int First
        {
            get
            {
                int result = -1;
                foreach (Range r in _ranges)
                {
                    if (result < 0)
                        result = r.Start;
                    else
                        result = Math.Min(result, r.Start);
                }
                return result;
            }
        }

        public int Last
        {
            get
            {
                int result = -1;
                foreach (Range r in _ranges)
                        result = Math.Max(result, r.End);
                return result;
            }
        }

        public Ranges()
        {
            _ranges = new List<Range>();
        }

        public void Add(int index)
        {
            if (Contains(index))
                return;
            Add(new Range(index, index));
        }

        public void Add(Range range)
        {
            Range toAdd = range;
            List<Range> toRemove = new List<Range>();
            foreach (Range r in _ranges)
            {
                if (r.Touches(toAdd))
                {
                    toRemove.Add(r);
                    toAdd = toAdd.Union(r);
                }
            }
            foreach (Range r in toRemove)
                _ranges.Remove(r);
            _ranges.Add(toAdd);
        }

        public void Clear()
        {
            _ranges.Clear();
        }

        //Remove from this the Range "range", and reduce all following indexes by the size of r.
        public void Collapse(Range range)
        {
            List<Range> toRemove = new List<Range>();
            List<Range> toAdd = new List<Range>();
            foreach (Range r in _ranges)
            {
                Range newRange = new Range(-1, -1);
                if (range.Contains(r))
                    toRemove.Add(r);
                else if (r.Start <= range.Start)
                {
                    if (r.End >= range.End)
                        newRange = new Range(r.Start, r.End - range.Size);
                    else if (r.End >= range.Start)
                        newRange = new Range(r.Start, range.Start - 1);
                }
                else
                {
                    if (r.Start <= range.End)
                        newRange = new Range(range.Start, r.End - range.Size);
                    else
                        newRange = new Range(r.Start - range.Size, r.End - range.Size);
                }
                if (newRange.Start >= 0)
                {
                    toRemove.Add(r);
                    toAdd.Add(newRange);
                }
            }
            foreach (Range r in toRemove)
                _ranges.Remove(r);
            foreach (Range r in toAdd)
                _ranges.Add(r);
        }

        public bool Contains(int index)
        {
            foreach (Range r in _ranges)
            {
                if (r.Contains(index))
                    return true;
            }
            return false;
        }


        //Insert "range" by pushing subsequent indexes. if range.Start AND range.Start - 1 are in Ranges, the expanded
        //range will be selected.
        public void Expand(Range range)
        {
            List<Range> toRemove = new List<Range>();
            List<Range> toAdd = new List<Range>();
            foreach (Range r in _ranges)
            {
                Range newRange = new Range(-1, -1);
                if (r.Start >= range.Start)
                    newRange = new Range(r.Start + range.Size, r.End + range.Size);
                else if (r.Contains(range.Start))
                    newRange = new Range(r.Start, r.End + range.Size);
                if (newRange.Start >= 0)
                {
                    toRemove.Add(r);
                    toAdd.Add(newRange);
                }
            }
            foreach (Range r in toRemove)
                _ranges.Remove(r);
            foreach (Range r in toAdd)
                _ranges.Add(r);
        }

        public IEnumerator<int> GetEnumerator()
        {
            for (int i = First; i <= Last; i++)
            {
                if (Contains(i))
                    yield return i;
            }
        }

        public bool Remove(int index)
        {
            foreach (Range r in _ranges)
            {
                if (r.Contains(index))
                {
                    _ranges.Remove(r);
                    if (index > r.Start)
                        Add(new Range(r.Start, index - 1));
                    if (index < r.End)
                        Add(new Range(index + 1, r.End));
                    return true;
                }
            }
            return false;
        }
    }
}
