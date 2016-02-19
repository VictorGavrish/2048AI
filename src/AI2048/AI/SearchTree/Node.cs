namespace AI2048.AI.SearchTree
{
    using System;
    using System.Linq;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    public class Node<T> where T : IComparable<T>
    {
        protected Node(IHeuristic<T> heuristic)
        {
            this.Heuristic = heuristic;
            this.heuristicLazy = new Lazy<T>(() => this.Heuristic.Evaluate(this), false);
        }

        public LogarithmicGrid Grid { get; protected set; }

        public readonly IHeuristic<T> Heuristic;
        private readonly Lazy<T> heuristicLazy;
        public T HeuristicValue => this.heuristicLazy.Value;
        
        public int EmptyCellCount => this.Grid.Flatten().Count(i => i == 0);

        public Node<T> RotateCw()
        {
            var newGrid = new byte[4, 4];
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    newGrid[3 - y, x] = this.Grid[x, y];
                }
            }

            var result = new Node<T>(this.Heuristic) { Grid = new LogarithmicGrid(newGrid) };

            return result;
        }
    }
}