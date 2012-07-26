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

        public PhysicsWorld(int mColumns, int mRows, int mCellSize)
        {
            _cells = new Cell[mColumns,mRows];
            _columns = mColumns;
            _rows = mRows;
            _cellSize = mCellSize;

            for (var iX = 0; iX < mColumns; iX++)
                for (var iY = 0; iY < mRows; iY++)
                    _cells[iX, iY] = new Cell();
        }

        private Cell CalculateCell(int mX, int mY)
        {
            var x = mX/_cellSize;
            var y = mY/_cellSize;

            return _cells[x, y];
        }
        private HashSet<Cell> CalculateCells(CBody mBody)
        {
            return new HashSet<Cell>
                   {
                       CalculateCell(mBody.Left, mBody.Top),
                       CalculateCell(mBody.Left, mBody.Bottom),
                       CalculateCell(mBody.Right, mBody.Top),
                       CalculateCell(mBody.Right, mBody.Bottom)
                   };
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
            var cells = CalculateCells(mBody);
            //if(cell == mBody.Cell) return;

            RemoveBody(mBody);
            mBody.SetCells(cells);
            foreach (var cell in cells) cell.AddBody(mBody);
        }

        public bool IsOutOfBounds(CBody mBody) { return mBody.Left < 0 || mBody.Top < 0 || mBody.Right > _columns*_cellSize || mBody.Bottom > _rows*_cellSize; }

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
        public IEnumerable<CBody> GetBodies(string mGroup) { return !_groupedBodies.ContainsKey(mGroup) ? new HashSet<CBody>() : new HashSet<CBody>(_groupedBodies[mGroup]); }
    }
}