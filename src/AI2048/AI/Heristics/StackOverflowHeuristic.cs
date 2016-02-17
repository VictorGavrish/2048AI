namespace AI2048.AI.Heristics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using AI2048.AI.SearchTree;
    using AI2048.Game;

    public class StackOverflowHeuristic : IHeuristic
    {
        public double Evaluate(Node node)
        {
            var result = 
                GetMonotonicity(node) * 1.0 +
                GetMaxValueEvalution(node) * 1.0 +
                GetEmptyCellEvalution(node) * 2.7 +
                GetSmoothness(node) * 0.1;

            return result;
        }

        private static double GetEmptyCellEvalution(Node node) => Math.Log(node.EmptyCellCount);
        
        private static double GetMaxValueEvalution(Node node) => node.Grid.Flatten().Max();

        private static double GetSmoothness(Node node)
        {
            double smoothness = 0;

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    var value = node.Grid[x, y];

                    foreach (var neighbor in GetNeighbors(node.Grid, value, x, y))
                    {
                        var neighborValue = neighbor;
                        smoothness = smoothness - Math.Abs(value - neighborValue);
                    }
                }
            }

            return smoothness;
        }

        private static IEnumerable<byte> GetNeighbors(LogarithmicGrid grid, byte value, int cellX, int cellY)
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

        private static double GetMonotonicity(Node node)
        {
            double down = 0;
            double up = 0;
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

                    if (current > nextValue)
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

            double right = 0;
            double left = 0;
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