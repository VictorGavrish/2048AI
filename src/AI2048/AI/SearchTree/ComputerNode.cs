namespace AI2048.AI.SearchTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class ComputerNode : Node
    {
        public ComputerNode(LogarithmicGrid grid, SearchTree searchTree)
            : base(searchTree)
        {
            this.Grid = grid;

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

            if (!this.allNodesWith2Computed)
            {
                foreach (var possibleState in this.Grid.NextPossibleWorldStatesWith2().Skip(this.computedNodesWith2.Count))
                {
                    PlayerNode playerNode;
                    if (!this.SearchTree.KnownPlayerNodes.TryGetValue(possibleState, out playerNode))
                    {
                        playerNode = new PlayerNode(possibleState, this.SearchTree);
                        this.SearchTree.KnownPlayerNodes.Add(possibleState, playerNode);
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

            if (!this.allNodesWith4Computed)
            {
                foreach (var possibleState in this.Grid.NextPossibleWorldStatesWith4().Skip(this.computedNodesWith4.Count))
                {
                    PlayerNode playerNode;
                    if (!this.SearchTree.KnownPlayerNodes.TryGetValue(possibleState, out playerNode))
                    {
                        playerNode = new PlayerNode(possibleState, this.SearchTree);
                        this.SearchTree.KnownPlayerNodes.Add(possibleState, playerNode);
                    }

                    this.computedNodesWith4.Add(playerNode);
                    yield return playerNode;
                }
            }

            this.allNodesWith4Computed = true;
        }

        public IEnumerable<PlayerNode> Children => this.GetChildrenWith4().Concat(this.GetChildrenWith2());

        public IEnumerable<ComputerNode> GetCachedComputerNodes()
        {
            var with2 = this.computedNodesWith2.SelectMany(pn => pn.GetCachedComputerNodes());

            var with4 = this.computedNodesWith4.SelectMany(pn => pn.GetCachedComputerNodes());

            return new[] { this }.Concat(with2).Concat(with4);
        }

        public IEnumerable<PlayerNode> GetCachedPlayerNodes()
        {
            var with2 = this.computedNodesWith2.SelectMany(pn => pn.GetCachedPlayerNodes());

            var with4 = this.computedNodesWith4.SelectMany(pn => pn.GetCachedPlayerNodes());

            return with2.Concat(with4);
        }
    }
}