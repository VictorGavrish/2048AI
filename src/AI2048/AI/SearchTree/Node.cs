namespace AI2048.AI.SearchTree
{
    using System;
    using System.Linq;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    public abstract class Node<T> where T : IComparable<T>
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
    }
}