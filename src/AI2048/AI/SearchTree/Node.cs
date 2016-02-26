namespace AI2048.AI.SearchTree
{
    using System;
    using System.Linq;

    using AI2048.Game;

    public abstract class Node
    {
        private readonly Lazy<int> emptyCellCountLazy;

        protected Node(SearchTree searchTree)
        {
            this.SearchTree = searchTree;

            this.emptyCellCountLazy = new Lazy<int>(this.GetEmptyCellCount, false);
        }

        public int EmptyCellCount => this.emptyCellCountLazy.Value;
        private int GetEmptyCellCount() => this.Grid.Flatten().Count(i => i == 0);

        public LogarithmicGrid Grid { get; protected set; }

        public SearchTree SearchTree { get; }
    }
}