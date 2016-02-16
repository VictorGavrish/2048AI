namespace AI2048.AI.Victor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class MaximizingNode : Node
    {
        private static readonly Move[] Moves = { Move.Up, Move.Left, Move.Down, Move.Right };

        public MaximizingNode(Grid state, MinimizingNode parentNode, Func<Node, double> heuristic)
            : base(heuristic)
        {
            this.State = state;
            this.parentNode = parentNode;

            this.possibleMovesLazy = new Lazy<IEnumerable<Move>>(this.GetPossibleMoves);
            this.childrenByMoveLazy = new Lazy<Dictionary<Move, MinimizingNode>>(this.GetChildrenByMove);
        }

        public MaximizingNode(Grid state, Func<Node, double> heuristic)
            : this(state, null, heuristic)
        {
        }

        private MinimizingNode parentNode;
        private bool isRootNode;
        public MaximizingNode RootMaximizingNode => this.isRootNode ? this : this.parentNode.RootMaximizingNode;

        private ConcurrentDictionary<Grid, MaximizingNode> knownPlayerNodes;
        public ConcurrentDictionary<Grid, MaximizingNode> KnownPlayerNodes => this.knownPlayerNodes ?? this.parentNode.KnownPlayerNodes;

        private ConcurrentDictionary<Grid, MinimizingNode> knownComputerNodes;
        public ConcurrentDictionary<Grid, MinimizingNode> KnownComputerNodes => this.knownComputerNodes ?? this.parentNode.KnownComputerNodes;

        public void MakeRoot()
        {
            this.isRootNode = true;
            this.parentNode = null;
            this.knownPlayerNodes = new ConcurrentDictionary<Grid, MaximizingNode>();
            this.knownComputerNodes = new ConcurrentDictionary<Grid, MinimizingNode>();
        }

        public Dictionary<Move, MinimizingNode> Children => this.childrenByMoveLazy.Value;
        private readonly Lazy<Dictionary<Move, MinimizingNode>> childrenByMoveLazy;
        private Dictionary<Move, MinimizingNode> GetChildrenByMove()
        {
            var dictionary = new Dictionary<Move, MinimizingNode>();

            foreach (var move in this.PossibleMoves)
            {
                var newState = GameLogic.MakeMove(this.State, move);

                MinimizingNode minimizingNode;
                if (!this.KnownComputerNodes.TryGetValue(newState, out minimizingNode))
                {
                    minimizingNode = this.KnownComputerNodes.GetOrAdd(newState, new MinimizingNode(newState, this, this.Heuristic));
                }

                dictionary.Add(move, minimizingNode);
            }

            return dictionary;
        }

        public IEnumerable<Move> PossibleMoves => this.possibleMovesLazy.Value;
        private readonly Lazy<IEnumerable<Move>> possibleMovesLazy;

        private IEnumerable<Move> GetPossibleMoves() => Moves.Where(move => this.State != GameLogic.MakeMove(this.State, move));

        public bool GameOver => !this.PossibleMoves.Any();
    }
}