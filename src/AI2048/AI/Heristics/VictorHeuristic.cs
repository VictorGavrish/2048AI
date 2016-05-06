namespace AI2048.AI.Heristics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using AI2048.AI.SearchTree;
    using AI2048.Game;

    public class VictorHeuristic : IHeuristic
    {
        private readonly long[,] heatMap =
        {
            { 0, 0, 1, 3 },
            { 0, 1, 2, 4 },
            { 2, 3, 4, 6 },
            { 5, 8, 16, 128 }
        };
        private readonly long[][,] heatMaps;

        public VictorHeuristic()
        {
            var maps = new List<long[,]> { this.heatMap, Mirror(this.heatMap) };

            var map = this.heatMap;

            for (var i = 0; i < 3; i++)
            {
                map = RotateCw(map);

                maps.Add(map);

                maps.Add(Mirror(map));
            }

            this.heatMaps = maps.ToArray();
        }

        private static long[,] RotateCw(long[,] matrix)
        {
            var newMatrix = new long[4, 4];

            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 4; x++)
                {
                    newMatrix[3 - y, x] = matrix[x, y];
                }
            }

            return newMatrix;
        }

        private static long[,] Mirror(long[,] matrix)
        {
            var mirrored = new long[4, 4];

            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 4; x++)
                {
                    mirrored[y, x] = matrix[x, y];
                }
            }

            return mirrored;
        }

        public double Evaluate(PlayerNode node)
        {
            return this.Evaluate((Node)node);
        }

        public double Evaluate(ComputerNode node)
        {
            return this.Evaluate((Node)node);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Evaluate(Node node)
        {
            var grid = node.Grid;

            var result = this.heatMaps.Select(heatMap => EvaluateGrid(grid, heatMap)).Max();

            result -= 1 << Math.Max(0, 6 - EvaluateEmptyCells(node.Grid));

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
        private static int EvaluateEmptyCells(LogarithmicGrid grid)
        {
            var ajacentCount = 0;

            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 3; x++)
                {
                    if (grid[x, y] != 0 && grid[x, y] == grid[x + 1, y])
                    {
                        ajacentCount++;
                        x++;
                    }
                }
            }

            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 3; y++)
                {
                    if (grid[x, y] != 0 && grid[x, y] == grid[x, y + 1])
                    {
                        ajacentCount++;
                        y++;
                    }
                }
            }

            var emptyCount = grid.Flatten().Count(b => b == 0);

            return ajacentCount + emptyCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long TwoToThePower(int power) => (long)1 << power;
    }
}