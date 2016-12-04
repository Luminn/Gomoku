using System;
using System.Collections.Generic;
namespace Luminoxce.Gomoku
{
    public struct Point
    {

        public static Point[] PositionVectors = { new Point(1, 0), new Point(0, 1), new Point(1, 1), new Point(1, -1) };
        
        public int x;
        public int y;

        public Point(int row, int col)
        {
            x = row;
            y = col;
        }
        
        public bool OnBound(int row, int col)
        {
            return (x == 0 || y == 0 || x == row - 1 || y == col - 1) && !outOfBound(row, col);
        }

        public bool OnRim(int row, int col, int rim)
        {
            return (x == rim - 1 || y == rim - 1 || x == row - rim || y == col - rim) && !outOfBound(row, col);
        }

        public bool InBounds(int row, int col)
        {
            return !outOfBound(row, col);
        }

        public bool outOfBound(int row, int col)
        {
            return x < 0 || y < 0 || x >= row || y >= col;
        }

        public static Point operator +(Point p1, Point p2)
        {
            return new Point(p1.x + p2.x, p1.y + p2.y);
        }

        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.x - p2.x, p1.y - p2.y);
        }

        public static Point operator *(Point p, int num)
        {
            return new Point(p.x * num, p.y * num);
        }

        public static Point operator *(int num, Point p)
        {
            return new Point(p.x * num, p.y * num);
        }

        public static bool operator ==(Point p1, Point p2)
        {
            return p1.x == p2.x && p1.y == p2.y;
        }

        public static bool operator !=(Point p1, Point p2)
        {
            return p1.x != p2.x || p1.y != p2.y;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                Point gp = (Point)obj;
                if (gp.x == x && gp.y == y)
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return x * 1000 + y;
        }

        public override string ToString()
        {
            return "(" + x + "," + y + ")";
        }

        /// <summary>
        /// Quickly enumerate through points in a grid.
        /// </summary>
        /// <param name="x">Rows</param>
        /// <param name="y">Columns</param>
        /// <returns>An IEnumerable contains all points in the grid.</returns>
        public static IEnumerable<Point> Enumerate(int x, int y)
        {
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    yield return new Point(x, y);
                }
            }
        }
    }
}

