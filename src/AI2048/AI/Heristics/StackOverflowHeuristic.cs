namespace AI2048.AI.Heristics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.SearchTree;
    using AI2048.Game;

    public class StackOverflowHeuristic : IHeuristic
    {
        private static readonly double Log2 = Math.Log(2);

        public double Evaluate(Node node)
        {
            var result = 
                GetMonotonicity(node) * 1000 +
                GetMaxValueEvalution(node) * 1000 +
                GetEmptyCellEvalution(node) * 2700 +
                GetSmoothness(node) * 200;

            return result;
        }

        private static double GetEmptyCellEvalution(Node node) => Math.Log(node.EmptyCellCount);
        
        private static double GetMaxValueEvalution(Node node) => node.Grid.Flatten().Max() / Log2;

        private static double GetSmoothness(Node node)
        {
            double smoothness = 0;
            foreach (var cell in node.Grid.Where(c => c.Value != 0))
            {
                var value = cell.Value / Log2;

                foreach (var neighbor in GetNeighbors(node, cell))
                {
                    var neighborValue = neighbor.Value / Log2;
                    smoothness = smoothness - Math.Abs(value - neighborValue);
                }
            }

            return smoothness;
        }

        private static IEnumerable<LogarithmicGridCell> GetNeighbors(Node node, LogarithmicGridCell cell)
        {
            for (int x = cell.X + 1; x < 4; x++)
            {
                if (node.Grid[x, cell.Y] == 0)
                {
                    continue;
                }

                yield return new LogarithmicGridCell(node.Grid[x, cell.Y], x, cell.Y);
                break;
            }

            for (int y = cell.Y + 1; y < 4; y++)
            {
                if (node.Grid[cell.X, y] == 0)
                {
                    continue;
                }

                yield return new LogarithmicGridCell(node.Grid[cell.X, y], cell.X, y);
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

                    var currentValue = node.Grid[x, current] != 0 ? node.Grid[x, current] / Log2 : 0;
                    var nextValue = node.Grid[x, next] != 0 ? node.Grid[x, next] / Log2 : 0;

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

                    var currentValue = node.Grid[current, y] != 0 ? node.Grid[current, y] / Log2 : 0;
                    var nextValue = node.Grid[next, y] != 0 ? node.Grid[next, y] / Log2 : 0;

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