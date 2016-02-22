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

            this.emptyCellCountLazy = new Lazy<int>(this.GetEmptyCellCount, false);
        }

        public LogarithmicGrid Grid { get; protected set; }

        public readonly IHeuristic<T> Heuristic;

        public int EmptyCellCount => this.emptyCellCountLazy.Value;
        private readonly Lazy<int> emptyCellCountLazy;
        private int GetEmptyCellCount() => this.Grid.Flatten().Count(i => i == 0);
    }
}