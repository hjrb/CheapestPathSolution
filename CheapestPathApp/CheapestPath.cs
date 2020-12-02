using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheapestPath
{
    /// <summary>
    /// this class computes the cheapest past from top to bottom in a grid
    /// every cell has a cost value
    /// you can move by going down a row and  either one column left, straight down or one column right, without leaving the grid :-)
    /// </summary>
    public static class CheapestPath
    {
        public record CellIndex(int Row, int Col);

        private static int MinThree(double[,] x, int row, int col)
        {
            var min = x[row, col]; var minIdx = col;
            if (col - 1 >= 0 && x[row, col - 1] < min) { minIdx = col - 1; min = x[row, col - 1]; }
            if (col + 1 < x.GetLength(1) && x[row, col + 1] < min)  minIdx = col + 1; 
            return minIdx;
        }

        /// <summary>
        /// computes the aggregation from the bottom up
        /// </summary>
        /// <param name="data">return the aggregated cost. The column with the lowest value in the first row is the starting point of the path</param>
        /// <returns></returns>
        private static double[,] ComputeAggregation(double[,] data)
        {
            // get rows and columns
            var rows = data.GetLength(0); var cols = data.GetLength(1);
            // crate output
            var x = new double[rows, cols];
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
        public static (double[,] matrix, List<CellIndex> path) ComputePath(double[,] data)
        {
            var x = ComputeAggregation(data);
            var path = new List<CellIndex>();
            int minIdx = 0;
            // find the minimum in the first row
            double minVal = x[0, 0];
            for (int col = 1; col < data.GetLength(1); ++col)
            {
                if (x[0, col] < minVal)
                {
                    minIdx = col;
                    minVal = x[0, col];
                }
            }
            // add to as first element in the path
            path.Add(new CellIndex(0, minIdx));
            // for the other rows
            for (int row = 1; row < data.GetLength(0); ++row)
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
