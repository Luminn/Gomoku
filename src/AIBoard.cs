using System;
using System.Collections.Generic;

namespace Luminoxce.Gomoku
{
    /// <summary>
    /// An implementation of GomokuBoard for a AI match.
    /// Warning: the AI is not for professional match.
    /// </summary>
    public class AIBoard : GomokuBoard
    {
        static System.Random Rnd = new System.Random();
        private List<Point> SearchSpace;
        private List<Point> FilledSpace;
        private static Point[] searchVector = { new Point(1, 0), new Point(2, 0), new Point(1, 1), new Point(2, 2), new Point(0, 1), new Point(0, 2), new Point(-1, 1), new Point(-2, 2), new Point(-1, 0), new Point(-2, 0), new Point(-1, -1), new Point(-2, -2), new Point(0, -1), new Point(0, -2), new Point(1, -1), new Point(2, -2) };

        /// <summary>
        /// Constuctor of a gomoku board with an AI.
        /// </summary>
        /// <param name="rows">Number of rows in the board.</param>
        /// <param name="cols">Number of columns in the board.</param>
        public AIBoard(int rows, int cols) : base(rows, cols)
        {
            SearchSpace = new List<Point>();
            FilledSpace = new List<Point>();
        }

        /// <summary>
        /// Refresh the board to initial state.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            SearchSpace = new List<Point>();
            FilledSpace = new List<Point>();
        }

        /// <summary>
        /// Let the AI make a move for a player.
        /// </summary>
        /// <param name="player">The player number of the AI.</param>
        /// <returns>The point that the AI plays.</returns>
        public Point AIPlay(int player)
        {
            Point p = GenerateNextMove(player);
            PutPiece(p, player);
            return p;
        }

        /// <summary>
        /// Let the AI generate a move or a player without playing.
        /// </summary>
        /// <param name="player">The player number of the AI.</param>
        /// <returns>The point that the AI plays.</returns>
        public Point GenerateNextMove(int player)
        {
            int MaxPoints = int.MinValue;
            List<Point> GPOutput = new List<Point>();
            GPOutput.Add(new Point(Rows / 2, Columns / 2));
            foreach (Point gp in SearchSpace)
            {
                if (board[gp] == 0)
                {
                    int point = 0;
                    foreach (Point pv in Point.PositionVectors)
                    {
                        List<int> line = new List<int>();
                        int Pivot = LineBasedAI.getLineInBoard(board, gp, pv, line);
                        LineBasedAI LineAI = new LineBasedAI(line.ToArray(), Pivot, player);
                        point += LineAI.eval();
                    }
                    if (gp.OnRim(Rows, Columns, 1))
                        point -= 20;
                    if (gp.OnRim(Rows, Columns, 2))
                        point -= 10;
                    if (gp.OnRim(Rows, Columns, 3))
                        point -= 5;
                    if (gp.OnRim(Rows, Columns, 4))
                        point -= 1;
                    if (point > MaxPoints)
                    {
                        GPOutput = new List<Point>();
                        GPOutput.Add(gp);
                        MaxPoints = point;
                    }
                    else if (point == MaxPoints)
                        GPOutput.Add(gp);
                }
            }
            return GPOutput[Rnd.Next(GPOutput.Count)];
        }

        /// <summary>
        /// Put a piece on board.
        /// </summary>
        /// <param name="gp">The position of the piece.</param>
        /// <param name="player">The number of the player.</param>
        /// <returns>true, if the play is valid; otherwise false</returns>
        public override bool PutPiece(Point gp, int player)
        {
            if (board[gp] != 0)
                return false;
            board[gp] = player;
            FilledSpace.Add(gp);
            SearchSpace.Remove(gp);
            foreach (Point sv in searchVector)
            {
                try
                {
                    Point ActiveGP = gp + sv;
                    if (board[ActiveGP] == 0 && SearchSpace.IndexOf(ActiveGP) == -1)
                        SearchSpace.Add(ActiveGP);
                }
                catch { }
            }
            onPut(player, gp);
            return true;
        }

        /// <summary>
        /// Remove a piece from the board.
        /// </summary>
        /// <param name="gp">The position of the piece.</param>
        public override void RemovePiece(Point gp)
        {
            board[gp.x, gp.y] = 0;
            FilledSpace.Remove(gp);
            SearchSpace.Add(gp);
            onRemove(0, gp);
        }

        
        private void GenerateSearchSpace()
        {
            SearchSpace = new List<Point>();
            foreach (Point gp in FilledSpace)
            {
                foreach (Point sv in searchVector)
                {
                    try
                    {
                        Point ActiveGP = gp + sv;
                        if (board[ActiveGP] == 0 && SearchSpace.IndexOf(ActiveGP) == -1)
                            SearchSpace.Add(ActiveGP);
                    }
                    catch { }
                }
            }

        }

        private void GenerateFilledSpace()
        {
            FilledSpace = new List<Point>();
            foreach (Point gp in Point.Enumerate(Rows, Columns))
            {
                if (board[gp] != 0)
                    FilledSpace.Add(gp);
            }
        }
    }
}