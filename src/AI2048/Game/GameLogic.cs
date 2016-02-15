namespace AI2048.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class GameLogic
    {
        public static IEnumerable<Grid> MakeMovePossibleStates(Grid grid, Move move)
        {
            var newState = MakeMove(grid, move);

            return NextPossibleWorldStates(newState);
        }

        public static Grid MakeMove(Grid grid, Move move)
        {
            var newGrid = new int[4, 4];
            switch (move)
            {
                case Move.Left:
                    for (var y = 0; y < 4; y++)
                    {
                        var nonZeroes = Merge(grid.GetRow(y).Where(n => n != 0).ToArray());
                        var l = nonZeroes.Length;
                        for (var i = 0; i < l; i++)
                        {
                            newGrid[i, y] = nonZeroes[i];
                        }
                    }

                    break;
                case Move.Right:
                    for (var y = 0; y < 4; y++)
                    {
                        var nonZeroes =
                            Merge(grid.GetRow(y).Where(n => n != 0).Reverse().ToArray()).Reverse().ToArray();
                        var l = nonZeroes.Length;
                        for (var i = 0; i < l; i++)
                        {
                            newGrid[i + (4 - l), y] = nonZeroes[i];
                        }
                    }

                    break;
                case Move.Up:
                    for (var x = 0; x < 4; x++)
                    {
                        var nonZeroes = Merge(grid.GetColumn(x).Where(n => n != 0).ToArray());
                        var l = nonZeroes.Length;
                        for (var i = 0; i < l; i++)
                        {
                            newGrid[x, i] = nonZeroes[i];
                        }
                    }

                    break;
                case Move.Down:
                    for (var x = 0; x < 4; x++)
                    {
                        var nonZeroes =
                            Merge(grid.GetColumn(x).Where(n => n != 0).ToArray().Reverse().ToArray())
                                .Reverse()
                                .ToArray();
                        var l = nonZeroes.Length;
                        for (var i = 0; i < l; i++)
                        {
                            newGrid[x, i + (4 - l)] = nonZeroes[i];
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(move));
            }

            return new Grid(newGrid);
        }

        private static int[] Merge(int[] line)
        {
            if (line.Length <= 1)
            {
                return line;
            }

            if (line.Length == 2)
            {
                if (line[0] == line[1])
                {
                    return new[] { line[0] * 2 };
                }

                return line;
            }

            if (line.Length == 3)
            {
                if (line[0] == line[1])
                {
                    return new[] { line[0] * 2, line[2] };
                }

                if (line[1] == line[2])
                {
                    return new[] { line[0], line[1] * 2 };
                }

                return line;
            }

            if (line.Length == 4)
            {
                if (line[0] == line[1])
                {
                    if (line[2] == line[3])
                    {
                        return new[] { line[0] * 2, line[2] * 2 };
                    }

                    return new[] { line[0] * 2, line[2], line[3] };
                }

                if (line[1] == line[2])
                {
                    return new[] { line[0], line[1] * 2, line[3] };
                }

                if (line[2] == line[3])
                {
                    return new[] { line[0], line[1], line[2] * 2 };
                }

                return line;
            }

            return line;
        }

        /// <summary>
        /// Rotates the grid clockwise
        /// </summary>
        public static Grid RotateCw(Grid grid)
        {
            var newGrid = new int[4, 4];
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    newGrid[3 - y, x] = grid[x, y];
                }
            }

            return new Grid(newGrid);
        }

        public static IEnumerable<Grid> NextPossibleWorldStates(Grid state)
        {
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    if (state[x, y] != 0)
                    {
                        continue;
                    }

                    var newStateWith2 = state.CloneMatrix();
                    newStateWith2[x, y] = 2;
                    yield return new Grid(newStateWith2);

                    var newStateWith4 = state.CloneMatrix();
                    newStateWith4[x, y] = 4;
                    yield return new Grid(newStateWith4);
                }
            }
        }
    }
}