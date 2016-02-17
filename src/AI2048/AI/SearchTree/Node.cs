namespace AI2048.AI.SearchTree
{
    using System;
    using System.Linq;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    public class Node
    {
        protected Node(IHeuristic heuristic)
        {
            this.Heuristic = heuristic;
            this.heuristicLazy = new Lazy<double>(() => this.Heuristic.Evaluate(this), false);
        }

        public LogarithmicGrid Grid { get; protected set; }

        public readonly IHeuristic Heuristic;
        private readonly Lazy<double> heuristicLazy;
        public double HeuristicValue => this.heuristicLazy.Value;
        
        public int EmptyCellCount => this.Grid.Flatten().Count(i => i == 0);

        public Node RotateCw()
        {
            var newGrid = new byte[4, 4];
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    newGrid[3 - y, x] = this.Grid[x, y];
                }
            }

            var result = new Node(this.Heuristic) { Grid = new LogarithmicGrid(newGrid) };

            return result;
        }
    }
}