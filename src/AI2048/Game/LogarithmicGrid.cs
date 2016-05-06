namespace AI2048.Game
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class LogarithmicGrid : IEquatable<LogarithmicGrid>
    {
        private readonly byte[,] grid;

        private static readonly Random Random = new Random();

        public LogarithmicGrid(byte[,] grid)
        {
            this.grid = grid;
        }

        public LogarithmicGrid(int[,] grid)
        {
            var result = new byte[4, 4];

            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 4; x++)
                {
                    var current = grid[x, y];

                    result[x, y] = current == 0 ? (byte)0 : (byte)Math.Log(current, 2);
                }
            }

            this.grid = result;
        }

        public byte this[int x, int y] => this.grid[x, y];

        public byte[] Flatten()
        {
            var result = new byte[16];
            Buffer.BlockCopy(this.grid, 0, result, 0, 16);
            return result;
        }

        public int Sum()
        {
            var sum = 0;
            for (var y = 0; y < 4; y++)
            {
                for (var x = 0; x < 4; x++)
                {
                    var value = this.grid[x, y];
                    sum += value == 0 ? 0 : 2 << (value - 1);
                }
            }

            return sum;
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

        public IEnumerable<LogarithmicGrid> NextPossibleStatesWith2()
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
                }
            }
        }

        public IEnumerable<LogarithmicGrid> NextPossibleStatesWith4()
        {
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    if (this.grid[x, y] != 0)
                    {
                        continue;
                    }

                    var newStateWith4 = this.CloneMatrix();
                    newStateWith4[x, y] = 2;
                    yield return new LogarithmicGrid(newStateWith4);
                }
            }
        }

        public IEnumerable<LogarithmicGrid> NextPossibleStates()
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var x = 0; x < this.grid.GetLength(1); x++)
            {
                for (var y = 0; y < this.grid.GetLength(0); y++)
                {
                    var current = this.grid[x, y];

                    var currentHumnan = current == 0 ? 0 : Math.Pow(2, current);

                    sb.Append($"{currentHumnan,5}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static LogarithmicGrid Parse(string stringGrid)
        {
            var grid = new int[4, 4];

            var values =
                stringGrid.Split(new[] {'\n', '\r', '\t', ' '}, StringSplitOptions.RemoveEmptyEntries)
                          .Select(v => v.Trim())
                          .Select(int.Parse)
                          .ToArray();

            if (values.Length != 16)
            {
                throw new ArgumentException(nameof(stringGrid));
            }

            Buffer.BlockCopy(values, 0, grid, 0, 16*sizeof(int));

            return new LogarithmicGrid(grid);
        }

        public override bool Equals(object obj) => UnsafeCompare(this.grid, ((LogarithmicGrid)obj).grid);

        public bool Equals(LogarithmicGrid other) => UnsafeCompare(this.grid, other.grid);

        public bool UnsafeEquals(LogarithmicGrid other) => UnsafeCompare(this.grid, other.grid);

        public override unsafe int GetHashCode()
        {
            fixed (byte* buffer = this.grid)
            {
                long* x = (long*)buffer, y = (long*)(buffer + 8);

                return (*x).GetHashCode()*31 + (*y).GetHashCode()*23;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe bool UnsafeCompare(byte[,] first, byte[,] second)
        {
            fixed (byte* p1 = first, p2 = second)
            {
                byte* x1 = p1, x2 = p2, y1 = p1 + 8, y2 = p2 + 8;

                return *(long*)x1 == *(long*)x2 && *(long*)y1 == *(long*)y2;
            }
        }

        private byte[,] CloneMatrix()
        {
            var result = new byte[4, 4];

            Buffer.BlockCopy(this.grid, 0, result, 0, 16);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LogarithmicGrid MoveDown()
        {
            var result = new byte[4, 4];

            for (var y = 0; y < 4; y++)
            {
                byte last = 0;
                var lastIndex = 3;

                for (var x = 3; x >= 0; x--)
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
                        lastIndex += -1;
                        continue;
                    }

                    result[lastIndex, y] = last;
                    last = current;
                    lastIndex += -1;
                }

                if (last != 0)
                {
                    result[lastIndex, y] = last;
                }
            }

            return new LogarithmicGrid(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LogarithmicGrid MoveLeft()
        {
            var result = new byte[4, 4];

            for (var x = 0; x < 4; x++)
            {
                byte last = 0;
                var lastIndex = 0;
                for (var y = 0; y < 4; y++)
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
                        lastIndex += 1;
                        continue;
                    }

                    result[x, lastIndex] = last;
                    last = current;
                    lastIndex += 1;
                }

                if (last != 0)
                {
                    result[x, lastIndex] = last;
                }
            }

            return new LogarithmicGrid(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LogarithmicGrid MoveRight()
        {
            var result = new byte[4, 4];

            for (var x = 0; x < 4; x++)
            {
                byte last = 0;
                var lastIndex = 3;
                for (var y = 3; y >= 0; y--)
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
                        lastIndex += -1;
                        continue;
                    }

                    result[x, lastIndex] = last;
                    last = current;
                    lastIndex += -1;
                }

                if (last != 0)
                {
                    result[x, lastIndex] = last;
                }
            }

            return new LogarithmicGrid(result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private LogarithmicGrid MoveUp()
        {
            var result = new byte[4, 4];

            for (var y = 0; y < 4; y++)
            {
                byte last = 0;
                var lastIndex = 0;

                for (var x = 0; x < 4; x++)
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
                        lastIndex += 1;
                        continue;
                    }

                    result[lastIndex, y] = last;
                    last = current;
                    lastIndex += 1;
                }

                if (last != 0)
                {
                    result[lastIndex, y] = last;
                }
            }

            return new LogarithmicGrid(result);
        }
    }
}