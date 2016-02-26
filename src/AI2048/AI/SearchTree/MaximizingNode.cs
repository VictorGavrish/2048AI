namespace AI2048.AI.SearchTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class MaximizingNode : Node
    {
        private static readonly Move[] Moves = { Move.Up, Move.Left, Move.Down, Move.Right };

        public MaximizingNode(LogarithmicGrid grid, SearchTree searchTree)
            : base(searchTree)
        {
            this.Grid = grid;

            this.heuristicLazy = new Lazy<double>(() => this.SearchTree.Heuristic.Evaluate(this), false);
            this.possibleStatesLazy = new Lazy<IEnumerable<KeyValuePair<Move, LogarithmicGrid>>>(this.GetPossibleStates, false);
            this.childrenByMoveLazy = new Lazy<IDictionary<Move, MinimizingNode>>(this.GetChildrenByMove, false);
            this.gameOverLazy = new Lazy<bool>(this.GetGameOver, false);
        }

        private readonly Lazy<double> heuristicLazy;
        public double HeuristicValue => this.heuristicLazy.Value;

        public IDictionary<Move, MinimizingNode> Children => this.childrenByMoveLazy.Value;
        private readonly Lazy<IDictionary<Move, MinimizingNode>> childrenByMoveLazy;
        private IDictionary<Move, MinimizingNode> GetChildrenByMove()
        {
            var dictionary = new Dictionary<Move, MinimizingNode>();

            foreach (var kvp in this.PossibleStates)
            {
                MinimizingNode minimizingNode;
                if (!this.SearchTree.KnownComputerNodes.TryGetValue(kvp.Value, out minimizingNode))
                {
                    minimizingNode = new MinimizingNode(kvp.Value, this.SearchTree);
                    this.SearchTree.KnownComputerNodes.Add(kvp.Value, minimizingNode);
                }

                dictionary.Add(kvp.Key, minimizingNode);
            }

            return dictionary;
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