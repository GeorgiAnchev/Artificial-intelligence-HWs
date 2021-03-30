using C5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HW2
{
    class Program
    {
        static void Main(string[] args)
        {

            //the input
            int n = int.Parse(Console.ReadLine());
            State.N = (int)Math.Sqrt(n + 1);//this solution only works for square matrix. The problem descriptions was kinda vague on this.

            int[][] matrix = new int[State.N][];
            Point coordinatesOf0 = new Point();

            for (int i = 0; i < State.N; i++)
            {
                var row = Console.ReadLine().Split(new char[] { ' ' }).Select(x => int.Parse(x));
                var index = row.ToList().IndexOf(0);
                if (index >= 0)
                {
                    coordinatesOf0.X = i;
                    coordinatesOf0.Y = index;
                }
                matrix[i] = row.ToArray();
            }
            DateTime start = DateTime.Now;

            State initial = new State(matrix, coordinatesOf0, 0, null);

            //check for solvability
            if (initial.GetManhattanDistance() % 2 == 1)
            {
                Console.WriteLine("Unsolvable input");
                return;
            }

            //calculate path
            State finalState = AStar(initial);

            //print answer
            Stack<string> finalPath = new Stack<string>();

            State child = finalState;
            State parent;
            while (child.Parent != null)
            {
                parent = child.Parent;
                finalPath.Push(getMovement(parent, child));
                child = parent;
            }
            
            Console.WriteLine(finalPath.Count);
            while(finalPath.Count > 0)
            {
                Console.WriteLine(finalPath.Pop());
            }
            Console.WriteLine((DateTime.Now - start).TotalSeconds);
        }

        private static string getMovement(State parent, State child)
        {
            if (child.PositionOf0.X > parent.PositionOf0.X)
            {
                return "up";
            }
            if (child.PositionOf0.X < parent.PositionOf0.X)
            {
                return "down";
            }
            if (child.PositionOf0.Y > parent.PositionOf0.Y)
            {
                return "left";
            }
                return "right";
        }

        static State AStar(State initial)

        {

            IComparer<State> comparer = new Comparer<State>();
            IPriorityQueue<State> candidates = new C5.IntervalHeap<State>(comparer);
            System.Collections.Generic.HashSet<string> visitedStates = new System.Collections.Generic.HashSet<string>();

            //initialize state
            candidates.Add(initial);
            visitedStates.Add(initial.ToString());

            while (candidates.Count > 0)
            {
                State currentBest = candidates.DeleteMin();

                if (currentBest.GetManhattanDistance() == 0)
                {
                    return currentBest;
                }
                
                // move right
                if (currentBest.PositionOf0.Y > 0)
                {
                    var newPositionof0 = new Point(currentBest.PositionOf0.X, currentBest.PositionOf0.Y - 1);
                    AddCandidate(currentBest, newPositionof0, visitedStates, candidates);
                }

                //move left
                if (currentBest.PositionOf0.Y < State.N - 1)
                {
                    var newPositionof0 = new Point(currentBest.PositionOf0.X, currentBest.PositionOf0.Y + 1);
                    AddCandidate(currentBest, newPositionof0, visitedStates, candidates);
                }

                //move down
                if (currentBest.PositionOf0.X > 0)
                {
                    Point newPositionof0 = new Point(currentBest.PositionOf0.X - 1, currentBest.PositionOf0.Y);
                    AddCandidate(currentBest, newPositionof0, visitedStates, candidates);
                }

                // move up
                if (currentBest.PositionOf0.X < State.N - 1)
                {
                    Point newPositionof0 = new Point(currentBest.PositionOf0.X + 1, currentBest.PositionOf0.Y);
                    AddCandidate(currentBest, newPositionof0, visitedStates, candidates);
                }
            }

            return null;//this should never be reached but the compilator requires it
        }

        static void AddCandidate(State currentBest, Point newPositionOf0, System.Collections.Generic.HashSet<string> visitedStates, IPriorityQueue<State> candidates)
        {
            State newState = Swap(currentBest, newPositionOf0);
            string newStateAsString = newState.ToString();
            if (!visitedStates.Contains(newStateAsString))
            {
                candidates.Add(newState);
                visitedStates.Add(newStateAsString);
            }
        }
        
        class Comparer<T> : IComparer<T>
        {
            public int Compare(T x, T y)
            {
                State a = x as State;
                State b = y as State;

                return (a.GetManhattanDistance() + a.MovesPerformed) - (b.GetManhattanDistance() + b.MovesPerformed);
            }
        }

        static State Swap(State currentState, Point newPositionof0)
        {
            State retValue = new State(new int[State.N][], newPositionof0, currentState.MovesPerformed + 1, currentState);
            for (int i = 0; i < State.N; i++)//clone the matrix
            {
                retValue[i] = new int[State.N];
                for (int j = 0; j < State.N; j++)
                {
                    retValue[i][j] = currentState[i][j];
                }
            }
            retValue[currentState.PositionOf0.X][currentState.PositionOf0.Y] = retValue[newPositionof0.X][newPositionof0.Y];
            retValue[newPositionof0.X][newPositionof0.Y] = 0;
            return retValue;
        }
    }
    
    public class State
    {
        public static int N;
        public State(int[][] matrix, Point positionOf0, int movesPerformed, State parent)
        {
            Matrix = matrix;
            PositionOf0 = positionOf0;
            MovesPerformed = movesPerformed;
            Parent = parent;
        }
        public int[][] Matrix;

        public Point PositionOf0;

        public int MovesPerformed;

        private int? manhattanDistance;

        public State Parent;

        private string asString;
        private string stringified;
        private int? asInt;

        public override string ToString()
        {
            if (asString == null)
            {
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < State.N; i++)
                {
                    for (int j = 0; j < State.N; j++)
                    {
                        result.Append(' ').Append(Matrix[i][j]);
                    }
                }
                asString =  result.ToString();
            }
            return asString;
        }

        internal int GetManhattanDistance()
        {
            if (manhattanDistance == null)//cache it so it is not recalculated
            {
                manhattanDistance = ManhattanDistance();
            }
            return (int)manhattanDistance;
        }

        public int[] this[int x]
        {
            get { return Matrix[x]; }

            set { Matrix[x] = value; }
        }

        private int ManhattanDistance()
        {
            int sum = 0;
            for (int i = 0; i < State.N; i++)
            {
                for (int j = 0; j < State.N; j++)
                {
                    int number = this[i][j];
                    if (number != 0)
                    {
                        number--;//because the tiles start with 1 instead of 0
                        int expectedRow = number / State.N;
                        int expectedCol = number % State.N;
                        sum += Math.Abs(i - expectedRow) + Math.Abs(j - expectedCol);
                    }
                    else
                    {
                        int expectedRow = State.N - 1;
                        int expectedCol = State.N - 1;
                        sum += Math.Abs(i - expectedRow) + Math.Abs(j - expectedCol);
                    }
                }
            }
            return sum;
        }

        internal string Stringify()
        {
            if (stringified == null)
            {
                StringBuilder result = new StringBuilder();
                for (int i = 0; i < State.N; i++)
                {
                    for (int j = 0; j < State.N; j++)
                    {
                        result.Append(' ').Append(Matrix[i][j] == 0 ? State.N * State.N : Matrix[i][j]);
                    }
                }
                stringified = result.ToString();
            }
            return stringified;
        }
    }
    
}