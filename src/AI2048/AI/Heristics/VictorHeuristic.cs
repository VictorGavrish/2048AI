namespace AI2048.AI.Heristics
{
    using System;
    using System.Runtime.CompilerServices;

    using AI2048.AI.SearchTree;
    using AI2048.Game;

    public class VictorHeuristic : IHeuristic<double>
    {
        private static readonly int[,] HeatMap =
        {
            { 0, 0, 1,  2 },
            { 0, 0, 1,  4 },
            { 1, 1, 1,  8 },
            { 2, 4, 8, 64 }
        };

        public double Evaluate(Node<double> node)
        {
            var grid = node.Grid;

            var result = EvaluateGrid(grid);

            for (var i = 0; i < 3; i++)
            {
                grid = grid.RotateCw();
                result = Math.Max(result, EvaluateGrid(grid));
            }

            result += node.EmptyCellCount;

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long EvaluateGrid(LogarithmicGrid grid)
        {
            long result = 0;

            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    result += TwoToThePower(grid[x, y]) * HeatMap[x, y];
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long TwoToThePower(int power) => 1 << power;
    }
}