using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW3SecondTry
{
    class Program2
    {
        public static int N;
        public static Random gen = new Random();
        static void Mainnnn(string[] args)
        {
            N = int.Parse(Console.ReadLine());

            Board board = new Board();


            while (true)
            {
                if (board.sortedQueensGood.All( x => x.NumConflicts  == 0)) break;

                //Console.WriteLine(board.ToString());
                //Console.WriteLine();

                board.NextState();
            }

            //Print

            Console.WriteLine(board.ToString());
            Console.WriteLine("done");

        }
    }

    internal class Board
    {
        Queen[] queensAtCol;
        //public IPriorityQueue<Queen> SortedQueens { get; set; }
        //public SortedSet<Queen> sortedQueensGood;// = new SortedSet<Queen>(new QueenComparer<Queen>());
        public List<Queen> sortedQueensGood;
        //List<Queen>[] queensAtRow;//todo maybe change type with better
        List<Queen>[] queensAtMainDiagonal;//todo maybe change type with better
        List<Queen>[] queensAtSecondaryDiagonal;//todo maybe change type with better

        Random gen;

        internal Board()
        {
            queensAtCol = new Queen[Program2.N];//todo see if needed
            queensAtMainDiagonal = new List<Queen>[Program2.N * 2];
            for (int i = 0; i < Program2.N * 2 - 1; i++) queensAtMainDiagonal[i] = new List<Queen>(100);

            queensAtSecondaryDiagonal = new List<Queen>[Program2.N * 2];
            for (int i = 0; i < Program2.N * 2 - 1; i++) queensAtSecondaryDiagonal[i] = new List<Queen>(100);

            gen = new Random();
            List<int> indexes = new List<int>(Program2.N);//todo scramble this

            for (int index = 0; index < Program2.N; index++)
            {
                //int row = gen.Next(Program2.N);
                Queen queen = new Queen(index, index);
                queensAtCol[index] = queen;

                //fill collections
                queensAtMainDiagonal[queen.MainDiagonalIndex].Add(queen);
                queensAtSecondaryDiagonal[queen.SecondaryDiagonalIndex].Add(queen);
            }
            
            for (int index = 0; index < Program2.N * 2 - 1; index++)//calculate main diagonal conflicts
            {
                var queens = queensAtMainDiagonal[index];
                if (queens?.Count >= 2)
                {
                    var mainDiagonalConflicts = queens.Count - 1;
                    foreach (Queen queen in queens)
                    {
                        queen.NumConflicts += mainDiagonalConflicts;
                    }
                }
            }

            for (int index = 0; index < Program2.N * 2 - 1; index++)//calculate secondary diagonal conflicts
            {
                var queens = queensAtSecondaryDiagonal[index];
                if (queens?.Count >= 2)
                {
                    var secondaryDiagonalConflicts = queens.Count - 1;
                    foreach (Queen queen in queens)
                    {
                        queen.NumConflicts += secondaryDiagonalConflicts;
                    }
                }
            }

            //sortedQueensGood = new SortedSet<Queen>(queensAtCol);//, new QueenComparer<Queen>());//todo see if correct //sort the queens
            sortedQueensGood = new List<Queen>(queensAtCol);
            sortedQueensGood.Sort(new QueenComparer<Queen>());

        }

        internal void NextState()
        {

            Queen firstQueen = sortedQueensGood.Last();
            //sortedQueensGood.RemoveAt(Program2.N - 1);
            int secondIndex;
            do
            {
                secondIndex = gen.Next(0, Program2.N - 1);//todo see if ok
            }
            while (secondIndex == firstQueen.Row);

            Queen secondQueen = sortedQueensGood[secondIndex];
            //sortedQueensGood.RemoveAt(secondIndex);

            int firstRowIndex = firstQueen.Row;
            int secondRowIndex = secondQueen.Row;

            //remove queen from collections
            queensAtMainDiagonal[firstQueen.MainDiagonalIndex].Remove(firstQueen);
            queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex].Remove(firstQueen);

            queensAtMainDiagonal[secondQueen.MainDiagonalIndex].Remove(secondQueen);
            queensAtSecondaryDiagonal[secondQueen.SecondaryDiagonalIndex].Remove(secondQueen);

            //update conflicting queens
            queensAtMainDiagonal[firstQueen.MainDiagonalIndex].ForEach(q => q.NumConflicts--);
            queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex].ForEach(q => q.NumConflicts--);

            queensAtMainDiagonal[secondQueen.MainDiagonalIndex].ForEach(q => q.NumConflicts--);
            queensAtSecondaryDiagonal[secondQueen.SecondaryDiagonalIndex].ForEach(q => q.NumConflicts--);

            //preallocate queen
            firstQueen.Row = secondRowIndex;
            secondQueen.Row = firstRowIndex;

            //update conflicting queens
            queensAtMainDiagonal[firstQueen.MainDiagonalIndex].ForEach(q => q.NumConflicts++);
            queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex].ForEach(q => q.NumConflicts++);

            queensAtMainDiagonal[secondQueen.MainDiagonalIndex].ForEach(q => q.NumConflicts++);
            queensAtSecondaryDiagonal[secondQueen.SecondaryDiagonalIndex].ForEach(q => q.NumConflicts++);
            
            //add queen to collections
            queensAtMainDiagonal[firstQueen.MainDiagonalIndex].Add(firstQueen);
            queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex].Add(firstQueen);

            queensAtMainDiagonal[secondQueen.MainDiagonalIndex].Add(secondQueen);
            queensAtSecondaryDiagonal[secondQueen.SecondaryDiagonalIndex].Add(secondQueen);
            


            firstQueen.NumConflicts = queensAtMainDiagonal[firstQueen.MainDiagonalIndex].Count - 1;
            firstQueen.NumConflicts += queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex].Count - 1;

            secondQueen.NumConflicts = queensAtMainDiagonal[secondQueen.MainDiagonalIndex].Count - 1;
            secondQueen.NumConflicts += queensAtSecondaryDiagonal[secondQueen.SecondaryDiagonalIndex].Count - 1;

            //sortedQueensGood.Add(firstQueen);
            //sortedQueensGood.Add(secondQueen);
            //sortedQueensGood = new SortedSet<Queen>(sortedQueensGood.ToArray());
            //sortedQueensGood.Add(firstQueen);
            //sortedQueensGood.Add(secondQueen);

            //sortedQueensGood.Sort(new QueenComparer<Queen>());

        }

        public override string ToString()
        {
            char[][] retVal = new char[Program2.N][];
            for (int i = 0; i < Program2.N; i++)
            {
                retVal[i] = new string('.', Program2.N).ToCharArray();
            }

            foreach (var queen in queensAtCol)
            {
                retVal[queen.Row][queen.Col] = '*';
            }
            return string.Join(Environment.NewLine, retVal.Select(x => new string(x)));
        }
    }
    
    internal class QueenComparer<T> : IComparer<Queen>
    {
        //Random gen = new Random();
        public int Compare(Queen x, Queen y)
        {
            //if (x.NumConflicts == y.NumConflicts)
            //{
            //    return gen.Next(0, 2) * 2 - 1;
            //}
            //this is reversed
            return x.NumConflicts - y.NumConflicts;//todo see if good or needs random/ allow duplicates
        }
    }

    internal class Queen : IComparable
    {
        public int Row { get; set; }

        public int Col { get; set; }

        public int MainDiagonalIndex
        {
            get { return Program2.N - Row + Col - 1; }
        }

        public int SecondaryDiagonalIndex
        {
            get { return Row + Col; }
        }

        public int NumConflicts
        {
            //get { return ConflictingQueens.Count; }//todoto see if not needed
            get;
            set;
        }

        //public int RowConflicts { get; set; }//todo see if needed

        public List<Queen> ConflictingQueens//todo see if needed
        {
            get;
            private set;
        }

        internal Queen(int row, int col)
        {
            this.Row = row;
            this.Col = col;
            //this.ConflictingQueens = new List<Queen>();//todo see if needed
        }

        public int CompareTo(object obj)
        {
            if (this.NumConflicts == ((Queen)obj).NumConflicts)
            {
                return Program2.gen.Next(0, 2) * 2 - 1;
            }
            return this.NumConflicts - ((Queen)obj).NumConflicts;//todo see if good or needs random/ allow duplicates

        }
    }
}
