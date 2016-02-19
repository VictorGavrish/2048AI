namespace AI2048.AI.SearchTree
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    public class MinimizingNode<T> : Node<T> where T : IComparable<T>
    {
        private readonly MaximizingNode<T> parentNode;

        public MinimizingNode(LogarithmicGrid grid, MaximizingNode<T> parentNode, IHeuristic<T> heuristic)
            : base(heuristic)
        {
            this.parentNode = parentNode;
            this.Grid = grid;

            this.childrenLazy = new Lazy<IEnumerable<MaximizingNode<T>>>(this.GetChildren, false);
        }

        public MaximizingNode<T> RootMaximizingNode => this.parentNode.RootMaximizingNode;
        
        public IDictionary<LogarithmicGrid, MaximizingNode<T>> KnownPlayerNodes => this.parentNode.KnownPlayerNodes;

        public IDictionary<LogarithmicGrid, MinimizingNode<T>> KnownComputerNodes => this.parentNode.KnownComputerNodes;

        private readonly List<MaximizingNode<T>> computedNodes = new List<MaximizingNode<T>>();
        private bool allNodesComputed;

        public IEnumerable<MaximizingNode<T>> Children => this.childrenLazy.Value;
        private readonly Lazy<IEnumerable<MaximizingNode<T>>> childrenLazy;
        private IEnumerable<MaximizingNode<T>> GetChildren()
        {
            foreach (var node in this.computedNodes)
            {
                yield return node;
            }

            if (!this.allNodesComputed)
            {
                foreach (var possibleState in this.Grid.NextPossibleWorldStates().Skip(this.computedNodes.Count))
                {
                    MaximizingNode<T> maximizingNode;
                    if (!this.KnownPlayerNodes.TryGetValue(possibleState, out maximizingNode))
                    {
                        maximizingNode = new MaximizingNode<T>(possibleState, this, this.Heuristic);
                        this.KnownPlayerNodes.Add(possibleState, maximizingNode);
                    }

                    this.computedNodes.Add(maximizingNode);
                    yield return maximizingNode;
                }
            }

            this.allNodesComputed = true;
        }
    }
}