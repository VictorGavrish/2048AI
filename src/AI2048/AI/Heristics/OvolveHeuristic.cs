namespace AI2048.AI.Heristics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using AI2048.AI.SearchTree;
    using AI2048.Game;

    public class OvolveHeuristic : IHeuristic
    {
        public double Evaluate(MaximizingNode node)
        {
            var result = GetMonotonicity(node) 
                         + GetMaxValueEvalution(node) 
                         + GetEmptyCellEvalution(node) * 2.7
                         + GetSmoothness(node) * 0.1;

            return result;
        }

        public double Evaluate(MinimizingNode node)
        {
            var result = GetMonotonicity(node) + GetEmptyCellEvalution(node);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double GetEmptyCellEvalution(Node node) => Math.Log(node.EmptyCellCount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte GetMaxValueEvalution(Node node) => node.Grid.Flatten().Max();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetSmoothness(Node node)
        {
            var smoothness = 0;

            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 4; x++)
                {
                    smoothness = GetNeighbors(node.Grid, x, y)
                        .Aggregate(smoothness, (current, neighbor) => current - Math.Abs(node.Grid[x, y] - neighbor));
                }
            }

            return smoothness;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<byte> GetNeighbors(LogarithmicGrid grid, int cellX, int cellY)
        {
            for (var x = cellX + 1; x < 4; x++)
            {
                if (grid[x, cellY] == 0)
                {
                    continue;
                }

                yield return grid[x, cellY];
                break;
            }

            for (var y = cellY + 1; y < 4; y++)
            {
                if (grid[cellX, y] == 0)
                {
                    continue;
                }

                yield return grid[cellX, y];
                break;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetMonotonicity(Node node)
        {
            var down = 0;
            var up = 0;
            for (var x = 0; x < 4; x++)
            {
                var current = 0;
                var next = current + 1;
                while (next < 4)
                {
                    while (next < 4 && node.Grid[x, next] == 0)
                    {
                        next++;
                    }

                    if (next >= 4)
                    {
                        next--;
                    }

                    var currentValue = node.Grid[x, current];
                    var nextValue = node.Grid[x, next];

                    if (currentValue > nextValue)
                    {
                        down += nextValue - currentValue;
                    }
                    else if (nextValue > currentValue)
                    {
                        up += currentValue - nextValue;
                    }

                    current = next;
                    next++;
                }
            }

            var right = 0;
            var left = 0;
            for (var y = 0; y < 4; y++)
            {
                var current = 0;
                var next = current + 1;

                while (next < 4)
                {
                    while (next < 4 && node.Grid[next, y] == 0)
                    {
                        next++;
                    }

                    if (next >= 4)
                    {
                        next--;
                    }

                    var currentValue = node.Grid[current, y];
                    var nextValue = node.Grid[next, y];

                    if (currentValue > nextValue)
                    {
                        right += nextValue - currentValue;
                    }
                    else if (nextValue > currentValue)
                    {
                        left += currentValue - nextValue;
                    }

                    current = next;
                    next++;
                }
            }

            return Math.Max(up, down) + Math.Max(left, right);
        }
    }
}