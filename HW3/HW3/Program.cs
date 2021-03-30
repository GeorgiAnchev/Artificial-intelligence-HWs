using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW3
{
    class Program
    {
        public static int N;
        static void Mainnnnnn(string[] args)
        {
            N = int.Parse(Console.ReadLine());

            Board board = new Board();
            

            while (true)
            {
                if (board.sortedQueensGood.Max.NumConflicts == 0) return;

                //Console.WriteLine(board.ToString());
                //Console.WriteLine();

                board.NextState();
            }

            //Print
            
        }
    }

    internal class Board
    {
        Queen[] queensAtCol;
        //public IPriorityQueue<Queen> SortedQueens { get; set; }
        public SortedSet<Queen> sortedQueensGood;// = new SortedSet<Queen>(new QueenComparer<Queen>());

        List<Queen>[] queensAtRow;//todo maybe change type with better
        List<Queen>[] queensAtMainDiagonal;//todo maybe change type with better
        List<Queen>[] queensAtSecondaryDiagonal;//todo maybe change type with better

        Random gen;

        internal Board()
        {
            queensAtCol = new Queen[Program.N];//todo see if needed
            //SortedQueens = new IntervalHeap<Queen>(Program.N, new QueenComparer<Queen>());

            queensAtRow = new List<Queen>[Program.N];
            for (int i = 0; i < Program.N; i++) queensAtRow[i] = new List<Queen>(100);//todot see if this capacity is ok
            
            queensAtMainDiagonal = new List<Queen>[Program.N * 2];
            for (int i = 0; i < Program.N * 2 - 1; i++) queensAtMainDiagonal[i] = new List<Queen>(100);

            queensAtSecondaryDiagonal = new List<Queen>[Program.N * 2];
            for (int i = 0; i < Program.N * 2 - 1; i++) queensAtSecondaryDiagonal[i] = new List<Queen>(100);

            gen = new Random();

            for (int col = 0; col < Program.N; col++)
            {
                int row = gen.Next(Program.N);
                Queen queen = new Queen(row, col);
                queensAtCol[col] = queen;
                
                //fill collections
                queensAtRow[row].Add(queen);
                queensAtMainDiagonal[queen.MainDiagonalIndex].Add(queen);
                queensAtSecondaryDiagonal[queen.SecondaryDiagonalIndex].Add(queen);
            }

            for (int index = 0; index < Program.N; index++)//calculate row conflicts
            {
                var queens = queensAtRow[index];
                if (queens?.Count >= 2)
                {
                    var rowConflicts = queens.Count - 1;
                    foreach (Queen queen in queens)
                    {
                        queen.NumConflicts += rowConflicts;
                    }
                }
            }

            for (int index = 0; index < Program.N * 2 - 1; index++)//calculate main diagonal conflicts
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

            for (int index = 0; index < Program.N * 2 - 1; index++)//calculate secondary diagonal conflicts
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

            sortedQueensGood = new SortedSet<Queen>(queensAtCol, new QueenComparer<Queen>());//sort the queens
        }

        internal void NextState()
        {
            Queen firstQueen = sortedQueensGood.Max;
            
            var emptyRows = new List<int>();
            for (int i = 0; i < Program.N; i++)
            {
                if (queensAtRow[i].Count == 0)
                {
                    emptyRows.Add(i);
                }
            }
            int newRowIndex;
            if (emptyRows.Count > 0)
            {
                newRowIndex = emptyRows[gen.Next(0, emptyRows.Count())];
            }
            else
            {
                newRowIndex = gen.Next(Program.N);
            }

            //update conflicting queens
            queensAtRow[firstQueen.Row].ForEach(q => q.NumConflicts--);
            queensAtMainDiagonal[firstQueen.MainDiagonalIndex].ForEach(q => q.NumConflicts--);
            queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex].ForEach(q => q.NumConflicts--);

            //remove queen from collections
            queensAtRow[firstQueen.Row].Remove(firstQueen);
            queensAtMainDiagonal[firstQueen.MainDiagonalIndex].Remove(firstQueen);
            queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex].Remove(firstQueen);
            
            //preallocate queen
            firstQueen.Row = newRowIndex;

            //add queen to collections
            queensAtRow[firstQueen.Row].Add(firstQueen);
            queensAtMainDiagonal[firstQueen.MainDiagonalIndex].Add(firstQueen);
            queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex].Add(firstQueen);

            //update conflicting queens
            queensAtRow[firstQueen.Row].ForEach(q => q.NumConflicts++);
            queensAtMainDiagonal[firstQueen.MainDiagonalIndex].ForEach(q => q.NumConflicts++);
            queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex].ForEach(q => q.NumConflicts++);

            sortedQueensGood = new SortedSet<Queen>(sortedQueensGood, new QueenComparer<Queen>());//todo rework this

        }

        public override string ToString()
        {
            char[][] retVal = new char[Program.N][];
            for (int i = 0; i < Program.N; i++)
            {
                retVal[i] = new string('.', Program.N).ToCharArray();
            }

            foreach (var row in queensAtRow)
            {
                foreach (var queen in row)
                {
                    retVal[queen.Row][queen.Col] = '*';
                }
            }
            return string.Join(Environment.NewLine, retVal.Select(x=> new string(x)));
        }
    }

    

    internal class QueenComparer<T> : IComparer<Queen>
    {
        Random gen = new Random();
        public int Compare(Queen x, Queen y)
        {
            if (x.NumConflicts == y.NumConflicts)
            {
                return gen.Next(0, 2) * 2 - 1;
            }
            return x.NumConflicts - y.NumConflicts;//todo see if good or needs random/ allow duplicates
        }
    }

    internal class Queen 
    {
        public int Row { get; set; }

        public int Col { get; set; }

        public int MainDiagonalIndex
        {
            get { return Program.N - Row + Col - 1; }
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
        
        //public List<Queen> ConflictingQueens//todo see if needed
        //{
        //    get;
        //    private set;
        //}

        internal Queen(int row, int col)
        {
            this.Row = row;
            this.Col = col;
            //this.ConflictingQueens = new List<Queen>();//todo see if needed
        }
    }
}
