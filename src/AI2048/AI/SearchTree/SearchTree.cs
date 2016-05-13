namespace AI2048.AI.SearchTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    public class SearchTree : ISearchTree
    {
        public IHeuristic Heuristic { get; }

        private PlayerNode rootNode;
        public IPlayerNode RootNode => this.rootNode;

        public int KnownPlayerNodeCount => this.knownPlayerNodesBySum.Sum(kvp => kvp.Value.Count);

        public int KnownComputerNodeCount => this.knownComputerNodesBySum.Sum(kvp => kvp.Value.Count);

        public SearchTree(IHeuristic heuristic, LogarithmicGrid startingGrid)
        {
            this.Heuristic = heuristic;
            this.rootNode = new PlayerNode(startingGrid, this, startingGrid.Sum());
        }

        public void MoveRoot(LogarithmicGrid newGrid)
        {
            this.rootNode = (PlayerNode)this.rootNode.Children.Values
                                            .SelectMany(cn => cn.Children)
                                            .First(pn => pn.Grid.Equals(newGrid));

            var sum = this.rootNode.Sum;

            foreach (var lesserSum in this.knownPlayerNodesBySum.Keys.Where(k => k <= sum).ToArray())
            {
                this.knownPlayerNodesBySum.Remove(lesserSum);
            }

            foreach (var lesserSum in this.knownComputerNodesBySum.Keys.Where(k => k < sum).ToArray())
            {
                this.knownComputerNodesBySum.Remove(lesserSum);
            }
        }

        private readonly IDictionary<int, IDictionary<LogarithmicGrid, IPlayerNode>> knownPlayerNodesBySum = new Dictionary<int, IDictionary<LogarithmicGrid, IPlayerNode>>();

        private readonly IDictionary<int, IDictionary<LogarithmicGrid, IComputerNode>> knownComputerNodesBySum = new Dictionary<int, IDictionary<LogarithmicGrid, IComputerNode>>();

        private abstract class Node
        {
            protected Node(LogarithmicGrid grid, SearchTree searchTree, int sum)
            {
                this.Grid = grid;
                this.SearchTree = searchTree;
                this.Sum = sum;
            }

            public int Sum { get; }

            public LogarithmicGrid Grid { get; }

            protected SearchTree SearchTree { get; }
        }

        private class ComputerNode : Node, IComputerNode
        {
            public ComputerNode(LogarithmicGrid grid, SearchTree searchTree, int sum)
                : base(grid, searchTree, sum)
            {
            }

            public IEnumerable<IPlayerNode> ChildrenWith2 => this.GetChildrenWith2();
            private List<IPlayerNode> computedNodesWith2;
            private bool allNodesWith2Computed;
            private IEnumerable<IPlayerNode> GetChildrenWith2()
            {
                if (this.computedNodesWith2 == null)
                {
                    this.computedNodesWith2 = new List<IPlayerNode>();
                }
                else
                {
                    foreach (var node in this.computedNodesWith2)
                    {
                        yield return node;
                    }
                }

                if (!this.allNodesWith2Computed)
                {
                    IDictionary<LogarithmicGrid, IPlayerNode> knownPlayerNodesWithSumPlus2;
                    if (!this.SearchTree.knownPlayerNodesBySum.TryGetValue(this.Sum, out knownPlayerNodesWithSumPlus2))
                    {
                        knownPlayerNodesWithSumPlus2 = new Dictionary<LogarithmicGrid, IPlayerNode>();
                        this.SearchTree.knownPlayerNodesBySum.Add(this.Sum, knownPlayerNodesWithSumPlus2);
                    }

                    foreach (var possibleState in this.Grid.NextPossibleStatesWith2().Skip(this.computedNodesWith2.Count))
                    {
                        IPlayerNode playerNode;
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

            public IEnumerable<IPlayerNode> ChildrenWith4 => this.GetChildrenWith4();
            private List<IPlayerNode> computedNodesWith4;
            private bool allNodesWith4Computed;
            private IEnumerable<IPlayerNode> GetChildrenWith4()
            {
                if (this.computedNodesWith4 == null)
                {
                    this.computedNodesWith4 = new List<IPlayerNode>();
                }
                else
                {
                    foreach (var node in this.computedNodesWith4)
                    {
                        yield return node;
                    }
                }

                if (!this.allNodesWith4Computed)
                {
                    IDictionary<LogarithmicGrid, IPlayerNode> knownPlayerNodesWithSumPlus4;
                    if (!this.SearchTree.knownPlayerNodesBySum.TryGetValue(this.Sum, out knownPlayerNodesWithSumPlus4))
                    {
                        knownPlayerNodesWithSumPlus4 = new Dictionary<LogarithmicGrid, IPlayerNode>();
                        this.SearchTree.knownPlayerNodesBySum.Add(this.Sum, knownPlayerNodesWithSumPlus4);
                    }

                    foreach (var possibleState in this.Grid.NextPossibleStatesWith4().Skip(this.computedNodesWith4.Count))
                    {
                        IPlayerNode playerNode;
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

            public IEnumerable<IPlayerNode> Children => this.GetChildrenWith4().Concat(this.GetChildrenWith2());
        }

        private class PlayerNode : Node, IPlayerNode
        {
            private static readonly Move[] Moves = {Move.Up, Move.Left, Move.Down, Move.Right};

            public PlayerNode(LogarithmicGrid grid, SearchTree searchTree, int sum)
                : base(grid, searchTree, sum)
            {
                this.heuristicLazy = new Lazy<double>(() => this.SearchTree.Heuristic.Evaluate(this), false);
                this.possibleStatesLazy = new Lazy<IEnumerable<KeyValuePair<Move, LogarithmicGrid>>>(this.GetPossibleStates, false);
                this.gameOverLazy = new Lazy<bool>(this.GetGameOver, false);
            }

            public double HeuristicValue => this.heuristicLazy.Value;
            private readonly Lazy<double> heuristicLazy;

            private readonly Lazy<IEnumerable<KeyValuePair<Move, LogarithmicGrid>>> possibleStatesLazy;
            private IEnumerable<KeyValuePair<Move, LogarithmicGrid>> GetPossibleStates() => Moves
                .Select(move => new KeyValuePair<Move, LogarithmicGrid>(move, this.Grid.MakeMove(move)))
                .Where(kvp => !kvp.Value.Equals(this.Grid));

            public bool GameOver => this.gameOverLazy.Value;
            private readonly Lazy<bool> gameOverLazy;
            private bool GetGameOver() => !this.possibleStatesLazy.Value.Any();

            public IDictionary<Move, IComputerNode> Children => this.GetChildrenByMove();
            private readonly IDictionary<Move, IComputerNode> cachedChildren = new Dictionary<Move, IComputerNode>();
            private bool finishedComputing;
            private IDictionary<Move, IComputerNode> GetChildrenByMove()
            {
                if (this.finishedComputing)
                {
                    return this.cachedChildren;
                }

                IDictionary<LogarithmicGrid, IComputerNode> knownComputerNodesWithSameSum;
                if (!this.SearchTree.knownComputerNodesBySum.TryGetValue(this.Sum, out knownComputerNodesWithSameSum))
                {
                    knownComputerNodesWithSameSum = new Dictionary<LogarithmicGrid, IComputerNode>();
                    this.SearchTree.knownComputerNodesBySum.Add(this.Sum, knownComputerNodesWithSameSum);
                }

                foreach (var kvp in this.possibleStatesLazy.Value)
                {
                    IComputerNode computerNode;

                    if (!knownComputerNodesWithSameSum.TryGetValue(kvp.Value, out computerNode))
                    {
                        computerNode = new ComputerNode(kvp.Value, this.SearchTree, this.Sum);
                        knownComputerNodesWithSameSum.Add(kvp.Value, computerNode);
                    }

                    this.cachedChildren.Add(kvp.Key, computerNode);
                }

                this.finishedComputing = true;

                return this.cachedChildren;
            }
        }
    }
}