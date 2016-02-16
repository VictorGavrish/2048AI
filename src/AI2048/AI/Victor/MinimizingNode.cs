namespace AI2048.AI.Victor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class MinimizingNode : Node
    {
        private readonly MaximizingNode parentNode;

        public MinimizingNode(Grid state, MaximizingNode parentNode, Func<Node, double> heuristic)
            : base(heuristic)
        {
            this.parentNode = parentNode;
            this.State = state;

            this.childrenLazy = new Lazy<IEnumerable<MaximizingNode>>(this.GetChildren);
        }

        public MaximizingNode RootMaximizingNode => this.parentNode.RootMaximizingNode;
        
        public ConcurrentDictionary<Grid, MaximizingNode> KnownPlayerNodes => this.parentNode.KnownPlayerNodes;

        public ConcurrentDictionary<Grid, MinimizingNode> KnownComputerNodes => this.parentNode.KnownComputerNodes;

        public bool GameOver => this.parentNode.GameOver;

        public IEnumerable<MaximizingNode> Children => this.childrenLazy.Value;
        private readonly Lazy<IEnumerable<MaximizingNode>> childrenLazy;
        private IEnumerable<MaximizingNode> GetChildren()
        {
            var possibleStates = GameLogic.NextPossibleWorldStates(this.State);
            foreach (var possibleState in possibleStates)
            {
                MaximizingNode maximizingNode;
                if (!this.KnownPlayerNodes.TryGetValue(possibleState, out maximizingNode))
                {
                    maximizingNode = this.KnownPlayerNodes.GetOrAdd(possibleState, new MaximizingNode(possibleState, this, this.Heuristic));
                }

                yield return maximizingNode;
            }
        }
    }
}