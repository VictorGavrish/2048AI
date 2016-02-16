namespace AI2048.Game
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Grid : IEnumerable<GridCell>
    {
        private readonly int[,] grid;

        public Grid(int[,] grid)
        {
            this.grid = grid;
        }

        public Grid(string[] rows)
        {
            var grid = new int[4, 4];
            for (var y = 0; y < rows.Length; y++)
            {
                var vals = rows[y].Split(' ').Select(int.Parse).ToArray();
                for (var x = 0; x < vals.Length; x++)
                {
                    grid[x, y] = vals[x];
                }
            }

            this.grid = grid;
        }

        public int EmptyCellsNo => this.grid.Cast<int>().Count(i => i == 0);

        public int this[int x, int y] => this.grid[x, y];

        public int[,] CloneMatrix()
        {
            return (int[,])this.grid.Clone();
        }

        public int[] Flatten()
        {
            return this.grid.Cast<int>().ToArray();
        }

        public int[] GetColumn(int x)
        {
            var res = new int[4];
            for (var i = 0; i < 4; i++)
            {
                res[i] = this.grid[x, i];
            }

            return res;
        }

        public int[] GetRow(int y)
        {
            var res = new int[4];
            for (var i = 0; i < 4; i++)
            {
                res[i] = this.grid[i, y];
            }

            return res;
        }

        public int SummAll()
        {
            var sum = 0;
            for (var x = 0; x < this.grid.GetLength(0); x++)
            {
                for (var y = 0; y < this.grid.GetLength(0); y++)
                {
                    sum += this.grid[x, y];
                }
            }

            return sum;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public IEnumerator<GridCell> GetEnumerator()
        {
            for (var y = 0; y < this.grid.GetLength(0); y++)
            {
                for (var x = 0; x < this.grid.GetLength(1); x++)
                {
                    yield return new GridCell(this.grid[x, y], x, y);
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
                    sb.Append($"{this.grid[x, y],5}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public static bool operator ==(Grid first, Grid second)
        {
            if (ReferenceEquals(null, first))
            {
                return false;
            }

            if (ReferenceEquals(null, second))
            {
                return false;
            }

            return first.Equals(second);
        }

        public static bool operator !=(Grid first, Grid second)
        {
            if (ReferenceEquals(null, first))
            {
                return false;
            }

            if (ReferenceEquals(null, second))
            {
                return false;
            }

            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (!(obj is Grid))
            {
                return false;
            }

            return this.Equals((Grid)obj);
        }

        public bool Equals(Grid other)
        {
            return this.grid[0, 0] == other.grid[0, 0] && this.grid[0, 1] == other.grid[0, 1]
                   && this.grid[0, 2] == other.grid[0, 2] && this.grid[0, 3] == other.grid[0, 3]
                   && this.grid[1, 0] == other.grid[1, 0] && this.grid[1, 1] == other.grid[1, 1]
                   && this.grid[1, 2] == other.grid[1, 2] && this.grid[1, 3] == other.grid[1, 3]
                   && this.grid[2, 0] == other.grid[2, 0] && this.grid[2, 1] == other.grid[2, 1]
                   && this.grid[2, 2] == other.grid[2, 2] && this.grid[2, 3] == other.grid[2, 3]
                   && this.grid[3, 0] == other.grid[3, 0] && this.grid[3, 1] == other.grid[3, 1]
                   && this.grid[3, 2] == other.grid[3, 2] && this.grid[3, 3] == other.grid[3, 3];
        }
        
        public override int GetHashCode()
        {
            return unchecked(
                this.grid[0, 0] +
                this.grid[0, 1] * 2 +
                this.grid[0, 2] * 3 +
                this.grid[0, 3] * 5 +
                this.grid[1, 0] * 7 +
                this.grid[1, 1] * 11 +
                this.grid[1, 2] * 13 +
                this.grid[1, 3] * 17 +
                this.grid[2, 0] * 19 +
                this.grid[2, 1] * 23 +
                this.grid[2, 2] * 29 +
                this.grid[2, 3] * 31 +
                this.grid[3, 0] * 37 +
                this.grid[3, 1] * 41 +
                this.grid[3, 2] * 43 +
                this.grid[3, 3] * 47);
        }
    }
}