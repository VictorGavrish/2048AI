namespace AI2048.AI.SearchTree
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    public class MinimizingNode : Node
    {
        private readonly MaximizingNode parentNode;

        public MinimizingNode(LogarithmicGrid grid, MaximizingNode parentNode, IHeuristic heuristic)
            : base(heuristic)
        {
            this.parentNode = parentNode;
            this.Grid = grid;

            this.childrenLazy = new Lazy<IEnumerable<MaximizingNode>>(this.GetChildren, false);
        }

        public MaximizingNode RootMaximizingNode => this.parentNode.RootMaximizingNode;
        
        public Dictionary<LogarithmicGrid, MaximizingNode> KnownPlayerNodes => this.parentNode.KnownPlayerNodes;

        public Dictionary<LogarithmicGrid, MinimizingNode> KnownComputerNodes => this.parentNode.KnownComputerNodes;

        public IEnumerable<MaximizingNode> Children => this.childrenLazy.Value;
        private readonly Lazy<IEnumerable<MaximizingNode>> childrenLazy;
        private IEnumerable<MaximizingNode> GetChildren()
        {
            var possibleStates = this.Grid.NextPossibleWorldStates();
            foreach (var possibleState in possibleStates)
            {
                MaximizingNode maximizingNode;
                if (!this.KnownPlayerNodes.TryGetValue(possibleState, out maximizingNode))
                {
                    maximizingNode = new MaximizingNode(possibleState, this, this.Heuristic);
                    this.KnownPlayerNodes.Add(possibleState, maximizingNode);
                }

                yield return maximizingNode;
            }
        }
    }
}