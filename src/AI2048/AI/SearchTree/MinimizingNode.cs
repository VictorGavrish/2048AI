namespace AI2048.AI.SearchTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class MinimizingNode : Node
    {
        public MinimizingNode(LogarithmicGrid grid, SearchTree searchTree)
            : base(searchTree)
        {
            this.Grid = grid;

            this.heuristicLazy = new Lazy<double>(() => this.SearchTree.Heuristic.Evaluate(this), false);
        }

        private readonly Lazy<double> heuristicLazy;
        public double HeuristicValue => this.heuristicLazy.Value;

        private readonly List<MaximizingNode> computedNodesWith2 = new List<MaximizingNode>();
        private bool allNodesWith2Computed;
        public IEnumerable<MaximizingNode> ChildrenWith2 => this.GetChildrenWith2();
        private IEnumerable<MaximizingNode> GetChildrenWith2()
        {
            foreach (var node in this.computedNodesWith2)
            {
                yield return node;
            }

            if (!this.allNodesWith2Computed)
            {
                foreach (var possibleState in this.Grid.NextPossibleWorldStatesWith2().Skip(this.computedNodesWith2.Count))
                {
                    MaximizingNode maximizingNode;
                    if (!this.SearchTree.KnownPlayerNodes.TryGetValue(possibleState, out maximizingNode))
                    {
                        maximizingNode = new MaximizingNode(possibleState, this.SearchTree);
                        this.SearchTree.KnownPlayerNodes.Add(possibleState, maximizingNode);
                    }

                    this.computedNodesWith2.Add(maximizingNode);
                    yield return maximizingNode;
                }
            }

            this.allNodesWith2Computed = true;
        }

        private readonly List<MaximizingNode> computedNodesWith4 = new List<MaximizingNode>();
        private bool allNodesWith4Computed;
        public IEnumerable<MaximizingNode> ChildrenWith4 => this.GetChildrenWith4();
        private IEnumerable<MaximizingNode> GetChildrenWith4()
        {
            foreach (var node in this.computedNodesWith4)
            {
                yield return node;
            }

            if (!this.allNodesWith4Computed)
            {
                foreach (var possibleState in this.Grid.NextPossibleWorldStatesWith4().Skip(this.computedNodesWith4.Count))
                {
                    MaximizingNode maximizingNode;
                    if (!this.SearchTree.KnownPlayerNodes.TryGetValue(possibleState, out maximizingNode))
                    {
                        maximizingNode = new MaximizingNode(possibleState, this.SearchTree);
                        this.SearchTree.KnownPlayerNodes.Add(possibleState, maximizingNode);
                    }

                    this.computedNodesWith4.Add(maximizingNode);
                    yield return maximizingNode;
                }
            }

            this.allNodesWith4Computed = true;
        }

        public IEnumerable<MaximizingNode> Children => this.GetChildrenWith4().Concat(this.GetChildrenWith2());
    }
}