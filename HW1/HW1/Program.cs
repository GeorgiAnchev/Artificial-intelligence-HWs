using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW1
{
    class Program
    {
        static Stack<char[]> visualizedResult;
        static int N;
        static void Main(string[] args)
        {
            visualizedResult = new Stack<char[]>();
            
            N = int.Parse(Console.ReadLine());
            char[] state = (new string('>', N) + '_' + new string('<', N)).ToArray();

            char[] result = DFS(state, N);


            while(visualizedResult.Count > 0)
            {
                Console.WriteLine(visualizedResult.Pop());
            }
        }

        static char[] DFS(char[] state, int indexOfEmpty)
        {
            
            if (StateIsValid(state))
            {
                visualizedResult.Push(state);
                return state;
            }

            //1
            if (indexOfEmpty > 0 && state[indexOfEmpty - 1] == '>')
            {
                char[] newState = Swap(state, indexOfEmpty, indexOfEmpty - 1);
                char[] result = DFS(newState, indexOfEmpty - 1);
                if (result != null)
                {
                    visualizedResult.Push(state);
                    return result;
                }
            }

            //2
            if (indexOfEmpty > 1 && state[indexOfEmpty - 2] == '>')
            {
                char[] newState = Swap(state, indexOfEmpty, indexOfEmpty - 2);
                char[] result = DFS(newState, indexOfEmpty - 2);
                if (result != null)
                {
                    visualizedResult.Push(state);
                    return result;
                }
            }

            //3
            if (indexOfEmpty < N*2 && state[indexOfEmpty + 1] == '<')
            {
                char[] newState = Swap(state, indexOfEmpty, indexOfEmpty + 1);
                char[] result = DFS(newState, indexOfEmpty + 1);
                if (result != null)
                {
                    visualizedResult.Push(state);
                    return result;
                }
            }

            //4
            if (indexOfEmpty < N*2 - 1 && state[indexOfEmpty + 2] == '<')
            {
                char[] newState = Swap(state, indexOfEmpty, indexOfEmpty + 2);
                char[] result = DFS(newState, indexOfEmpty + 2);
                if (result != null)
                {
                    visualizedResult.Push(state);
                    return result;
                }
            }

            return null;
        }

        static char[] Swap(char[] state, int emptyIndex, int secondIndex)
        {
            char[] retValue = (char[])state.Clone();
            retValue[emptyIndex] = retValue[secondIndex];
            retValue[secondIndex] = '_';

            return retValue;
        }

        static bool StateIsValid(char[] state)
        {
            for (int i = 0; i < N; i++)
            {
                if (state[i] != '<' || state[2*N - i] != '>')
                {
                    return false;
                }
            }
            return true;
        }
    }
}
