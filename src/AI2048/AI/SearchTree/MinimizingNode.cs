namespace AI2048.AI.SearchTree
{
    using System;
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

            this.heuristicLazy = new Lazy<T>(() => this.Heuristic.Evaluate(this), false);
        }

        public MaximizingNode<T> RootMaximizingNode => this.parentNode.RootMaximizingNode;
        
        public IDictionary<LogarithmicGrid, MaximizingNode<T>> KnownPlayerNodes => this.parentNode.KnownPlayerNodes;

        public IDictionary<LogarithmicGrid, MinimizingNode<T>> KnownComputerNodes => this.parentNode.KnownComputerNodes;

        private readonly Lazy<T> heuristicLazy;
        public T HeuristicValue => this.heuristicLazy.Value;

        private readonly List<MaximizingNode<T>> computedNodesWith2 = new List<MaximizingNode<T>>();
        private bool allNodesWith2Computed;
        public IEnumerable<MaximizingNode<T>> ChildrenWith2 => this.GetChildrenWith2();
        private IEnumerable<MaximizingNode<T>> GetChildrenWith2()
        {
            foreach (var node in this.computedNodesWith2)
            {
                yield return node;
            }

            if (!this.allNodesWith2Computed)
            {
                foreach (var possibleState in this.Grid.NextPossibleWorldStatesWith2().Skip(this.computedNodesWith2.Count))
                {
                    MaximizingNode<T> maximizingNode;
                    if (!this.KnownPlayerNodes.TryGetValue(possibleState, out maximizingNode))
                    {
                        maximizingNode = new MaximizingNode<T>(possibleState, this, this.Heuristic);
                        this.KnownPlayerNodes.Add(possibleState, maximizingNode);
                    }

                    this.computedNodesWith2.Add(maximizingNode);
                    yield return maximizingNode;
                }
            }

            this.allNodesWith2Computed = true;
        }

        private readonly List<MaximizingNode<T>> computedNodesWith4 = new List<MaximizingNode<T>>();
        private bool allNodesWith4Computed;
        public IEnumerable<MaximizingNode<T>> ChildrenWith4 => this.GetChildrenWith4();
        private IEnumerable<MaximizingNode<T>> GetChildrenWith4()
        {
            foreach (var node in this.computedNodesWith4)
            {
                yield return node;
            }

            if (!this.allNodesWith4Computed)
            {
                foreach (var possibleState in this.Grid.NextPossibleWorldStatesWith4().Skip(this.computedNodesWith4.Count))
                {
                    MaximizingNode<T> maximizingNode;
                    if (!this.KnownPlayerNodes.TryGetValue(possibleState, out maximizingNode))
                    {
                        maximizingNode = new MaximizingNode<T>(possibleState, this, this.Heuristic);
                        this.KnownPlayerNodes.Add(possibleState, maximizingNode);
                    }

                    this.computedNodesWith4.Add(maximizingNode);
                    yield return maximizingNode;
                }
            }

            this.allNodesWith4Computed = true;
        }

        public IEnumerable<MaximizingNode<T>> Children => this.GetChildrenWith4().Concat(this.GetChildrenWith2());
    }
}