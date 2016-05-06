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
            this.RootNode = new PlayerNode(startingGrid, this, startingGrid.Sum());
        }

        public void MoveRoot(LogarithmicGrid newGrid)
        {
            this.RootNode = this.RootNode.Children.Values
                .SelectMany(cn => cn.Children)
                .First(pn => pn.Grid.Equals(newGrid));

            var sum = this.RootNode.Sum;

            foreach (var lesserSum in this.KnownPlayerNodesBySum.Keys.Where(k => k <= sum).ToArray())
            {
                this.KnownPlayerNodesBySum.Remove(lesserSum);
            }

            foreach (var lesserSum in this.KnownComputerNodesBySum.Keys.Where(k => k < sum).ToArray())
            {
                this.KnownComputerNodesBySum.Remove(lesserSum);
            }
        }

        public IDictionary<int, IDictionary<LogarithmicGrid, PlayerNode>> KnownPlayerNodesBySum { get; } = new Dictionary<int, IDictionary<LogarithmicGrid, PlayerNode>>();

        public IDictionary<int, IDictionary<LogarithmicGrid, ComputerNode>> KnownComputerNodesBySum { get; } = new Dictionary<int, IDictionary<LogarithmicGrid, ComputerNode>>();
    }
}