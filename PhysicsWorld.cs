using System.Collections.Generic;
using TestGenericShooter.Components;

namespace TestGenericShooter
{
    public class PhysicsWorld
    {
        private readonly int _cellSize;
        private readonly Cell[,] _cells;
        private readonly int _columns;
        private readonly int _offsetX, _offsetY;
        private readonly int _range;
        private readonly int _rows;

        public PhysicsWorld(int mColumns, int mRows, int mCellSize, int mOffsetX = 0, int mOffsetY = 0, int mRange = 0)
        {
            _cells = new Cell[mColumns,mRows];
            _columns = mColumns;
            _rows = mRows;
            _cellSize = mCellSize;
            _offsetX = mOffsetX;
            _offsetY = mOffsetY;
            _range = mRange;

            for (var iX = 0; iX < mColumns; iX++)
                for (var iY = 0; iY < mRows; iY++)
                    _cells[iX, iY] = new Cell();
        }

        private HashSet<Cell> CalculateCells(CBody mBody)
        {
            var result = new HashSet<Cell>();

            var startX = mBody.Left/_cellSize - _range + _offsetX;
            var endX = mBody.Right/_cellSize + _range + _offsetX;
            var startY = mBody.Top/_cellSize - _range + _offsetY;
            var endY = mBody.Bottom/_cellSize + _range + _offsetY;

            for (var iY = startY; iY <= endY; iY++)
                for (var iX = startX; iX <= endX; iX++)
                    result.Add(_cells[iX, iY]);

            return result;
        }

        public void AddBody(CBody mBody)
        {
            var cells = CalculateCells(mBody);
            mBody.Cells = cells;
            foreach (var cell in cells) cell.AddBody(mBody);
        }
        public static void RemoveBody(CBody mBody)
        {
            foreach (var cell in mBody.Cells)
                cell.RemoveBody(mBody);
        }
        public void UpdateBody(CBody mBody)
        {
            RemoveBody(mBody);
            AddBody(mBody);
        }

        public bool IsOutOfBounds(CBody mBody)
        {
            return mBody.Left < 0 - _offsetX*_cellSize
                   || mBody.Right > _columns*_cellSize + _offsetX*_cellSize
                   || mBody.Top < 0 - _offsetY*_cellSize
                   || mBody.Bottom > _rows*_cellSize + _offsetY*_cellSize;
        }

        public static IEnumerable<CBody> GetBodies(CBody mBody)
        {
            foreach (var cell in mBody.Cells)
                foreach (var group in mBody.GetGroupsToCheck())
                    foreach (var body in cell.GetBodies(group))
                        yield return body;
        }
    }

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
    }
}