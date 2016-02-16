namespace AI2048.Game
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;

    public class LogGridCell
    {
        public LogGridCell(byte value, int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.Value = value;
        }

        public int X { get; }

        public int Y { get; }

        public byte Value { get; }
    }

    public class LogGrid : IEnumerable<LogGridCell>
    {
        private readonly byte[,] grid;

        public LogGrid(byte[,] grid)
        {
            this.grid = grid;
        }

        public LogGrid(int[,] grid)
        {
            this.grid = ToByteInLogSpace(grid);
        }

        private static byte[,] ToByteInLogSpace(int[,] cloneMatrix)
        {
            var result = new byte[4,4];

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    var current = cloneMatrix[x, y];

                    result[x, y] = current == 0
                        ? (byte)0
                        : (byte)Math.Log(current, 2);
                }
            }

            return result;
        }

        public LogGrid MakeMove(Move move)
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

        public IEnumerable<LogGrid> NextPossibleWorldStates()
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
                    yield return new LogGrid(newStateWith2);

                    var newStateWith4 = this.CloneMatrix();
                    newStateWith4[x, y] = 2;
                    yield return new LogGrid(newStateWith4);
                }
            }
        }

        private LogGrid MoveLeft() => new LogGrid(this.MoveHorizontally(false));

        private LogGrid MoveRight() => new LogGrid(this.MoveHorizontally(true));

        private LogGrid MoveUp() => new LogGrid(this.MoveVertically(false));

        private LogGrid MoveDown() => new LogGrid(this.MoveVertically(true));

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

        public int EmptyCellCount => this.Flatten().Count(i => i == 0);

        public byte this[int x, int y] => this.grid[x, y];

        private byte[,] CloneMatrix() => (byte[,])this.grid.Clone();

        public IEnumerable<byte> Flatten() => this.grid.Cast<byte>();

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

        public override bool Equals(object obj)
        {
            return SequenceEqual(this.grid, (obj as LogGrid)?.grid);
        }

        public bool Equals(LogGrid other)
        {
            return SequenceEqual(this.grid, other.grid);
        }

        private static bool SequenceEqual(byte[,] first, byte[,] second)
        {
            return memcmp(first, second, Marshal.SizeOf(typeof(byte)) * first.Length) == 0;
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[,] first, byte[,] second, int count);

        public override int GetHashCode()
        {
            return
                unchecked(
                    this.grid[0, 0] + this.grid[0, 1] * 2 + this.grid[0, 2] * 3 + this.grid[0, 3] * 5
                    + this.grid[1, 0] * 7 + this.grid[1, 1] * 11 + this.grid[1, 2] * 13 + this.grid[1, 3] * 17
                    + this.grid[2, 0] * 19 + this.grid[2, 1] * 23 + this.grid[2, 2] * 29 + this.grid[2, 3] * 31
                    + this.grid[3, 0] * 37 + this.grid[3, 1] * 41 + this.grid[3, 2] * 43 + this.grid[3, 3] * 47);
        }

        public IEnumerator<LogGridCell> GetEnumerator()
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    yield return new LogGridCell(this.grid[x, y], x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}