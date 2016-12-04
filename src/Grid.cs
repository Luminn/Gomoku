using System;
using System.Collections.Generic;
namespace Luminoxce.Gomoku
{
    /// <summary>
    /// A 2x2 array that can take Point as it's indexer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Grid<T>
    {

        T[,] _Grid;

        public int Rows { get { return _Grid.GetLength(0); } }

        public int Columns { get { return _Grid.GetLength(1); } }

        public T this[int x, int y]
        {
            get { return _Grid[x, y]; }
            set { _Grid[x, y] = value; }
        }

        public T this[Point p]
        {
            get { return _Grid[p.x, p.y]; }
            set { _Grid[p.x, p.y] = value; }
        }

        public Grid(int row, int col)
        {
            _Grid = new T[row, col];
        }
    }
}