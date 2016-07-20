namespace AI2048.AI.Heristics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using AI2048.AI.SearchTree;
    using AI2048.Game;

    public static class Heuristics
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetEmptyCellCount(LogarithmicGrid grid)
        {
            var emptyCount = 0;

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    emptyCount += grid[x, y] == 0 ? 1 : 0;
                }
            }

            return emptyCount;
        }

        public static int GetAdjacentCellCount(LogarithmicGrid grid)
        {
            var adjacentCount = 0;

            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 3; x++)
                {
                    if (grid[x, y] != 0 && grid[x, y] == grid[x + 1, y])
                    {
                        adjacentCount++;
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
                        adjacentCount++;
                        y++;
                    }
                }
            }

            return adjacentCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetMaxValueEvalution(IPlayerNode node) => node.Grid.Flatten().Max();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSmoothness(LogarithmicGrid grid)
        {
            var smoothness = 0;
            
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 4; x++)
                {
                    foreach (var neighbor in GetNeighbors(grid, x, y))
                    {
                        smoothness = smoothness - Math.Abs(grid[x, y] - neighbor);
                    }
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
        public static int GetMonotonicity(LogarithmicGrid grid)
        {
            var down = 0;
            var up = 0;
            
            for (var x = 0; x < 4; x++)
            {
                var current = 0;
                var next = current + 1;
                while (next < 4)
                {
                    while (next < 4 && grid[x, next] == 0)
                    {
                        next++;
                    }

                    if (next >= 4)
                    {
                        next--;
                    }

                    var currentValue = grid[x, current];
                    var nextValue = grid[x, next];

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
                    while (next < 4 && grid[next, y] == 0)
                    {
                        next++;
                    }

                    if (next >= 4)
                    {
                        next--;
                    }

                    var currentValue = grid[current, y];
                    var nextValue = grid[next, y];

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