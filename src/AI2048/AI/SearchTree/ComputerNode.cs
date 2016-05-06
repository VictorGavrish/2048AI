namespace AI2048.AI.SearchTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class ComputerNode : Node
    {
        public ComputerNode(LogarithmicGrid grid, SearchTree searchTree, int sum)
            : base(grid, searchTree, sum)
        {
            this.heuristicLazy = new Lazy<double>(() => this.SearchTree.Heuristic.Evaluate(this), false);
        }

        private readonly Lazy<double> heuristicLazy;
        public double HeuristicValue => this.heuristicLazy.Value;

        private readonly List<PlayerNode> computedNodesWith2 = new List<PlayerNode>();
        private bool allNodesWith2Computed;
        public IEnumerable<PlayerNode> ChildrenWith2 => this.GetChildrenWith2();
        private IEnumerable<PlayerNode> GetChildrenWith2()
        {
            foreach (var node in this.computedNodesWith2)
            {
                yield return node;
            }

            IDictionary<LogarithmicGrid, PlayerNode> knownPlayerNodesWithSumPlus2;
            if (!this.SearchTree.KnownPlayerNodesBySum.TryGetValue(this.Sum, out knownPlayerNodesWithSumPlus2))
            {
                knownPlayerNodesWithSumPlus2 = new Dictionary<LogarithmicGrid, PlayerNode>();
                this.SearchTree.KnownPlayerNodesBySum.Add(this.Sum, knownPlayerNodesWithSumPlus2);
            }

            if (!this.allNodesWith2Computed)
            {
                foreach (var possibleState in this.Grid.NextPossibleStatesWith2().Skip(this.computedNodesWith2.Count))
                {
                    PlayerNode playerNode;
                    if (!knownPlayerNodesWithSumPlus2.TryGetValue(possibleState, out playerNode))
                    {
                        playerNode = new PlayerNode(possibleState, this.SearchTree, this.Sum + 2);
                        knownPlayerNodesWithSumPlus2.Add(possibleState, playerNode);
                    }

                    this.computedNodesWith2.Add(playerNode);
                    yield return playerNode;
                }
            }

            this.allNodesWith2Computed = true;
        }

        private readonly List<PlayerNode> computedNodesWith4 = new List<PlayerNode>();
        private bool allNodesWith4Computed;
        public IEnumerable<PlayerNode> ChildrenWith4 => this.GetChildrenWith4();
        private IEnumerable<PlayerNode> GetChildrenWith4()
        {
            foreach (var node in this.computedNodesWith4)
            {
                yield return node;
            }

            IDictionary<LogarithmicGrid, PlayerNode> knownPlayerNodesWithSumPlus4;
            if (!this.SearchTree.KnownPlayerNodesBySum.TryGetValue(this.Sum, out knownPlayerNodesWithSumPlus4))
            {
                knownPlayerNodesWithSumPlus4 = new Dictionary<LogarithmicGrid, PlayerNode>();
                this.SearchTree.KnownPlayerNodesBySum.Add(this.Sum, knownPlayerNodesWithSumPlus4);
            }

            if (!this.allNodesWith4Computed)
            {
                foreach (var possibleState in this.Grid.NextPossibleStatesWith4().Skip(this.computedNodesWith4.Count))
                {
                    PlayerNode playerNode;
                    if (!knownPlayerNodesWithSumPlus4.TryGetValue(possibleState, out playerNode))
                    {
                        playerNode = new PlayerNode(possibleState, this.SearchTree, this.Sum + 4);
                        knownPlayerNodesWithSumPlus4.Add(possibleState, playerNode);
                    }

                    this.computedNodesWith4.Add(playerNode);
                    yield return playerNode;
                }
            }

            this.allNodesWith4Computed = true;
        }

        public IEnumerable<PlayerNode> Children => this.GetChildrenWith4().Concat(this.GetChildrenWith2());
    }
}