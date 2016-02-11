namespace AI2048.Game
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Grid
    {
        protected bool Equals(Grid other)
        {
            return Equals(this.grid, other.grid);
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
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((Grid)obj);
        }

        public override int GetHashCode()
        {
            return (this.grid != null ? this.grid.GetHashCode() : 0);
        }

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

        public int this[int x, int y]
        {
            get
            {
                return this.grid[x, y];
            }
        }

        public int[,] CloneMatrix()
        {
            return (int[,])this.grid.Clone();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var y = 0; y < this.grid.GetLength(0); y++)
            {
                for (var x = 0; x < this.grid.GetLength(0); x++)
                {
                    sb.Append(this.grid[x, y] + " ");
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

            return first.ToString() == second.ToString(); // yeh, this is not the slowest operation here))
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

        public int[] GetRow(int y)
        {
            var res = new int[4];
            for (var i = 0; i < 4; i++)
            {
                res[i] = this.grid[i, y];
            }

            return res;
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

        public int EmptyCellsNo
        {
            get
            {
                var n = 0;
                for (var x = 0; x < this.grid.GetLength(0); x++)
                {
                    for (var y = 0; y < this.grid.GetLength(0); y++)
                    {
                        if (this.grid[x, y] == 0)
                        {
                            n++;
                        }
                    }
                }

                return n;
            }
        }

        public int[] Flatten()
        {
            var res = new List<int>();
            for (var x = 0; x < this.grid.GetLength(0); x++)
            {
                for (var y = 0; y < this.grid.GetLength(0); y++)
                {
                    res.Add(this.grid[x, y]);
                }
            }

            return res.ToArray();
        }
    }
}