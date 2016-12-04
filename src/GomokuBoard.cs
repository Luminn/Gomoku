using System;
using System.Collections.Generic;

namespace Luminoxce.Gomoku
{
    /// <summary>
    /// A board for a PvP gomoku game.
    /// </summary>
    public class GomokuBoard
    {
        /// <summary>
        /// Number of Rows in the board.
        /// </summary>
        public int Rows { get { return board.Rows; } }
        /// <summary>
        /// Number of columns in the board
        /// </summary>
        public int Columns { get { return board.Columns; } }

        public delegate void BoardEventHandler(object sender, BoardEventArgs args);
        /// <summary>
        /// An event raised when a piese is put.
        /// </summary>
        public event BoardEventHandler OnPut;
        /// <summary>
        /// An event raised when a piece is removed.
        /// </summary>
        public event BoardEventHandler OnRemove;

        protected Grid<int> board;
        private int[] playerModifiers = { 0, 4, 100 };

        /// <summary>
        /// Constructor of the board.
        /// </summary>
        /// <param name="rows">Number of rows in the board.</param>
        /// <param name="cols">Number of columns in the board.</param>
        public GomokuBoard(int rows, int cols)
        {
            board = new Grid<int>(rows, cols);
        }

        /// <summary>
        /// Refresh the board to initial state.
        /// </summary>
        public virtual void Refresh()
        {
            board = new Grid<int>(Rows, Columns);
            foreach (Point gp in Point.Enumerate(Rows, Columns))
            {
                board[gp] = 0;
            }
        }

        /// <summary>
        /// Check whether a point is empty.
        /// </summary>
        /// <param name="gp">The position of the point.</param>
        /// <returns>true, if the point is empty; otherwise false</returns>
        public bool CheckEmpty(Point gp)
        {
            return board[gp] == 0;
        }

        /// <summary>
        /// Put a piece on board.
        /// </summary>
        /// <param name="gp">The position of the piece.</param>
        /// <param name="player">The number of the player.</param>
        /// <returns>true, if the play is valid; otherwise false</returns>
        public virtual bool PutPiece(Point gp, int player)
        {
            if (board[gp] != 0)
                return false;
            board[gp] = player;
            onPut(player, gp);
            return true;
        }

        /// <summary>
        /// Remove a piece from board.
        /// </summary>
        /// <param name="gp">The position of the piece.</param>
        public virtual void RemovePiece(Point gp)
        {
            board[gp] = 0;
            onRemove(0, gp);
        }

        protected void onPut(int player, Point gp)
        {
            OnPut(this, new BoardEventArgs(player, gp));
        }

        protected void onRemove(int player, Point gp)
        {
            OnRemove(this, new BoardEventArgs(player, gp));
        }

        /// <summary>
        /// Test if putting a piece on the point makes the player winner.
        /// </summary>
        /// <param name="gp">The point of the piece.</param>
        /// <returns>true, if the player is the winner.</returns>
        public bool testWinner(Point gp)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = -5; j <= 0; j++)
                {
                    int sum = 0;
                    try
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            sum += playerModifiers[board[gp + Point.PositionVectors[i] * (j + k)]];
                        }
                    }
                    catch { }
                    if (sum == playerModifiers[board[gp] * 5])
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Test if putting a piece on the point makes the player winner, for indication use.
        /// </summary>
        /// <param name="gp">The point of the piece.</param>
        /// <param name="result">The list of points that makes the player win.</param>
        /// <returns>true, if the player is the winner.</returns>
        public bool testWinner(Point gp, List<Point> result)
        {
            bool test = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = -5; j <= 0; j++)
                {
                    int sum = 0;
                    try
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            sum += playerModifiers[board[gp + Point.PositionVectors[i] * (j + k)]];
                        }
                    }
                    catch
                    {
                        sum = 0;
                    }
                    if (sum == playerModifiers[board[gp]] * 5)
                    {
                        test = true;
                        for (int k = 0; k < 5; k++)
                        {
                            result.Add(gp + Point.PositionVectors[i] * (j + k));
                        }
                        try
                        {
                            int t = -1;
                            while (board[gp + Point.PositionVectors[i] * (j + t)] == board[gp])
                            {
                                result.Add(gp + Point.PositionVectors[i] * (j + t));
                                t--;
                            }
                        }
                        catch { }
                        try
                        {
                            int t = 5;
                            while (board[gp + Point.PositionVectors[i] * (j + t)] == board[gp])
                            {
                                result.Add(gp + Point.PositionVectors[i] * (j + t));
                                t++;
                            }
                        }
                        catch { }

                    }
                }
            }

            return test;
        }

        /// <summary>
        /// Test if putting a piece on the point makes the player have four pieces in a line, for indication use.
        /// </summary>
        /// <param name="gp">The point of the piece.</param>
        /// <param name="result">The list of points the four pieces in a line.</param>
        /// <returns>true, if the player have four pieces in a line.</returns>
        public bool testFour(Point gp, List<Point> result)
        {
            bool test = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = -5; j <= 0; j++)
                {
                    int sum = 0;
                    try
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            sum += playerModifiers[board[gp + Point.PositionVectors[i] * (j + k)]];
                        }
                    }
                    catch
                    {
                        sum = 0;
                    }
                    if (sum == playerModifiers[board[gp]] * 4)
                    {
                        test = true;
                        for (int k = 0; k < 5; k++)
                        {
                            result.Add(gp + Point.PositionVectors[i] * (j + k));
                        }
                        if (board[gp + Point.PositionVectors[i] * j] != 0 && board[gp + Point.PositionVectors[i] * (j + 4)] != 0)
                        {
                            try
                            {
                                int t = -1;
                                while (board[gp + Point.PositionVectors[i] * (j + t)] == board[gp])
                                {
                                    result.Add(gp + Point.PositionVectors[i] * (j + t));
                                    t--;
                                }
                            }
                            catch { }
                            try
                            {
                                int t = 5;
                                while (board[gp + Point.PositionVectors[i] * (j + t)] == board[gp])
                                {
                                    result.Add(gp + Point.PositionVectors[i] * (j + t));
                                    t++;
                                }
                            }
                            catch { }
                        }
                    }
                }
            }
            return test;
        }

        /// <summary>
        /// Test if putting a piece on the point makes the player have three free pieces in a line, for indication use.
        /// </summary>
        /// <param name="gp">The point of the piece.</param>
        /// <param name="result">The list of points the three pieces in a line.</param>
        /// <returns>true, if the player have three free pieces in a line.</returns>
        public bool testFreeThree(Point gp, List<Point> result)
        {
            bool test = false;
            test = test || testThree(gp, result);
            test = test || testTwoOne(gp, result);
            return test;
        }

        public bool testThree(Point gp, List<Point> result)
        {
            bool test = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = -2; j <= 0; j++)
                {
                    int sum = 0;
                    bool Unbound = true;
                    try
                    {
                        Unbound = Unbound && 0 == board[gp + Point.PositionVectors[i] * (j - 1)];
                        Unbound = Unbound && 0 == board[gp + Point.PositionVectors[i] * (j + 3)];
                        for (int k = 0; k < 3; k++)
                        {
                            sum += playerModifiers[board[gp + Point.PositionVectors[i] * (j + k)]];
                        }
                    }
                    catch
                    {
                        sum = 0;
                    }
                    if (sum == playerModifiers[board[gp]] * 3 && Unbound)
                    {
                        test = true;
                        for (int k = 0; k < 3; k++)
                        {
                            result.Add(gp + Point.PositionVectors[i] * (j + k));
                        }
                    }
                }
            }
            return test;
        }

        public bool testTwoOne(Point gp, List<Point> result)
        {
            bool test = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = -3; j <= 0; j++)
                {
                    int sum = 0;
                    bool Res = true;
                    try
                    {
                        Res = Res && 0 == board[gp + Point.PositionVectors[i] * (j - 1)];
                        Res = Res && 0 == board[gp + Point.PositionVectors[i] * (j + 4)];
                        Res = Res && board[gp] == board[gp + Point.PositionVectors[i] * j];
                        Res = Res && board[gp] == board[gp + Point.PositionVectors[i] * (j + 3)];
                        for (int k = 1; k < 3; k++)
                        {
                            sum += playerModifiers[board[gp + Point.PositionVectors[i] * (j + k)]];
                        }
                    }
                    catch
                    {
                        sum = 0;
                    }
                    if (sum == playerModifiers[board[gp]] && Res)
                    {
                        test = true;
                        for (int k = 0; k < 4; k++)
                        {
                            result.Add(gp + Point.PositionVectors[i] * (j + k));
                        }
                    }
                }
            }
            return test;
        }

        /// <summary>
        /// Test whether the first player has disobeyed the rule against having more than five pieces in a line in a professional match.
        /// For Professional matches only.
        /// </summary>
        /// <param name="gp">The position of the piece.</param>
        /// <returns>true, if the player has violated the rule.</returns>
        public bool banTestBeforeWinCheck(Point gp)
        {
            return testMoreThanFiveBan(gp);
        }

        /// <summary>
        /// Test whether the first player has disobeyed the rules in a professional match.
        /// For Professional matches only. 
        /// Please evaluate after win check.
        /// </summary>
        /// <param name="gp">The position of the piece.</param>
        /// <returns>true, if the player has violated the rule.</returns>
        public bool banTestAfterWinCheck(Point gp)
        {
            return testDoubleThreeBan(gp) || testThreeFourBan(gp);
        }

        bool testThreeFourBan(Point gp)
        {
            var test = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = -5; j <= 0; j++)
                {
                    int sum = 0;
                    try
                    {
                        for (int k = 0; k < 5; k++)
                        {
                            sum += playerModifiers[board[gp + Point.PositionVectors[i] * (j + k)]];
                        }
                    }
                    catch
                    {
                        sum = 0;
                    }
                    if (sum == playerModifiers[board[gp]] * 4)
                    {
                        test++;
                        break;
                    }
                }
            }
            return test >= 2;
        }

        bool testDoubleThreeBan(Point gp)
        {
            return testThreeBan(gp) + testTwoOneBan(gp) >= 2;
        }

        int testThreeBan(Point gp)
        {
            var test = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = -2; j <= 0; j++)
                {
                    int sum = 0;
                    bool Free = true;
                    try
                    {
                        Free = Free && 0 == board[gp + Point.PositionVectors[i] * (j - 1)];
                        Free = Free && 0 == board[gp + Point.PositionVectors[i] * (j + 3)];
                        for (int k = 0; k < 3; k++)
                        {
                            sum += playerModifiers[board[gp + Point.PositionVectors[i] * (j + k)]];
                        }
                        if (sum == playerModifiers[board[gp]] * 3 && Free)
                        {
                            test++;
                            break;
                        }
                    }
                    catch
                    {
                        sum = 0;
                    }

                }
            }
            return test;
        }

        int testTwoOneBan(Point gp)
        {
            var test = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = -3; j <= 0; j++)
                {
                    int sum = 0;
                    bool Res = true;
                    try
                    {
                        Res = Res && 0 == board[gp + Point.PositionVectors[i] * (j - 1)];
                        Res = Res && 0 == board[gp + Point.PositionVectors[i] * (j + 4)];
                        Res = Res && board[gp] == board[gp + Point.PositionVectors[i] * j];
                        Res = Res && board[gp] == board[gp + Point.PositionVectors[i] * (j + 3)];
                        for (int k = 1; k < 3; k++)
                        {
                            sum += playerModifiers[board[gp + Point.PositionVectors[i] * (j + k)]];
                        }
                        if (sum == playerModifiers[board[gp]] && Res)
                        {
                            test++;
                            break;
                        }
                    }
                    catch
                    {
                        sum = 0;
                    }

                }
            }
            return test;
        }

        bool testMoreThanFiveBan(Point gp)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = -6; j <= 0; j++)
                {
                    int sum = 0;
                    try
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            sum += playerModifiers[board[gp + Point.PositionVectors[i] * (j + k)]];
                        }
                        if (sum == playerModifiers[board[gp]] * 6)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        sum = 0;
                    }

                }
            }
            return false;
        }

    }

    public class BoardEventArgs : EventArgs
    {
        public int Player { get; set; }
        public Point Point { get; set; }
        public BoardEventArgs(int player, Point point)
        {
            Player = player;
            Point = point;
        }
    }
}