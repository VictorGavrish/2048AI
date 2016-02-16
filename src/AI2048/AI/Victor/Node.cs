namespace AI2048.AI.Victor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public abstract class Node
    {
        private static readonly double Log2 = Math.Log(2);

        protected Node(Func<Node, double> heuristic)
        {
            this.emptyCellCountLazy = new Lazy<int>(this.GetEmptyCellCount);

            this.Heuristic = heuristic;
            this.heuristicLazy = new Lazy<double>(() => this.Heuristic(this));

            this.emptyCellEvalutionLazy = new Lazy<double>(this.GetEmptyCellEvalution);
            this.maxValueEvalutionLazy = new Lazy<double>(this.GetMaxValueEvalution);
            this.monotonicityLazy = new Lazy<double>(this.GetMonotonicity);
            this.smoothnessLazy = new Lazy<double>(this.GetSmoothness);
        }
        public Grid State { get; protected set; }

        public readonly Func<Node, double> Heuristic;
        private readonly Lazy<double> heuristicLazy;
        public double HeuristicValue => this.heuristicLazy.Value;
        
        public int EmptyCellCount => this.emptyCellCountLazy.Value;
        private readonly Lazy<int> emptyCellCountLazy;
        private int GetEmptyCellCount() => this.State.EmptyCellsNo;

        public double EmptyCellEvalution => this.emptyCellEvalutionLazy.Value;
        private readonly Lazy<double> emptyCellEvalutionLazy;
        private double GetEmptyCellEvalution() => Math.Log(this.EmptyCellCount);

        public double MaxValueEvaluation => this.maxValueEvalutionLazy.Value;
        private readonly Lazy<double> maxValueEvalutionLazy;
        private double GetMaxValueEvalution() => Math.Log(this.State.Flatten().Max()) / Log2;

        public double Smoothness => this.smoothnessLazy.Value;
        private readonly Lazy<double> smoothnessLazy;
        public double GetSmoothness()
        {
            double smoothness = 0;
            foreach (var cell in this.State.Where(c => c.Value != 0))
            {
                var value = Math.Log(cell.Value) / Log2;

                foreach (var neighbor in this.GetNeighbors(cell))
                {
                    var neighborValue = Math.Log(neighbor.Value) / Log2;
                    smoothness -= Math.Abs(value - neighborValue);
                }
            }

            return smoothness;
        }

        private IEnumerable<GridCell> GetNeighbors(GridCell cell)
        {
            for (int x = cell.X + 1; x < 4; x++)
            {
                if (this.State[x, cell.Y] == 0)
                {
                    continue;
                }

                yield return new GridCell(this.State[x, cell.Y], x, cell.Y);
                break;
            }

            for (int y = cell.Y + 1; y < 4; y++)
            {
                if (this.State[cell.X, y] == 0)
                {
                    continue;
                }

                yield return new GridCell(this.State[cell.X, y], cell.X, y);
                break;
            }
        }

        public double Monotonicity => this.monotonicityLazy.Value;
        private readonly Lazy<double> monotonicityLazy;
        private double GetMonotonicity()
        {
            double down = 0;
            double up = 0;
            for (var x = 0; x < 4; x++)
            {
                var current = 0;
                var next = current + 1;
                while (next < 4)
                {
                    while (next < 4 && this.State[x, next] == 0)
                    {
                        next++;
                    }

                    if (next >= 4)
                    {
                        next--;
                    }

                    var currentValue = this.State[x, current] != 0 ? Math.Log(this.State[x, current]) / Log2 : 0;
                    var nextValue = this.State[x, next] != 0 ? Math.Log(this.State[x, next] / Log2) : 0;

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
                    while (next < 4 && this.State[next, y] == 0)
                    {
                        next++;
                    }

                    if (next >= 4)
                    {
                        next--;
                    }

                    var currentValue = this.State[current, y] != 0 ? Math.Log(this.State[current, y]) / Log2 : 0;
                    var nextValue = this.State[next, y] != 0 ? Math.Log(this.State[next, y] / Log2) : 0;

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