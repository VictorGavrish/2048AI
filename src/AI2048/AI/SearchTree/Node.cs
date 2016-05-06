namespace AI2048.AI.SearchTree
{
    using System;
    using System.Linq;

    using AI2048.Game;

    public abstract class Node
    {
        protected Node(LogarithmicGrid grid, SearchTree searchTree, int sum)
        {
            this.Grid = grid;
            this.SearchTree = searchTree;
            this.Sum = sum;

            this.emptyCellCountLazy = new Lazy<int>(this.GetEmptyCellCount, false);
        }

        public int Sum { get; }

        private readonly Lazy<int> emptyCellCountLazy;
        public int EmptyCellCount => this.emptyCellCountLazy.Value;
        private int GetEmptyCellCount() => this.Grid.Flatten().Count(i => i == 0);

        public LogarithmicGrid Grid { get; }

        public SearchTree SearchTree { get; }
    }
}