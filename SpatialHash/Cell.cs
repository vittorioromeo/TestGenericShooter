using System.Collections.Generic;
using System.Linq;
using TestGenericShooter.Components;

namespace TestGenericShooter.SpatialHash
{
    public class Cell
    {
        public Cell(int mLeft, int mRight, int mTop, int mBottom, IEnumerable<int> mGroups)
        {
            Left = mLeft;
            Right = mRight;
            Top = mTop;
            Bottom = mBottom;

            GroupedBodies = new Dictionary<int, List<CBody>>();
            foreach (var group in mGroups) GroupedBodies.Add(group, new List<CBody>());
        }

        public int Left { get; private set; }
        public int Right { get; private set; }
        public int Top { get; private set; }
        public int Bottom { get; private set; }

        public Dictionary<int, List<CBody>> GroupedBodies { get; set; }

        public void AddBody(CBody mBody)
        {
            foreach (var group in mBody.Groups)
                GroupedBodies[group].Add(mBody);
        }
        public void RemoveBody(CBody mBody)
        {
            foreach (var group in mBody.Groups)
                GroupedBodies[group].Remove(mBody);
        }
        public bool HasGroup(int mGroup) { return GroupedBodies[mGroup].Any(); }
    }
}