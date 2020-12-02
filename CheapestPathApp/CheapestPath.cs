using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheapestPath
{

    public class RowArray2D<T>
    {
        readonly T[] data;
        public int Rows { get; }
        public int Cols { get; }

        public RowArray2D(int rows, int cols) {
            this.Rows=rows; 
            this.Cols=cols;
            data=new T[rows * cols];
        }

        public RowArray2D(T[,] input)
        {
            this.Rows = input.GetLength(0);
            this.Cols = input.GetLength(1);
            this.data = new T[this.Rows * this.Cols];
            for (int row = 0; row < this.Rows; ++row) for (int col = 0; col < this.Cols; ++col) this.data[(row * this.Cols) + col] = input[row, col];

        }

        public T this[int row, int col]
        {
            get => data[(row * Cols) + col];
            set => data[(row * Cols) + col] = value;
        }

        public Span<T> this[int row]
        {
            get => data.AsSpan<T>(row * Cols, Cols);
        }
    }

    /// <summary>
    /// this class computes the cheapest past from top to bottom in a grid
    /// every cell has a cost value
    /// you can move by going down a row and  either one column left, straight down or one column right, without leaving the grid :-)
    /// </summary>
    public static class CheapestPath
    {
        public record CellIndex(int Row, int Col);

        private static int MinThree(RowArray2D<double> x, int row, int col)
        {
            var min = x[row, col]; 
            var minIdx = col;
            if (col - 1 >= 0 && x[row, col - 1] < min) { 
                minIdx = col - 1; 
                min = x[row, col - 1]; 
            }
            if (col + 1 < x.Cols && x[row, col + 1] < min)
            {
                minIdx = col + 1;
            }
            return minIdx;
        }

        /// <summary>
        /// computes the aggregation from the bottom up
        /// </summary>
        /// <param name="data">return the aggregated cost. The column with the lowest value in the first row is the starting point of the path</param>
        /// <returns></returns>
        private static RowArray2D<double> ComputeAggregation(RowArray2D<double> data)
        {
            // get rows and columns
            var rows = data.Rows; var cols = data.Cols;
            // crate output
            var x = new RowArray2D<double>(rows, cols);
            // copy the lowest row, as the cost in the lowest row is simply the cost of the original data
            for (int col = 0; col < cols; ++col)
            {
                x[rows - 1, col] = data[rows - 1, col];
            }
            // now process the rows above
            for (int row = rows - 2; row >= 0; --row)
            {
                // simply look in the three possible cells in the row below for the lowest value - that is what the cost will be
                for (int col = 0; col < cols; ++col)
                {
                    var minIdex = MinThree(x, row, col);
                    x[row, col] = data[row, col] + x[row + 1, minIdex];
                }
            }
            return x;
        }

        /// <summary>
        /// computes the path
        /// </summary>
        /// <param name="data">input data</param>
        /// <returns>a tuple of the aggregate matrix and a list of the cells of the cheapest path</returns>
        public static (RowArray2D<double> aggregated, List<CellIndex> path) ComputePath(RowArray2D<double> data)
        {
            var x = ComputeAggregation(data);
            var path = new List<CellIndex>();
            // find the minimum in the first row
            int minIdx = Enumerable.Range(0, x.Cols).Aggregate((a, b) => (x[0,a] < x[0,b]) ? a : b); 
            // add to as first element in the path
            path.Add(new CellIndex(0, minIdx));
            // for the other rows
            for (int row = 1; row < data.Rows; ++row)
            {
                // find the minimum 
                var newMinIdx = MinThree(x, row, minIdx);
                path.Add(new CellIndex(row, newMinIdx));
                minIdx = newMinIdx;
            }
            return (x, path);
        }
    }
}
