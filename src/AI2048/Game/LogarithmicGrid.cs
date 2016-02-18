﻿namespace AI2048.Game
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    public class LogarithmicGrid
    {
        private readonly byte[,] grid;

        private static readonly Random Random = new Random();

        public LogarithmicGrid(byte[,] grid)
        {
            this.grid = grid;
        }

        public LogarithmicGrid(int[,] grid)
        {
            this.grid = ToByteInLogSpace(grid);
        }

        public byte this[int x, int y] => this.grid[x, y];

        public byte[] Flatten()
        {
            var result = new byte[16];
            Buffer.BlockCopy(this.grid, 0, result, 0, 16);
            return result;
        }

        public LogarithmicGrid AddRandomTile()
        {
            var flat = this.Flatten();

            var emptyCells = flat.Count(c => c == 0);
            
            var value = Random.NextDouble() < 0.9 ? (byte)1 : (byte)2;
            var position = Random.Next(emptyCells);

            for (var i = 0; i < 16; i++)
            {
                if (flat[i] != 0)
                {
                    continue;
                }

                if (position == 0)
                {
                    flat[i] = value;
                    break;
                }

                position--;
            }

            var newGrid = new byte[4, 4];
            Buffer.BlockCopy(flat, 0, newGrid, 0, 16);

            return new LogarithmicGrid(newGrid);
        }

        public LogarithmicGrid MakeMove(Move move)
        {
            switch (move)
            {
                case Move.Left:
                    return this.MoveLeft();
                case Move.Right:
                    return this.MoveRight();
                case Move.Up:
                    return this.MoveUp();
                case Move.Down:
                    return this.MoveDown();
                default:
                    throw new ArgumentOutOfRangeException(nameof(move), move, null);
            }
        }

        public IEnumerable<LogarithmicGrid> NextPossibleWorldStates()
        {
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    if (this.grid[x, y] != 0)
                    {
                        continue;
                    }

                    var newStateWith2 = this.CloneMatrix();
                    newStateWith2[x, y] = 1;
                    yield return new LogarithmicGrid(newStateWith2);

                    var newStateWith4 = this.CloneMatrix();
                    newStateWith4[x, y] = 2;
                    yield return new LogarithmicGrid(newStateWith4);
                }
            }
        }

        public override bool Equals(object obj) =>
            //obj is LogarithmicGrid && 
            InnerEqual(this.grid, ((LogarithmicGrid)obj).grid);

        public bool Equals(LogarithmicGrid other) => InnerEqual(this.grid, other.grid);

        public override int GetHashCode() => unchecked(
            this.grid[0, 0] + this.grid[0, 1] * 2 + this.grid[0, 2] * 3 + this.grid[0, 3] * 5
            + this.grid[1, 0] * 7 + this.grid[1, 1] * 11 + this.grid[1, 2] * 13 + this.grid[1, 3] * 17
            + this.grid[2, 0] * 19 + this.grid[2, 1] * 23 + this.grid[2, 2] * 29 + this.grid[2, 3] * 31
            + this.grid[3, 0] * 37 + this.grid[3, 1] * 41 + this.grid[3, 2] * 43 + this.grid[3, 3] * 47);

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var x = 0; x < this.grid.GetLength(1); x++)
            {
                for (var y = 0; y < this.grid.GetLength(0); y++)
                {
                    var current = this.grid[x, y];

                    var currentHumnan = current == 0 ? 0 : Math.Pow(2, current);

                    sb.Append($"{currentHumnan, 5}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[,] first, byte[,] second, int count);

        private static bool InnerEqual(byte[,] first, byte[,] second)
        {
            //return first[0, 0] == second[0, 0] && first[0, 1] == second[0, 1] && first[0, 2] == second[0, 2]
            //       && first[0, 3] == second[0, 3] && first[1, 0] == second[1, 0] && first[1, 1] == second[1, 1]
            //       && first[1, 2] == second[1, 2] && first[1, 3] == second[1, 3] && first[2, 0] == second[2, 0]
            //       && first[2, 1] == second[2, 1] && first[2, 2] == second[2, 2] && first[2, 3] == second[2, 3]
            //       && first[3, 0] == second[3, 0] && first[3, 1] == second[3, 1] && first[3, 2] == second[3, 2]
            //       && first[3, 3] == second[3, 3];

            return memcmp(first, second, first.Length) == 0;
        }

        private static byte[,] ToByteInLogSpace(int[,] matrix)
        {
            var result = new byte[4, 4];

            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 4; x++)
                {
                    var current = matrix[x, y];

                    result[x, y] = current == 0 ? (byte)0 : (byte)Math.Log(current, 2);
                }
            }

            return result;
        }

        private byte[,] CloneMatrix()
        {
            var result = new byte[4, 4];

            Buffer.BlockCopy(this.grid, 0, result, 0, 16);

            return result;
        }

        private LogarithmicGrid MoveDown() => new LogarithmicGrid(this.MoveVertically(true));

        private LogarithmicGrid MoveLeft() => new LogarithmicGrid(this.MoveHorizontally(false));

        private LogarithmicGrid MoveRight() => new LogarithmicGrid(this.MoveHorizontally(true));

        private LogarithmicGrid MoveUp() => new LogarithmicGrid(this.MoveVertically(false));

        private byte[,] MoveHorizontally(bool left)
        {
            var result = new byte[4, 4];
            var startIndex = left ? 3 : 0;
            var endIndex = left ? -1 : 4;
            var increment = left ? -1 : 1;

            for (var x = 0; x < 4; x++)
            {
                byte last = 0;
                var lastIndex = startIndex;
                for (var y = startIndex; y != endIndex; y += increment)
                {
                    var current = this.grid[x, y];

                    if (current == 0)
                    {
                        continue;
                    }

                    if (last == 0)
                    {
                        last = current;
                        continue;
                    }

                    if (current == last)
                    {
                        result[x, lastIndex] = (byte)(last + 1);
                        last = 0;
                        lastIndex += increment;
                        continue;
                    }

                    result[x, lastIndex] = last;
                    last = current;
                    lastIndex += increment;
                }

                if (last != 0)
                {
                    result[x, lastIndex] = last;
                }
            }

            return result;
        }

        private byte[,] MoveVertically(bool down)
        {
            var result = new byte[4, 4];
            var startIndex = down ? 3 : 0;
            var endIndex = down ? -1 : 4;
            var increment = down ? -1 : 1;

            for (var y = 0; y < 4; y++)
            {
                byte last = 0;
                var lastIndex = startIndex;

                for (var x = startIndex; x != endIndex; x += increment)
                {
                    var current = this.grid[x, y];

                    if (current == 0)
                    {
                        continue;
                    }

                    if (last == 0)
                    {
                        last = current;
                        continue;
                    }

                    if (current == last)
                    {
                        result[lastIndex, y] = (byte)(last + 1);
                        last = 0;
                        lastIndex += increment;
                        continue;
                    }

                    result[lastIndex, y] = last;
                    last = current;
                    lastIndex += increment;
                }

                if (last != 0)
                {
                    result[lastIndex, y] = last;
                }
            }

            return result;
        }
    }
}