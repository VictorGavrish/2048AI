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

        public IPlayerNode RootNode { get; private set; }

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

        public IDictionary<int, IDictionary<LogarithmicGrid, IPlayerNode>> KnownPlayerNodesBySum { get; } = new Dictionary<int, IDictionary<LogarithmicGrid, IPlayerNode>>();

        public IDictionary<int, IDictionary<LogarithmicGrid, IComputerNode>> KnownComputerNodesBySum { get; } = new Dictionary<int, IDictionary<LogarithmicGrid, IComputerNode>>();

        private abstract class Node
        {
            protected Node(LogarithmicGrid grid, SearchTree searchTree, int sum)
            {
                this.Grid = grid;
                this.SearchTree = searchTree;
                this.Sum = sum;

                this.emptyCellCountLazy = new Lazy<int>(this.GetEmptyCellCount, false);
            }

            public int Sum { get; }

            public int EmptyCellCount => this.emptyCellCountLazy.Value;
            private readonly Lazy<int> emptyCellCountLazy;
            private int GetEmptyCellCount() => this.Grid.Flatten().Count(i => i == 0);

            public LogarithmicGrid Grid { get; }

            public SearchTree SearchTree { get; }
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

                foreach (var node in this.computedNodesWith2)
                {
                    yield return node;
                }

                IDictionary<LogarithmicGrid, IPlayerNode> knownPlayerNodesWithSumPlus2;
                if (!this.SearchTree.KnownPlayerNodesBySum.TryGetValue(this.Sum, out knownPlayerNodesWithSumPlus2))
                {
                    knownPlayerNodesWithSumPlus2 = new Dictionary<LogarithmicGrid, IPlayerNode>();
                    this.SearchTree.KnownPlayerNodesBySum.Add(this.Sum, knownPlayerNodesWithSumPlus2);
                }

                if (!this.allNodesWith2Computed)
                {
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

                foreach (var node in this.computedNodesWith4)
                {
                    yield return node;
                }

                IDictionary<LogarithmicGrid, IPlayerNode> knownPlayerNodesWithSumPlus4;
                if (!this.SearchTree.KnownPlayerNodesBySum.TryGetValue(this.Sum, out knownPlayerNodesWithSumPlus4))
                {
                    knownPlayerNodesWithSumPlus4 = new Dictionary<LogarithmicGrid, IPlayerNode>();
                    this.SearchTree.KnownPlayerNodesBySum.Add(this.Sum, knownPlayerNodesWithSumPlus4);
                }

                if (!this.allNodesWith4Computed)
                {
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
            private readonly IDictionary<Move, IComputerNode> cachedChildren = new Dictionary<Move, IComputerNode>();

            public IDictionary<Move, IComputerNode> Children => this.GetChildrenByMove();
            private bool finishedComputing;
            private IDictionary<Move, IComputerNode> GetChildrenByMove()
            {
                if (this.finishedComputing)
                {
                    return this.cachedChildren;
                }

                IDictionary<LogarithmicGrid, IComputerNode> knownComputerNodesWithSameSum;
                if (!this.SearchTree.KnownComputerNodesBySum.TryGetValue(this.Sum, out knownComputerNodesWithSameSum))
                {
                    knownComputerNodesWithSameSum = new Dictionary<LogarithmicGrid, IComputerNode>();
                    this.SearchTree.KnownComputerNodesBySum.Add(this.Sum, knownComputerNodesWithSameSum);
                }

                foreach (var kvp in this.PossibleStates)
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

            public IEnumerable<KeyValuePair<Move, LogarithmicGrid>> PossibleStates => this.possibleStatesLazy.Value;
            private readonly Lazy<IEnumerable<KeyValuePair<Move, LogarithmicGrid>>> possibleStatesLazy;
            private IEnumerable<KeyValuePair<Move, LogarithmicGrid>> GetPossibleStates() => Moves
                .Select(move => new KeyValuePair<Move, LogarithmicGrid>(move, this.Grid.MakeMove(move)))
                .Where(kvp => !kvp.Value.Equals(this.Grid));

            public bool GameOver => this.gameOverLazy.Value;
            private readonly Lazy<bool> gameOverLazy;
            private bool GetGameOver() => !this.PossibleStates.Any();
        }
    }
}