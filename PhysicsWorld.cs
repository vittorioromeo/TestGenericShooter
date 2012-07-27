using System.Collections.Generic;
using TestGenericShooter.Components;

namespace TestGenericShooter
{
    public class PhysicsWorld
    {
        private readonly int _cellSize;
        private readonly Cell[,] _cells;
        private readonly int _columns;
        private readonly int _rows;
        private readonly int _offsetX;
        private readonly int _offsetY;
        private readonly int _range;

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

            var startX = mBody.Left / _cellSize;
            var endX = mBody.Right / _cellSize;
            var startY = mBody.Top / _cellSize;
            var endY = mBody.Bottom / _cellSize;

            for (var iY = startY - _range + _offsetY; iY <= endY + _range + _offsetY; iY++)
                for (var iX = startX - _range + _offsetX; iX <= endX + _range + _offsetX; iX++)
                    result.Add(_cells[iX, iY]);

            return result;
        }

        public void AddBody(CBody mBody)
        {
            var cells = CalculateCells(mBody);
            mBody.SetCells(cells);
            foreach (var cell in cells) cell.AddBody(mBody);
        }
        public void RemoveBody(CBody mBody) { foreach (var cell in mBody.Cells) cell.RemoveBody(mBody); }
        public void UpdateBody(CBody mBody)
        {
            RemoveBody(mBody);
            AddBody(mBody);
        }

        public bool IsOutOfBounds(CBody mBody)
        {
            return mBody.Left < 0 - _offsetX * _cellSize
            || mBody.Right > _columns * _cellSize + _offsetX * _cellSize
            || mBody.Top < 0 - _offsetY * _cellSize
            || mBody.Bottom > _rows * _cellSize + _offsetY * _cellSize;
        }

        public List<CBody> GetBodies(CBody mBody)
        {
            var result = new List<CBody>();

            foreach (var group in mBody.GetGroupsToCheck())
                foreach (var cell in CalculateCells(mBody))
                    foreach(var body in cell.GetBodies(group)) 
                        result.Add(body);

            return result;
        }
    }

    public class Cell
    {
        private readonly Dictionary<string, HashSet<CBody>> _groupedBodies;

        public Cell() { _groupedBodies = new Dictionary<string, HashSet<CBody>>();}

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