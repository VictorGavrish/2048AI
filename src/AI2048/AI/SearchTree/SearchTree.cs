namespace AI2048.AI.SearchTree
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    using NodaTime;

    public class SearchTree
    {
        public IHeuristic Heuristic { get; }

        public PlayerNode RootNode { get; private set; }

        public SearchTree(IHeuristic heuristic, LogarithmicGrid startingGrid)
        {
            this.Heuristic = heuristic;
            this.RootNode = new PlayerNode(startingGrid, this);

            this.KnownPlayerNodes = new Dictionary<LogarithmicGrid, PlayerNode>();
            this.KnownComputerNodes = new Dictionary<LogarithmicGrid, ComputerNode>();
        }

        private int turnCount;

        public void MoveRoot(LogarithmicGrid newGrid)
        {
            this.turnCount++;

            this.RootNode = this.RootNode.Children.Values
                .SelectMany(cn => cn.Children)
                .First(pn => pn.Grid.Equals(newGrid));

            if (this.turnCount % 10 == 0)
            {
                this.CleanUpDictionaries();
            }
        }

        private void CleanUpDictionaries()
        {
            this.KnownPlayerNodes = this.GetUniqueCachedPlayerNodes()
                .ToDictionary(pn => pn.Grid, pn => pn);
            this.KnownComputerNodes = this.GetUniqueCachedComputerNodes()
                .ToDictionary(cn => cn.Grid, cn => cn);

            GC.Collect();
        }

        public IDictionary<LogarithmicGrid, PlayerNode> KnownPlayerNodes { get; private set; }

        public IDictionary<LogarithmicGrid, ComputerNode> KnownComputerNodes { get; private set; }

        private IEnumerable<PlayerNode> GetUniqueCachedPlayerNodes()
        {
            return this.RootNode.GetCachedPlayerNodes().Distinct();
        }
        private IEnumerable<ComputerNode> GetUniqueCachedComputerNodes()
        {
            return this.RootNode.GetCachedComputerNodes().Distinct();
        }
    }
}