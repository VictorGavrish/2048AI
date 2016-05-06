namespace AI2048.AI.SearchTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class PlayerNode : Node
    {
        private static readonly Move[] Moves = { Move.Up, Move.Left, Move.Down, Move.Right };

        public PlayerNode(LogarithmicGrid grid, SearchTree searchTree, int sum)
            : base(grid, searchTree, sum)
        {
            this.heuristicLazy = new Lazy<double>(() => this.SearchTree.Heuristic.Evaluate(this), false);
            this.possibleStatesLazy = new Lazy<IEnumerable<KeyValuePair<Move, LogarithmicGrid>>>(this.GetPossibleStates, false);
            this.gameOverLazy = new Lazy<bool>(this.GetGameOver, false);
        }

        private readonly Lazy<double> heuristicLazy;
        public double HeuristicValue => this.heuristicLazy.Value;

        private readonly IDictionary<Move, ComputerNode> cachedChildren = new Dictionary<Move, ComputerNode>();
        private bool finishedComputing;

        public IDictionary<Move, ComputerNode> Children => this.GetChildrenByMove();
        private IDictionary<Move, ComputerNode> GetChildrenByMove()
        {
            if (this.finishedComputing)
            {
                return this.cachedChildren;
            }

            IDictionary<LogarithmicGrid, ComputerNode> knownComputerNodesWithSameSum;
            if (!this.SearchTree.KnownComputerNodesBySum.TryGetValue(this.Sum, out knownComputerNodesWithSameSum))
            {
                knownComputerNodesWithSameSum = new Dictionary<LogarithmicGrid, ComputerNode>();
                this.SearchTree.KnownComputerNodesBySum.Add(this.Sum, knownComputerNodesWithSameSum);
            }

            foreach (var kvp in this.PossibleStates)
            {
                ComputerNode computerNode;

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