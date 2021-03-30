using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NQueens
{
    class Program
    {
        static void Main(string[] args)
        {
            int N = int.Parse(Console.ReadLine());

            Matrix matrix = new Matrix(N);

            while (true)
            {
                for (int i = 0; i < Matrix.N * 2; i++)//100 and 50 are also good
                {
                    matrix.Next();
                    if (matrix.TotalThreatedQueens == 0)
                    {
                        //print(matrix);
                        return;
                    }
                }
            }

        }

        internal class Matrix
        {
            internal static int N;
            Queen[] QueensOnColumn; //todo is this needed?
            int[] queensOnRow;
            int[] queensOnMainDiagonal;
            int[] queensOnSecondaryDiagonal;
            internal int TotalThreatedQueens;//todo is this needed?
            
            internal void Next()
            {
                int mostConflictsOnQueen = QueensOnColumn.Max(x => this.GetThreats(x));//optimize
                var mostThreatenedQueen = QueensOnColumn.First(x => this.GetThreats(x) == mostConflictsOnQueen);//get the random one not the first one

                int bestRowIndex = -1;
                int minThreat = N;//initialize with maximum 

                for (int rowIndex = 0; rowIndex < N; rowIndex++)//iterate rows and see if better position with less conflicts exists.   
                {
                    int mainDiagonalIndex = N - rowIndex + mostThreatenedQueen.Col - 1;
                    int secondaryDiagonalIndex = rowIndex + mostThreatenedQueen.Col;

                    int totalThreatsForRow = queensOnRow[rowIndex] + queensOnMainDiagonal[mainDiagonalIndex] + queensOnSecondaryDiagonal[secondaryDiagonalIndex];
                    if (rowIndex == mostThreatenedQueen.Row)
                    {
                        totalThreatsForRow -= 3;//delete the conflicts with itself
                    }

                    if (totalThreatsForRow < minThreat)
                    {
                        minThreat = totalThreatsForRow;//save the best
                        bestRowIndex = rowIndex;
                    }
                }

                if (bestRowIndex != -1)
                {
                    queensOnRow[mostThreatenedQueen.Row]--;
                    queensOnMainDiagonal[mostThreatenedQueen.MainDiagonalIndex]--;
                    queensOnSecondaryDiagonal[mostThreatenedQueen.SecondaryDiagonalIndex]--;

                    //place the queen to a new place
                    mostThreatenedQueen.Row = bestRowIndex;

                    queensOnRow[mostThreatenedQueen.Row]++;
                    queensOnMainDiagonal[mostThreatenedQueen.MainDiagonalIndex]++;//todo this is not ok, change formula with row + col
                    queensOnSecondaryDiagonal[mostThreatenedQueen.SecondaryDiagonalIndex]++;//todo this is not ok, change formula with row + col

                    if (mostThreatenedQueen.Threats == 0)
                    {
                        TotalThreatedQueens--;//one less queen to move
                    }
                }
            }

            private int GetThreats(Queen x)
            {
                int totalThreats  = queensOnRow[x.Row] + queensOnMainDiagonal[x.Row] + queensOnSecondaryDiagonal[x.Row] - 3;

                return totalThreats;
            }

            internal Matrix(int N)
            {
                queensOnRow = new int[N];
                queensOnMainDiagonal = new int[2 * N - 1];
                queensOnSecondaryDiagonal = new int[2 * N - 1];
                QueensOnColumn = new Queen[N];

                List<int> indexes = new List<int>(N);

                for (int i = 0; i < N; i++)
                {
                    //todo random row for every i, fill the arrays, optimize with one queen for a row, or something else.
                }


                for (int i = 0; i < N; i++)
                {
                    //todo random row for every i, fill the arrays, optimize with one queen for a row, or something else.
                }


                TotalThreatedQueens = N;//todo, if this is needed at all, change the number to the actual threated queens

            }


        }

    }
}