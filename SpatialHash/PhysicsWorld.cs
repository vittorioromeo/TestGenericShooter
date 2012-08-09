using System.Collections.Generic;
using TestGenericShooter.Components;

namespace TestGenericShooter.SpatialHash
{
    public class PhysicsWorld
    {
        private readonly int _cellSize;
        private readonly Cell[,] _cells;
        private readonly int _columns;
        private readonly IEnumerable<int> _groups;
        private readonly int _offset;
        private readonly int _rows;

        public PhysicsWorld(IEnumerable<int> mGroups, int mColumns, int mRows, int mCellSize, int mOffset = 0)
        {
            _groups = mGroups;
            _cells = new Cell[mColumns,mRows];
            _columns = mColumns;
            _rows = mRows;
            _cellSize = mCellSize;
            _offset = mOffset;

            for (var iX = 0; iX < mColumns; iX++)
                for (var iY = 0; iY < mRows; iY++)
                {
                    var left = 0 + iX*mCellSize;
                    var right = _cellSize + iX * mCellSize;
                    var top = 0 + iY * mCellSize;
                    var bottom = _cellSize + iY * mCellSize;
                    
                    _cells[iX, iY] = new Cell(left, right, top, bottom, _groups);
                }
        }


        private HashSet<Cell> CalculateCells(CBody mBody)
        {
            var startX = mBody.Left/_cellSize + _offset;
            var startY = mBody.Top/_cellSize + _offset;
            var endX = mBody.Right/_cellSize + _offset;
            var endY = mBody.Bottom/_cellSize + _offset;

            var result = new HashSet<Cell>();

            if (startX < 0 || endX >= _columns || startY < 0 || endY >= _rows)
            {
                mBody.Entity.Destroy();
                return result;
            }

            for (var iY = startY; iY <= endY; iY++)
                for (var iX = startX; iX <= endX; iX++)
                    result.Add(_cells[iX, iY]);

            return result;
        }

        public void AddBody(CBody mBody)
        {
            mBody.Cells = CalculateCells(mBody);
            foreach (var cell in mBody.Cells)
                cell.AddBody(mBody);
        }
        public void RemoveBody(CBody mBody)
        {
            foreach (var cell in mBody.Cells)
                cell.RemoveBody(mBody);
        }
        public void UpdateBody(CBody mBody)
        {
            RemoveBody(mBody);
            AddBody(mBody);
        }

        public List<CBody> GetBodies(CBody mBody)
        {
            var result = new List<CBody>();

            foreach (var cell in mBody.Cells)
                foreach (var group in mBody.GroupsToCheck)
                    foreach (var body in cell.GroupedBodies[group])
                        result.Add(body);

            return result;
        }

        public bool[,] GetObstacleMap(int mObstacleGroup)
        {
            var result = new bool[_columns,_rows];

            for (var iX = 0; iX < _columns; iX++)
                for (var iY = 0; iY < _rows; iY++)
                    result[iX, iY] = _cells[iX, iY].HasGroup(mObstacleGroup);

            return result;
        }
    }
}