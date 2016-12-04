using System;
using System.Collections.Generic;

namespace Luminoxce.Gomoku
{
    internal class LineBasedAI
    {
        private int[] LeftSpace;
        private int[] RightSpace;
        private int AnchorPoint;
        private int Length;
        private int LLength;
        private int RLength;

        public static int CONST_FREESPACE = 2;
        public static int LS_EMPTYLINE = 1;

        public LineBasedAI(int[] Line, int anchor, int player)
        {
            int[] LineSpace = Line;
            LineSpace[anchor] = player;
            AnchorPoint = anchor;
            Length = Line.Length;
            for (int i = 0; i < Length; i++)
            {
                if (Line[i] == player)
                    Line[i] = 1;
                else if (Line[i] != 0)
                    Line[i] = 9;
            }
            LeftSpace = PartOfArray(Line, anchor - 1, 0);
            RightSpace = PartOfArray(Line, anchor + 1, Length - 1);
            LLength = LeftSpace.Length;
            RLength = RightSpace.Length;
        }


        public static int[] PartOfArray(int[] array, int LBound, int UBound)
        {
            List<int> LArray = new List<int>();
            if (LBound == -1 || UBound == -1 || LBound == array.Length || UBound == array.Length)
                return new int[0];
            if (UBound >= LBound)
            {
                for (int i = LBound; i <= UBound; i++)
                {
                    LArray.Add(array[i]);
                }
            }
            else
            {
                for (int i = LBound; i >= UBound; i--)
                {
                    LArray.Add(array[i]);
                }
            }
            return LArray.ToArray();

        }

        public static int getLineInBoard(Grid<int> board, Point Point, Point PositionVector, List<int> Result)
        {
            int row = board.Rows;
            int col = board.Columns;
            int anchor = 5;
            for (int i = -5; i <= 5; i++)
            {
                if (i == 0)
                    anchor = Result.Count;
                if (!(Point + i * PositionVector).outOfBound(row, col))
                    Result.Add(board[Point + i * PositionVector]);
            }

            return anchor;
        }

        public int eval()
        {
            int Res = 0;
            try
            {
                Hit LHit = FirstHit(LeftSpace);
                Hit RHit = FirstHit(RightSpace);
                if (LHit.pos >= CONST_FREESPACE && RHit.pos >= CONST_FREESPACE)
                {
                    return LS_EMPTYLINE;
                }
                else if (LLength > 0 && RLength > 0 && LeftSpace[0] == 9 && RightSpace[0] == 9 && LLength + RLength >= 5)
                {
                    return new cutInAction(LeftSpace, RightSpace).Calculate();
                }

                if ((LHit.val == 1 && LHit.isCloseHit()) || (RHit.val == 1 && RHit.isCloseHit()))
                {
                    Res += new extendingAction(LeftSpace, RightSpace).Calculate();
                }

                Res += new counterAction(LeftSpace, RightSpace).Calculate();

                if (LLength >= 4 && LeftSpace[0] == 9)
                {
                    Res += new blockingAction(LeftSpace).Calculate();
                }


                if (RLength >= 4 && RightSpace[0] == 9)
                {
                    Res += new blockingAction(RightSpace).Calculate();
                }



            }
            catch { }
            return Res;
        }

        public Hit FirstHit(int[] array)
        {
            int k = -1;
            foreach (int i in array)
            {
                k++;
                if (i != 0)
                {
                    return new Hit(k, i);
                }
            }
            return new Hit(0, k);
        }

    }


    internal class Hit
    {
        public int pos;
        public int val;
        public Hit(int position, int player)
        {
            pos = position;
            val = player;
        }

        public bool isDirectHit()
        {
            return pos == 0;
        }

        public bool isCloseHit()
        {
            return pos < 2;
        }

    }

    internal class extendingAction
    {
        public static int EL_TWO = 20;
        public static int EL_ONEONE = 20;
        public static int EL_THREE = 1000;
        public static int EL_TWOONE = 990;
        public static int EL_THREE_BOUND = 15;
        public static int EL_THREE_TRAPPED = 300;
        public static int EL_FOUR = 30000;
        public static int EL_FOUR_BOUND = 10000;
        public static int EL_TOFIVE = 10000;
        public static int EL_THREEONE = 20000;
        public static int EL_FIVE = 999999999;
        int[] LeftSpace;
        int[] RightSpace;
        int freeLength;
        public extendingAction(int[] LeftSpace, int[] RightSpace)
        {
            this.LeftSpace = LeftSpace;
            this.RightSpace = RightSpace;
            freeLength = calcFreeSpace();
        }

        public int Calculate()
        {
            if (isFive())
                return EL_FIVE;
            if (isFour())
                return EL_FOUR;
            if (isThreeOne())
                return EL_THREEONE;
            if (isToFive())
                return EL_TOFIVE;
            if (isFourBound())
                return EL_FOUR_BOUND;
            if (isThreeTrapped())
                return EL_THREE_TRAPPED;
            if (isThree())
                return EL_THREE;
            if (isTwoOne())
                return EL_TWOONE;
            if (isThreeBound())
                return EL_THREE_BOUND;
            if (isTwo())
                return EL_TWO;
            if (isOneOne())
                return EL_ONEONE;
            return 0;
        }

        public int calcFreeSpace()
        {
            int k = 1;
            foreach (int i in LeftSpace)
            {
                if (i == 9)
                    break;
                k++;
            }
            foreach (int i in RightSpace)
            {
                if (i == 9)
                    break;
                k++;
            }
            if (k < 5)
                return 0;
            return 2;
        }

        private bool isFive()
        {
            try
            {
                if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && LeftSpace[2] == 1 && LeftSpace[3] == 1)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && LeftSpace[2] == 1 && RightSpace[0] == 1)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && RightSpace[1] == 1 && RightSpace[0] == 1)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 1 && RightSpace[2] == 1 && RightSpace[1] == 1 && RightSpace[0] == 1)
                    return true;
            }
            catch { }
            try
            {
                if (RightSpace[3] == 1 && RightSpace[2] == 1 && RightSpace[1] == 1 && RightSpace[0] == 1)
                    return true;
            }
            catch { }
            return false;
        }

        private bool isThreeOne()
        {
            try
            {
                if (LeftSpace[0] == 1 && RightSpace[0] == 1 && ((LeftSpace[1] == 0 && LeftSpace[2] == 1 && RightSpace[1] == 0) || (RightSpace[1] == 0 && RightSpace[2] == 1 && LeftSpace[1] == 0)))
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[1] == 1 && RightSpace[1] == 1 && ((LeftSpace[0] == 1 && LeftSpace[2] == 0 && RightSpace[0] == 0) || (RightSpace[0] == 1 && RightSpace[2] == 0 && LeftSpace[0] == 0)))
                    return true;
            }
            catch { }
            try
            {
                if ((LeftSpace[0] == 1 && LeftSpace[1] == 1 && LeftSpace[2] == 0 && LeftSpace[3] == 1 && RightSpace[1] == 0) || (RightSpace[0] == 1 && RightSpace[1] == 1 && RightSpace[2] == 0 && RightSpace[3] == 1 && LeftSpace[1] == 0))
                    return true;
            }
            catch { }
            try
            {
                if ((LeftSpace[0] == 0 && LeftSpace[1] == 1 && LeftSpace[2] == 1 && LeftSpace[3] == 1 && LeftSpace[4] == 0) || (RightSpace[0] == 0 && RightSpace[1] == 1 && RightSpace[2] == 1 && RightSpace[3] == 1 && RightSpace[4] == 0))
                    return true;
            }
            catch { }
            return false;
        }

        private bool isToFive()
        {
            try
            {
                if (LeftSpace[0] == 1 && RightSpace[0] == 1 && ((LeftSpace[1] == 0 && LeftSpace[2] == 1 && RightSpace[1] == 9) || (RightSpace[1] == 0 && RightSpace[2] == 1 && LeftSpace[1] == 9)))
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[1] == 1 && RightSpace[1] == 1 && ((LeftSpace[0] == 1 && LeftSpace[2] == 9 && RightSpace[0] == 0) || (RightSpace[0] == 1 && RightSpace[2] == 9 && LeftSpace[0] == 0)))
                    return true;
            }
            catch { }
            try
            {
                if ((LeftSpace[0] == 1 && LeftSpace[1] == 1 && LeftSpace[2] == 0 && LeftSpace[3] == 1 && RightSpace[1] == 9) || (RightSpace[0] == 1 && RightSpace[1] == 1 && RightSpace[2] == 0 && RightSpace[3] == 1 && LeftSpace[1] == 9))
                    return true;
            }
            catch { }
            try
            {
                if ((LeftSpace[0] == 0 && LeftSpace[1] == 1 && LeftSpace[2] == 1 && LeftSpace[3] == 1 && LeftSpace[4] == 9) || (RightSpace[0] == 0 && RightSpace[1] == 1 && RightSpace[2] == 1 && RightSpace[3] == 1 && RightSpace[4] == 9))
                    return true;
            }
            catch { }
            try
            {
                if ((LeftSpace[0] == 1 && RightSpace[0] == 0 && RightSpace[1] == 1 && RightSpace[2] == 1) || (RightSpace[0] == 1 && LeftSpace[0] == 0 && LeftSpace[1] == 1 && LeftSpace[2] == 1))
                    return true;
            }
            catch { }
            try
            {
                if ((LeftSpace[0] == 1 && LeftSpace[1] == 0 && LeftSpace[2] == 1 && LeftSpace[3] == 1) || (RightSpace[0] == 1 && RightSpace[1] == 0 && RightSpace[2] == 1 && RightSpace[3] == 1))
                    return true;
            }
            catch { }
            return false;
        }

        private bool isFour()
        {
            try
            {
                if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && LeftSpace[2] == 1 && LeftSpace[3] == 0 && RightSpace[0] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && LeftSpace[2] == 0 && RightSpace[0] == 1 && RightSpace[1] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 1 && LeftSpace[1] == 0 && RightSpace[1] == 1 && RightSpace[0] == 1 && RightSpace[2] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 0 && RightSpace[2] == 1 && RightSpace[1] == 1 && RightSpace[0] == 1 && RightSpace[3] == 0)
                    return true;
            }
            catch { }
            return false;
        }

        private bool isFourBound()
        {
            try
            {
                if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && LeftSpace[2] == 1 && ((LeftSpace[3] == 9 && RightSpace[0] == 0) || (LeftSpace[3] == 0 && RightSpace[0] == 9)))
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && RightSpace[0] == 1 && ((LeftSpace[2] == 9 && RightSpace[1] == 0) || (LeftSpace[2] == 0 && RightSpace[1] == 9)))
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 1 && RightSpace[1] == 1 && RightSpace[0] == 1 && ((LeftSpace[1] == 9 && RightSpace[2] == 0) || (LeftSpace[1] == 0 && RightSpace[2] == 9)))
                    return true;
            }
            catch { }
            try
            {
                if (RightSpace[2] == 1 && RightSpace[1] == 1 && RightSpace[0] == 1 && ((LeftSpace[0] == 9 && RightSpace[3] == 0) || (LeftSpace[0] == 0 && RightSpace[3] == 9)))
                    return true;
            }
            catch { }
            return false;
        }

        private bool isThree()
        {
            if (freeLength == 2)
            {
                try
                {
                    if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && LeftSpace[2] == 0 && RightSpace[0] == 0)
                        return true;
                }
                catch { }
                try
                {
                    if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && RightSpace[0] == 1 && RightSpace[1] == 0)
                        return true;
                }
                catch { }
                try
                {
                    if (LeftSpace[0] == 0 && RightSpace[1] == 1 && RightSpace[0] == 1 && RightSpace[2] == 0)
                        return true;
                }
                catch { }
            }
            return false;
        }

        private bool isTwoOne()
        {
            try
            {
                if (LeftSpace[0] == 1 && RightSpace[0] == 0 && ((LeftSpace[1] == 0 && RightSpace[1] == 1 && RightSpace[2] == 0) || (LeftSpace[1] == 0 && LeftSpace[2] == 1 && LeftSpace[3] == 0)))
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 0 && RightSpace[0] == 1 && ((RightSpace[1] == 0 && LeftSpace[1] == 1 && LeftSpace[2] == 0) || (RightSpace[1] == 0 && RightSpace[2] == 1 && RightSpace[3] == 0)))
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 0 && RightSpace[0] == 0 && ((LeftSpace[1] == 1 && LeftSpace[2] == 1 && LeftSpace[3] == 0) || (RightSpace[1] == 1 && RightSpace[2] == 1 && RightSpace[3] == 0)))
                    return true;
            }
            catch { }
            return false;
        }

        private bool isThreeTrapped()
        {
            if (freeLength == 1)
            {
                try
                {
                    if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && LeftSpace[2] == 0 && RightSpace[0] == 0)
                        return true;
                }
                catch { }
                try
                {
                    if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && RightSpace[0] == 1 && RightSpace[1] == 0)
                        return true;
                }
                catch { }
                try
                {
                    if (LeftSpace[0] == 0 && RightSpace[1] == 1 && RightSpace[0] == 1 && RightSpace[2] == 0)
                        return true;
                }
                catch { }
            }
            return false;
        }

        private bool isThreeBound()
        {
            if (freeLength > 0)
            {
                try
                {
                    if (LeftSpace[0] == 1 && LeftSpace[1] == 1 && ((LeftSpace[2] == 9 && RightSpace[0] == 0) || (LeftSpace[2] == 0 && RightSpace[0] == 9)))
                        return true;
                }
                catch { }
                try
                {
                    if (LeftSpace[0] == 1 && RightSpace[0] == 1 && ((LeftSpace[1] == 9 && RightSpace[1] == 0) || (LeftSpace[1] == 0 && RightSpace[1] == 9)))
                        return true;
                }
                catch { }
                try
                {
                    if (RightSpace[1] == 1 && RightSpace[0] == 1 && ((LeftSpace[0] == 9 && RightSpace[2] == 0) || (LeftSpace[0] == 0 && RightSpace[2] == 9)))
                        return true;
                }
                catch { }

                try
                {
                    if (LeftSpace[0] == 1 && RightSpace[0] == 0 && RightSpace[1] == 1 && (LeftSpace[1] == 9 || RightSpace[2] == 9))
                        return true;
                }
                catch { }
                try
                {
                    if (LeftSpace[0] == 1 && LeftSpace[1] == 0 && LeftSpace[2] == 1 && (LeftSpace[3] == 9 || RightSpace[0] == 9))
                        return true;
                }
                catch { }
                try
                {
                    if (LeftSpace[0] == 0 && LeftSpace[1] == 1 && LeftSpace[2] == 1 && (LeftSpace[3] == 9 || RightSpace[0] == 9))
                        return true;
                }
                catch { }
                try
                {
                    if (RightSpace[0] == 1 && LeftSpace[0] == 0 && LeftSpace[1] == 1 && (LeftSpace[2] == 9 || RightSpace[1] == 9))
                        return true;
                }
                catch { }
                try
                {
                    if (RightSpace[0] == 1 && RightSpace[1] == 0 && RightSpace[2] == 1 && (LeftSpace[0] == 9 || RightSpace[3] == 9))
                        return true;
                }
                catch { }
                try
                {
                    if (RightSpace[0] == 0 && RightSpace[1] == 1 && RightSpace[2] == 1 && (LeftSpace[0] == 9 || RightSpace[3] == 9))
                        return true;
                }
                catch { }
            }
            return false;
        }

        private bool isTwo()
        {
            if (freeLength == 2)
            {
                try
                {
                    if (LeftSpace[0] == 1 && LeftSpace[1] == 0 && RightSpace[0] == 0)
                        return true;
                }
                catch { }
                try
                {
                    if (LeftSpace[0] == 0 && RightSpace[0] == 1 && RightSpace[1] == 0)
                        return true;
                }
                catch { }
            }
            return false;
        }

        private bool isOneOne()
        {
            if (freeLength == 2)
            {
                try
                {
                    if (LeftSpace[0] == 0 && RightSpace[0] == 0 && LeftSpace[1] == 1 && LeftSpace[2] == 0)
                        return true;
                }
                catch { }
                try
                {
                    if (LeftSpace[0] == 0 && RightSpace[0] == 0 && RightSpace[1] == 1 && RightSpace[2] == 0)
                        return true;
                }
                catch { }
            }
            return false;
        }


    }



    internal class cutInAction
    {
        public static int CA_ONEONE = 110;
        public static int CA_ONETWO = 5000;
        public static int CA_ONETWO_BOUND = 200;
        public static int CA_FOUR = 200000;
        int[] LeftSpace;
        int[] RightSpace;
        public cutInAction(int[] LeftSpace, int[] RightSpace)
        {
            this.LeftSpace = LeftSpace;
            this.RightSpace = RightSpace;
        }

        public int Calculate()
        {
            if (isFour())
                return CA_FOUR;
            else if (isOneTwoBound())
                return CA_ONETWO_BOUND;
            else if (isOneTwo())
                return CA_ONETWO;
            else if (isOneOne())
                return CA_ONEONE;
            return 0;
        }

        public bool isOneOne()
        {
            try
            {
                if (LeftSpace[1] == 0 && RightSpace[1] == 0 && (LeftSpace[2] == 0 || RightSpace[2] == 0))
                    return true;
            }
            catch { }
            return false;
        }

        public bool isOneTwo()
        {
            try
            {
                if ((LeftSpace[1] == 9 && RightSpace[1] == 0 && LeftSpace[2] == 0) || (LeftSpace[1] == 0 && RightSpace[1] == 9 && RightSpace[2] == 0))
                    return true;
            }
            catch { }
            return false;
        }

        public bool isOneTwoBound()
        {
            try
            {
                if ((LeftSpace[1] == 9 && RightSpace[1] == 0 && LeftSpace[2] == 1) || (LeftSpace[1] == 9 && RightSpace[1] == 1 && LeftSpace[2] == 0))
                    return true;
            }
            catch { }
            try
            {
                if ((LeftSpace[1] == 0 && RightSpace[1] == 9 && RightSpace[2] == 1) || (LeftSpace[1] == 1 && RightSpace[1] == 9 && RightSpace[2] == 0))
                    return true;
            }
            catch { }
            return false;
        }

        public bool isFour()
        {
            try
            {
                if (LeftSpace[1] == 9 && RightSpace[1] == 9)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[1] == 9 && LeftSpace[2] == 9)
                    return true;
            }
            catch { }
            try
            {
                if (RightSpace[1] == 9 && RightSpace[2] == 9)
                    return true;
            }
            catch { }
            return false;
        }
    }


    internal class blockingAction
    {
        public static int BA_ONE = 5;
        public static int BA_TWO = 100;
        public static int BA_ONEONE = 100;
        public static int BA_THREE = 5000;
        public static int BA_THREE_TRAPPED = 5100;
        public static int BA_THREE_BOUND = 200;
        public static int BA_ONETWO = 3500;
        public static int BA_FOUR = 200000;


        int[] LineSpace;

        public blockingAction(int[] LineSpace)
        {
            this.LineSpace = LineSpace;
        }

        public int Calculate()
        {
            if (isDangerous())
                return 0;
            else if (isFour())
                return BA_FOUR;
            else if (isThreeTrapped())
                return BA_THREE_TRAPPED;
            else if (isThreeBound())
                return BA_THREE_BOUND;
            else if (isThree())
                return BA_THREE;
            else if (isTwoOne())
                return BA_ONETWO;
            else if (isTwo())
                return BA_TWO;
            else if (isOneOne())
                return BA_ONEONE;
            else if (isOne())
                return BA_ONE;
            return 0;
        }

        private bool isOne()
        {
            try
            {
                if (LineSpace[1] == 0 && LineSpace[2] == 0 && LineSpace[3] == 0)
                    return true;
            }
            catch { }
            return false;
        }

        private bool isTwo()
        {
            try
            {
                if (LineSpace[0] == 9 && LineSpace[1] == 9 && LineSpace[2] == 0 && LineSpace[3] == 0)
                    return true;
            }
            catch { }
            return false;
        }


        private bool isOneOne()
        {
            try
            {
                if (LineSpace[1] == 0 && LineSpace[2] == 9 && LineSpace[3] == 0 && LineSpace[4] == 0)
                    return true;
            }
            catch { }
            return false;
        }

        private bool isThree()
        {
            try
            {
                if (LineSpace[1] == 9 && LineSpace[2] == 9 && LineSpace[3] == 0 && LineSpace[4] == 0)
                    return true;
            }
            catch { }
            return false;
        }

        private bool isThreeTrapped()
        {
            try
            {
                if (LineSpace[1] == 9 && LineSpace[2] == 9 && LineSpace[3] == 0 && LineSpace[4] == 1)
                    return true;
            }
            catch { }
            return false;
        }

        private bool isThreeBound()
        {
            try
            {
                if (LineSpace[1] == 9 && LineSpace[2] == 9 && LineSpace[3] == 1)
                    return true;
            }
            catch { }
            try
            {
                if (LineSpace[1] == 9 && LineSpace[2] == 0 && LineSpace[3] == 9 && LineSpace[4] == 1)
                    return true;
            }
            catch { }
            try
            {
                if (LineSpace[1] == 0 && LineSpace[2] == 9 && LineSpace[3] == 9 && LineSpace[4] == 1)
                    return true;
            }
            catch { }
            return false;
        }

        private bool isTwoOne()
        {
            try
            {
                if (LineSpace[1] == 9 && LineSpace[2] == 0 && LineSpace[3] == 9 && LineSpace[4] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (LineSpace[1] == 0 && LineSpace[2] == 9 && LineSpace[3] == 9 && LineSpace[4] == 0)
                    return true;
            }
            catch { }
            return false;
        }

        private bool isFour()
        {
            try
            {
                if (LineSpace[1] == 9 && LineSpace[2] == 9 && LineSpace[3] == 9)
                    return true;
            }
            catch { }
            return false;
        }

        private bool isDangerous()
        {
            try
            {
                if (LineSpace[1] == 0 && LineSpace[2] == 9 && LineSpace[3] == 9 && LineSpace[4] == 9)
                    return true;
            }
            catch { }
            try
            {
                if (LineSpace[1] == 9 && LineSpace[2] == 0 && LineSpace[3] == 9 && LineSpace[4] == 9)
                    return true;
            }
            catch { }
            try
            {
                if (LineSpace[1] == 9 && LineSpace[2] == 9 && LineSpace[3] == 0 && LineSpace[4] == 9)
                    return true;
            }
            catch { }
            return false;
        }

    }

    internal class counterAction
    {
        public static int CT_TOTHREE = 10;
        public static int CT_TOFOUR = 500;
        public static int CT_TOFOUR_BOUND = 70;
        public int[] LeftSpace;
        public int[] RightSpace;
        public counterAction(int[] LeftSpace, int[] RightSpace)
        {
            this.LeftSpace = LeftSpace;
            this.RightSpace = RightSpace;
        }

        public int Calculate()
        {
            if (isToFour())
                return CT_TOFOUR;
            else if (isToFourBound())
                return CT_TOFOUR_BOUND;
            else if (isToThree())
                return CT_TOTHREE;
            return 0;
        }

        public bool isToThree()
        {
            try
            {
                if (LeftSpace[0] == 0 && LeftSpace[1] == 9 && LeftSpace[2] == 0 && RightSpace[0] == 9 && RightSpace[2] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (RightSpace[0] == 0 && RightSpace[1] == 9 && RightSpace[2] == 0 && LeftSpace[0] == 9 && LeftSpace[2] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 0 && LeftSpace[1] == 9 && LeftSpace[2] == 9 && LeftSpace[3] == 0 && RightSpace[0] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (RightSpace[0] == 0 && RightSpace[1] == 9 && RightSpace[2] == 9 && RightSpace[3] == 0 && LeftSpace[0] == 0)
                    return true;
            }
            catch { }

            return false;
        }

        public bool isToFour()
        {
            try
            {
                if (LeftSpace[0] == 0 && LeftSpace[1] == 9 && LeftSpace[2] == 9 && LeftSpace[3] == 0 && RightSpace[0] == 9 && RightSpace[1] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (RightSpace[0] == 0 && RightSpace[1] == 9 && RightSpace[2] == 9 && RightSpace[3] == 0 && LeftSpace[0] == 9 && LeftSpace[1] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 0 && LeftSpace[1] == 9 && LeftSpace[2] == 0 && RightSpace[0] == 9 && RightSpace[1] == 9 && RightSpace[2] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (RightSpace[0] == 0 && RightSpace[1] == 9 && RightSpace[2] == 0 && LeftSpace[0] == 9 && LeftSpace[1] == 9 && LeftSpace[2] == 0)
                    return true;
            }
            catch { }
            return false;
        }

        public bool isToFourBound()
        {
            try
            {
                if (LeftSpace[0] == 0 && LeftSpace[1] == 9 && LeftSpace[2] == 9 && LeftSpace[3] == 1 && RightSpace[0] == 9 && RightSpace[1] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (RightSpace[0] == 0 && RightSpace[1] == 9 && RightSpace[2] == 9 && RightSpace[3] == 1 && LeftSpace[0] == 9 && LeftSpace[1] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 0 && LeftSpace[1] == 9 && LeftSpace[2] == 1 && RightSpace[0] == 9 && RightSpace[1] == 9 && RightSpace[2] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (RightSpace[0] == 0 && RightSpace[1] == 9 && RightSpace[2] == 1 && LeftSpace[0] == 9 && LeftSpace[1] == 9 && LeftSpace[2] == 0)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 0 && LeftSpace[1] == 9 && LeftSpace[2] == 9 && LeftSpace[3] == 0 && RightSpace[0] == 9 && RightSpace[1] == 1)
                    return true;
            }
            catch { }
            try
            {
                if (RightSpace[0] == 0 && RightSpace[1] == 9 && RightSpace[2] == 9 && RightSpace[3] == 0 && LeftSpace[0] == 9 && LeftSpace[1] == 1)
                    return true;
            }
            catch { }
            try
            {
                if (LeftSpace[0] == 0 && LeftSpace[1] == 9 && LeftSpace[2] == 0 && RightSpace[0] == 9 && RightSpace[1] == 9 && RightSpace[2] == 1)
                    return true;
            }
            catch { }
            try
            {
                if (RightSpace[0] == 0 && RightSpace[1] == 9 && RightSpace[2] == 0 && LeftSpace[0] == 9 && LeftSpace[1] == 9 && LeftSpace[2] == 1)
                    return true;
            }
            catch { }
            return false;
        }


    }

}