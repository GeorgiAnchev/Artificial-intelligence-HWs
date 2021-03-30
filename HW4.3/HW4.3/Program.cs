using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW4._3
{
    class Program
    {
        static void Main(string[] args)
        {
            var whoIsStarting = int.Parse(Console.ReadLine());

            var input = Console.ReadLine().Split(new char[] { ' ' });
            bool isMaxOnTurn = whoIsStarting == 1 ? true : false;
            int inputRow = int.Parse(input[0]) - 1;
            int inputCol = int.Parse(input[1]) - 1;

            int[,] board = new int[3, 3];
            board[inputRow, inputCol] = isMaxOnTurn ? 1 : -1;

            var currentState = isMaxOnTurn ? FindMin(board, inputRow, inputCol, int.MinValue, int.MaxValue) : FindMax(board, inputRow, inputCol, int.MinValue, int.MaxValue);

            bool currentTurnIsMax = false;

            while (true)
            {
                //bottom
                if (currentState.childrenStates == null || currentState.childrenStates.Count == 0)
                {
                    Print(board);

                    if (currentState.Result == 0)
                    {
                        Console.WriteLine("Stalemate");
                    }
                    else if (currentState.Result == 1)
                    {
                        if (isMaxOnTurn) Console.WriteLine("Player wins");
                        else Console.WriteLine("Bot wins");
                    }
                    else
                    {
                        if (isMaxOnTurn) Console.WriteLine("Bot wins");
                        else Console.WriteLine("Player wins");
                    }
                    Console.ReadLine();
                    break;
                }

                if (currentTurnIsMax)
                {
                    Print(board);

                    input = Console.ReadLine().Split(new char[] { ' ' });
                    inputRow = int.Parse(input[0]) - 1;
                    inputCol = int.Parse(input[1]) - 1;

                    currentState = currentState.childrenStates.First(state => state.lastRow == inputRow && state.lastCol == inputCol);
                    board = currentState.Board;
                }
                else
                {
                    currentState = currentState.minsBestMove;
                    board = currentState.Board;
                }

                currentTurnIsMax = !currentTurnIsMax;
            }

        }

        private static void Print(int[,] board)
        {
            char[,] newBoard = new char[3, 3];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0) newBoard[i, j] = ' ';
                    if (board[i, j] == 1) newBoard[i, j] = 'X';
                    if (board[i, j] == -1) newBoard[i, j] = 'O';
                }
            }

            Console.WriteLine(newBoard[0, 0] + "|" + newBoard[0, 1] + "|" + newBoard[0, 2]);
            Console.WriteLine("-----");
            Console.WriteLine(newBoard[1, 0] + "|" + newBoard[1, 1] + "|" + newBoard[1, 2]);
            Console.WriteLine("-----");
            Console.WriteLine(newBoard[2, 0] + "|" + newBoard[2, 1] + "|" + newBoard[2, 2]);
            Console.WriteLine();
        }

        static State FindMax(int[,] board, int lastRow, int lastCol, int alpha, int beta)
        {
            var state = new State() { Board = board.Clone() as int[,], IsMaxOnTurn = true, lastCol = lastCol, lastRow = lastRow };

            bool maxWon = false;

            //row
            if (board[lastRow, 0] == board[lastRow, 1] && board[lastRow, 1] == board[lastRow, 2]) maxWon = true;

            //col
            if (board[0, lastCol] == board[1, lastCol] && board[1, lastCol] == board[2, lastCol]) maxWon = true;

            //mainDiag
            if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] && board[1, 1] != 0) maxWon = true;

            //reverseDiag
            if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0] && board[1, 1] != 0) maxWon = true;

            //bottom
            if (maxWon == true)
            {
                state.IsLeaf = true;
                state.Result = -1;
                state.minHeightToMinWin = 0;//impossible win for min
                state.minHeightToStalemate = int.MaxValue;//impossible stalemate for min
                return state;
            }

            int currentResult = int.MinValue;
            State childState;

            int currentAlpha = alpha;
            int currentBeta = beta;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0)
                    {
                        board[i, j] = 1;
                        childState = FindMin(board, i, j, currentAlpha, currentBeta);
                        state.childrenStates.Add(childState);
                        board[i, j] = 0;

                        if (childState.Result > currentResult)//todo add optimality checks
                        {
                            currentResult = (int)childState.Result;
                            if (currentResult > currentBeta)//pruning
                            {
                                state.IsLeaf = false;
                                state.Result = childState.Result;
                                state.minHeightToMinWin = childState.minHeightToMinWin + 1;
                                state.minHeightToStalemate = childState.minHeightToStalemate + 1;
                                state.minsBestMove = childState;

                                return state;
                            }
                            else
                            {
                                currentAlpha = Math.Max(currentAlpha, currentResult);
                            }
                        }
                    }
                }
            }

            if (state.childrenStates.Count == 0)//stalemate
            {
                state.IsLeaf = true;
                state.Result = 0;
                state.minHeightToMinWin = int.MaxValue;
                state.minHeightToStalemate = 0;
                return state;
            }

            var bestMoveForMax = state.childrenStates[0];
            foreach (var child in state.childrenStates)
            {//todo rework this
                if (child.Result > bestMoveForMax.Result)//todo maybe add more cases?
                {
                    bestMoveForMax = child;
                }
            }
            state.IsLeaf = false;
            state.Result = bestMoveForMax.Result;
            state.minHeightToMinWin = bestMoveForMax.minHeightToMinWin + 1;
            state.minHeightToStalemate = bestMoveForMax.minHeightToStalemate + 1;
            state.minsBestMove = bestMoveForMax;

            return state;
        }

        static State FindMin(int[,] board, int lastRow, int lastCol, int alpha, int beta)
        {
            var state = new State() { Board = board.Clone() as int[,], IsMaxOnTurn = false, lastCol = lastCol, lastRow = lastRow };

            bool minWon = false;

            //row
            if (board[lastRow, 0] == board[lastRow, 1] && board[lastRow, 1] == board[lastRow, 2]) minWon = true;

            //col
            if (board[0, lastCol] == board[1, lastCol] && board[1, lastCol] == board[2, lastCol]) minWon = true;

            //mainDiag
            if (board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2] && board[1, 1] != 0) minWon = true;

            //reverseDiag
            if (board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0] && board[1, 1] != 0) minWon = true;

            //bottom
            if (minWon == true)
            {
                state.IsLeaf = true;
                state.Result = 1;
                state.minHeightToMinWin = int.MaxValue;
                state.minHeightToStalemate = int.MaxValue;
                return state;
            }

            int currentResult = int.MaxValue;
            State childState;
            int currentAlpha = alpha;
            int currentBeta = beta;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == 0)
                    {
                        board[i, j] = -1;
                        childState = FindMax(board, i, j, currentAlpha, currentBeta);
                        state.childrenStates.Add(childState);
                        board[i, j] = 0;

                        if (childState.Result < currentResult)//todo add optimality checks
                        {
                            currentResult = (int)childState.Result;
                            if (currentResult < currentAlpha)//pruning
                            {
                                state.IsLeaf = false;
                                state.Result = childState.Result;
                                state.minHeightToMinWin = childState.minHeightToMinWin + 1;
                                state.minHeightToStalemate = childState.minHeightToStalemate + 1;
                                state.minsBestMove = childState;

                                return state;
                            }
                            else
                            {
                                currentBeta = Math.Min(currentBeta, currentResult);
                            }

                        }
                        
                    }
                }
            }

            if (state.childrenStates.Count == 0)//stalemate
            {
                state.IsLeaf = true;
                state.Result = 0;
                state.minHeightToMinWin = int.MaxValue;
                state.minHeightToStalemate = 0;
                return state;
            }

            var bestMove = state.childrenStates[0];
            foreach (var child in state.childrenStates)
            {//todo rework this if
                if (child.Result < bestMove.Result ||
                    (child.Result == bestMove.Result && child.Result == -1 && child.minHeightToMinWin < bestMove.minHeightToMinWin) ||
                    (child.Result == bestMove.Result && child.Result == 0 && child.minHeightToStalemate < bestMove.minHeightToStalemate))//todo add third case
                {
                    bestMove = child;
                }
            }
            state.IsLeaf = false;
            state.Result = bestMove.Result;
            state.minHeightToMinWin = bestMove.minHeightToMinWin + 1;
            state.minHeightToStalemate = bestMove.minHeightToStalemate + 1;
            state.minsBestMove = bestMove;

            return state;
        }
    }

    internal class State
    {
        public int[,] Board { get; set; }

        public bool IsMaxOnTurn { get; set; }

        public List<State> childrenStates { get; set; } = new List<State>();

        public bool IsLeaf { get; set; }

        public int? Result { get; set; }

        public int minHeightToMinWin { get; set; }

        public State minsBestMove { get; set; }

        public int minHeightToStalemate { get; internal set; }

        public int lastRow { get; set; }

        public int lastCol { get; set; }
    }
}
