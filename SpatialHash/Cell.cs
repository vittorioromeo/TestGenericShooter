using System.Collections.Generic;
using System.Linq;
using TestGenericShooter.Components;

namespace TestGenericShooter.SpatialHash
{
    public class Cell
    {
        private readonly Dictionary<string, HashSet<CBody>> _groupedBodies;

        public Cell() { _groupedBodies = new Dictionary<string, HashSet<CBody>>(); }

        public void AddBody(CBody mBody)
        {
            foreach (var group in mBody.GetGroups())
            {
                if (!_groupedBodies.ContainsKey(group))
                    _groupedBodies.Add(group, new HashSet<CBody>());

                _groupedBodies[group].Add(mBody);
            }
        }
        public void RemoveBody(CBody mBody)
        {
            foreach (var group in mBody.GetGroups())
                if (_groupedBodies.ContainsKey(group))
                    _groupedBodies[group].Remove(mBody);
        }
        public IEnumerable<CBody> GetBodies(string mGroup) { return !_groupedBodies.ContainsKey(mGroup) ? new HashSet<CBody>() : _groupedBodies[mGroup]; }
        public bool HasGroup(string mGroup) { return _groupedBodies.ContainsKey(mGroup) && _groupedBodies[mGroup].Any(); }
    }
}