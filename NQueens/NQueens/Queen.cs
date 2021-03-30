namespace NQueens
{
    internal class Queen
    {
        
        public int Row { get; set; }

        public int Col { get; set; }

        private int n;

        public int MainDiagonalIndex
        {
            get { return n - Row + Col - 1; }
        }

        public int SecondaryDiagonalIndex
        {
            get { return Row + Col; }
        }
        
        internal int Threats { get; set; }

        internal Queen(int row, int col, int N)
        {
            this.Row = row;
            this.Col = col;
            this.n = N;
        }
    }
    
}