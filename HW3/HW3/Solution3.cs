using C5;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW3thirdTry
{
    class Program3
    {
        public static int N;
        public static Random gen = new Random();
        static void Main(string[] args)
        {
            N = int.Parse(Console.ReadLine());
            
            
            while (true)
            {
                gen = new Random();
                Board board = new Board();
                for (int i = 0; i < Program3.N * 2; i++)
                {

                    if (board.sortedQueensGood.All(x => x.Threats == 0))
                    {
                        Console.WriteLine(board.ToString());
                        return;
                    }

                    board.NextState();
                }

                //Console.WriteLine(board.ToString());
                //Console.WriteLine();

            }

            //Print

            //Console.WriteLine("done");

        }
    }

    internal class Board
    {
        Queen[] queensAtCol;

        public List<Queen> sortedQueensGood;

        Dictionary<int, Queen>[] queensAtMainDiagonal;//todo maybe change type with better
        Dictionary<int, Queen>[] queensAtSecondaryDiagonal;//todo maybe change type with better

        Random gen;

        internal Board()
        {
            queensAtCol = new Queen[Program3.N];//todo see if needed
            queensAtMainDiagonal = new Dictionary<int, Queen>[Program3.N * 2];
            for (int i = 0; i < Program3.N * 2 - 1; i++) queensAtMainDiagonal[i] = new Dictionary<int, Queen>(Program3.N * 2 - 1);

            queensAtSecondaryDiagonal = new Dictionary<int, Queen>[Program3.N * 2];
            for (int i = 0; i < Program3.N * 2 - 1; i++) queensAtSecondaryDiagonal[i] = new Dictionary<int, Queen>(Program3.N * 2 - 1);

            gen = new Random();
            List<int> indexes = new List<int>(Program3.N);//todo scramble this

            for (int index = 0; index < Program3.N; index++)
            {
                //int row = gen.Next(Program3.N);
                Queen queen = new Queen(index, index);//to 
                queensAtCol[index] = queen;

                //fill collections
                foreach(var neighbour in queensAtMainDiagonal[queen.MainDiagonalIndex])
                {
                    neighbour.Value.ConflictingQueens.Add(queen.Row, queen);
                }

                foreach (var neighbour in queensAtSecondaryDiagonal[queen.SecondaryDiagonalIndex])
                {
                    neighbour.Value.ConflictingQueens.Add(queen.Row, queen);
                }

                foreach (var neighbour in queensAtMainDiagonal[queen.MainDiagonalIndex])
                {
                    queen.ConflictingQueens.Add(neighbour.Key, neighbour.Value);
                }

                foreach (var neighbour in queensAtSecondaryDiagonal[queen.SecondaryDiagonalIndex])
                {
                    queen.ConflictingQueens.Add(neighbour.Key, neighbour.Value);
                }

                queensAtMainDiagonal[queen.MainDiagonalIndex].Add(queen.Row, queen);
                queensAtSecondaryDiagonal[queen.SecondaryDiagonalIndex].Add(queen.Row, queen);
            }
            
            sortedQueensGood = new List<Queen>(queensAtCol);
            //sortedQueensGood.Sort(new QueenComparer<Queen>());

        }

        internal void NextState()
        {
            int maxConflicts = sortedQueensGood.Last().Threats;
            int indexOfFirstMax = sortedQueensGood.FindIndex(x => x.Threats == maxConflicts);
            int firstIndex = gen.Next(indexOfFirstMax, Program3.N);
            Queen firstQueen = sortedQueensGood[firstIndex];
            int secondIndex;
            do
            {
                if (indexOfFirstMax < Program3.N - 1)
                {
                    secondIndex = gen.Next(indexOfFirstMax, Program3.N);
                }
                secondIndex = gen.Next(0, Program3.N - 1);
            }
            while (secondIndex == firstIndex);

            Queen secondQueen = sortedQueensGood[secondIndex];

            int firstRowIndex = firstQueen.Row;
            int secondRowIndex = secondQueen.Row;
            
            //remove queen from collections
            queensAtMainDiagonal[firstQueen.MainDiagonalIndex].Remove(firstQueen.Row);
            queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex].Remove(firstQueen.Row);

            queensAtMainDiagonal[secondQueen.MainDiagonalIndex].Remove(secondQueen.Row);
            queensAtSecondaryDiagonal[secondQueen.SecondaryDiagonalIndex].Remove(secondQueen.Row);

            //update old neighbour queens
            foreach (var oldNeighbour in queensAtMainDiagonal[firstQueen.MainDiagonalIndex])
            {
                oldNeighbour.Value.ConflictingQueens.Remove(firstQueen.Row);
            }
            foreach (var oldNeighbour in queensAtMainDiagonal[secondQueen.MainDiagonalIndex])
            {
                oldNeighbour.Value.ConflictingQueens.Remove(secondQueen.Row);
            }
            foreach (var oldNeighbour in queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex])
            {
                oldNeighbour.Value.ConflictingQueens.Remove(firstQueen.Row);
            }
            foreach (var oldNeighbour in queensAtSecondaryDiagonal[secondQueen.SecondaryDiagonalIndex])
            {
                oldNeighbour.Value.ConflictingQueens.Remove(secondQueen.Row);
            }
            
            //preallocate queen
            firstQueen.Row = secondRowIndex;
            secondQueen.Row = firstRowIndex;

            //update new neighbour queens
            foreach (var newNeighbours in queensAtMainDiagonal[firstQueen.MainDiagonalIndex])
            {
                newNeighbours.Value.ConflictingQueens.Add(firstQueen.Row, firstQueen);
            }
            foreach (var newNeighbours in queensAtMainDiagonal[secondQueen.MainDiagonalIndex])
            {
                newNeighbours.Value.ConflictingQueens.Add(secondQueen.Row, secondQueen);
            }
            foreach (var newNeighbours in queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex])
            {
                newNeighbours.Value.ConflictingQueens.Add(firstQueen.Row, firstQueen);
            }
            foreach (var newNeighbours in queensAtSecondaryDiagonal[secondQueen.SecondaryDiagonalIndex])
            {
                newNeighbours.Value.ConflictingQueens.Add(secondQueen.Row, secondQueen);
            }

            //update queens
            firstQueen.ConflictingQueens = new Dictionary<int, Queen>(Program3.N);
            secondQueen.ConflictingQueens = new Dictionary<int, Queen>(Program3.N);

            foreach (var neighbour in queensAtMainDiagonal[firstQueen.MainDiagonalIndex])
            {
                firstQueen.ConflictingQueens.Add(neighbour.Key, neighbour.Value);
            }
            foreach (var neighbour in queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex])
            {
                firstQueen.ConflictingQueens.Add(neighbour.Key, neighbour.Value);
            }
            foreach (var neighbour in queensAtMainDiagonal[secondQueen.MainDiagonalIndex])
            {
                secondQueen.ConflictingQueens.Add(neighbour.Key, neighbour.Value);
            }
            foreach (var neighbour in queensAtSecondaryDiagonal[secondQueen.SecondaryDiagonalIndex])
            {
                secondQueen.ConflictingQueens.Add(neighbour.Key, neighbour.Value);
            }

            //update queens conflicts to one another
            if (firstQueen.MainDiagonalIndex == secondQueen.MainDiagonalIndex || firstQueen.SecondaryDiagonalIndex == secondQueen.SecondaryDiagonalIndex)
            {
                firstQueen.ConflictingQueens.Add(secondQueen.Row, secondQueen);
                secondQueen.ConflictingQueens.Add(firstQueen.Row, firstQueen);
            }
            
            //add queens to collections
            queensAtMainDiagonal[firstQueen.MainDiagonalIndex].Add(firstQueen.Row, firstQueen);
            queensAtSecondaryDiagonal[firstQueen.SecondaryDiagonalIndex].Add(firstQueen.Row, firstQueen);

            queensAtMainDiagonal[secondQueen.MainDiagonalIndex].Add(secondQueen.Row, secondQueen);
            queensAtSecondaryDiagonal[secondQueen.SecondaryDiagonalIndex].Add(secondQueen.Row, secondQueen);

            //resort
            sortedQueensGood = InsertionSort(sortedQueensGood.ToArray()).ToList();//.Sort(new QueenComparer<Queen>());
        }

        static Queen[] InsertionSort(Queen[] inputArray)
        {
            for (int i = 0; i < Program3.N - 1; i++)
            {
                for (int j = i + 1; j > 0; j--)
                {
                    if (inputArray[j - 1].Threats > inputArray[j].Threats)
                    {
                        Queen temp = inputArray[j - 1];
                        inputArray[j - 1] = inputArray[j];
                        inputArray[j] = temp;
                    }
                }
            }
            return inputArray;
        }

        public override string ToString()
        {
            char[][] retVal = new char[Program3.N][];
            for (int i = 0; i < Program3.N; i++)
            {
                retVal[i] = new string('.', Program3.N).ToCharArray();
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
            return x.Threats - y.Threats;//todo see if good or needs random/ allow duplicates
        }
    }

    internal class Queen //: IComparable
    {
        public int Row { get; set; }

        public int Col { get; set; }

        public int MainDiagonalIndex
        {
            get { return Program3.N - Row + Col - 1; }
        }

        public int SecondaryDiagonalIndex
        {
            get { return Row + Col; }
        }

        //public int NumConflicts
        //{
        //    //get { return ConflictingQueens.Count; }//todoto see if not needed
        //    get;
        //    set;
        //}

        //public int RowConflicts { get; set; }//todo see if needed

        public Dictionary<int, Queen> ConflictingQueens = new Dictionary<int, Queen>(Program3.N);

        public int Threats { get { return ConflictingQueens.Count; } }

        internal Queen(int row, int col)
        {
            this.Row = row;
            this.Col = col;
            //this.ConflictingQueens = new List<Queen>();//todo see if needed
        }

        //public int CompareTo(object obj)
        //{
        //    if (this.Threats == ((Queen)obj).Threats)
        //    {
        //        return Program3.gen.Next(0, 2) * 2 - 1;
        //    }
        //    return this.Threats - ((Queen)obj).Threats;//todo see if good or needs random/ allow duplicates

        //}
    }

}
