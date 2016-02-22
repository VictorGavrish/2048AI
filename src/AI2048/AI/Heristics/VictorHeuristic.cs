namespace AI2048.AI.Heristics
{
    using System;
    using System.Runtime.CompilerServices;

    using AI2048.AI.SearchTree;
    using AI2048.Game;

    public class VictorHeuristic : IHeuristic<double>
    {
        private static readonly long[,] HeatMap =
        {
            { 0, 0, 2, 5 },
            { 0, 1, 3, 8 },
            { 2, 3, 4, 16 },
            { 5, 8, 16, 128 }
        };
        
        public double Evaluate(MaximizingNode<double> node)
        {
            return this.Evaluate((Node<double>)node);
        }

        public double Evaluate(MinimizingNode<double> node)
        {
            return this.Evaluate((Node<double>)node);
        }

        //private static readonly long[,] HeatMap =
        //{
        //    { 0, 0, 1, 3 },
        //    { 0, 1, 2, 4 },
        //    { 2, 3, 4, 5 },
        //    { 5, 8, 16, 128 }
        //};

        private static long[,] HeatMapMirrored
        {
            get
            {
                var mirrored = new long[4, 4];

                for (var y = 0; y < 4; y++)
                {
                    for (var x = 0; x < 4; x++)
                    {
                        mirrored[y, x] = HeatMap[x, y];
                    }
                }

                return mirrored;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Evaluate(Node<double> node)
        {
            var grid = node.Grid;

            var result = long.MinValue;

            var i = 4;
            do
            {
                result = Math.Max(result, EvaluateGrid(grid, HeatMap));
                //result = Math.Max(result, EvaluateGrid(grid, HeatMapMirrored));
                grid = grid.RotateCw();
            }
            while (i-- > 0);

            result += node.EmptyCellCount;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long EvaluateGrid(LogarithmicGrid grid, long[,] heatMap)
        {
            long result = 0;

            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    result += TwoToThePower(grid[x, y] * 2) * heatMap[x, y];
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long TwoToThePower(int power) => (long)1 << power;
    }
}