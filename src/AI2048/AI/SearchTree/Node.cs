namespace AI2048.AI.SearchTree
{
    using System;
    using System.Linq;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    public class Node
    {
        private static readonly double Log2 = Math.Log(2);

        protected Node(IHeuristic heuristic)
        {
            this.emptyCellCountLazy = new Lazy<int>(this.GetEmptyCellCount);

            this.Heuristic = heuristic;
            this.heuristicLazy = new Lazy<double>(() => this.Heuristic.Evaluate(this));
        }
        public LogarithmicGrid Grid { get; protected set; }

        public readonly IHeuristic Heuristic;
        private readonly Lazy<double> heuristicLazy;
        public double HeuristicValue => this.heuristicLazy.Value;
        
        public int EmptyCellCount => this.emptyCellCountLazy.Value;
        private readonly Lazy<int> emptyCellCountLazy;
        private int GetEmptyCellCount() => this.Grid.Flatten().Count(i => i == 0);
    }
}