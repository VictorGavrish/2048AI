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

        public MaximizingNode(LogGrid state, MinimizingNode parentNode, Func<Node, double> heuristic)
            : base(heuristic)
        {
            this.State = state;
            this.parentNode = parentNode;

            this.possibleMovesLazy = new Lazy<IEnumerable<Move>>(this.GetPossibleMoves);
            this.childrenByMoveLazy = new Lazy<Dictionary<Move, MinimizingNode>>(this.GetChildrenByMove);
        }

        public MaximizingNode(LogGrid state, Func<Node, double> heuristic)
            : this(state, null, heuristic)
        {
        }

        private MinimizingNode parentNode;
        private bool isRootNode;
        public MaximizingNode RootMaximizingNode => this.isRootNode ? this : this.parentNode.RootMaximizingNode;

        private ConcurrentDictionary<LogGrid, MaximizingNode> knownPlayerNodes;
        public ConcurrentDictionary<LogGrid, MaximizingNode> KnownPlayerNodes => this.knownPlayerNodes ?? this.parentNode.KnownPlayerNodes;

        private ConcurrentDictionary<LogGrid, MinimizingNode> knownComputerNodes;
        public ConcurrentDictionary<LogGrid, MinimizingNode> KnownComputerNodes => this.knownComputerNodes ?? this.parentNode.KnownComputerNodes;

        public void MakeRoot()
        {
            this.isRootNode = true;
            this.parentNode = null;
            this.knownPlayerNodes = new ConcurrentDictionary<LogGrid, MaximizingNode>();
            this.knownComputerNodes = new ConcurrentDictionary<LogGrid, MinimizingNode>();
        }

        public Dictionary<Move, MinimizingNode> Children => this.childrenByMoveLazy.Value;
        private readonly Lazy<Dictionary<Move, MinimizingNode>> childrenByMoveLazy;
        private Dictionary<Move, MinimizingNode> GetChildrenByMove()
        {
            var dictionary = new Dictionary<Move, MinimizingNode>();

            foreach (var move in this.PossibleMoves)
            {
                var newState = this.State.MakeMove(move);

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
        private IEnumerable<Move> GetPossibleMoves() => Moves.Where(move => !this.State.Equals(this.State.MakeMove(move)));

        public bool GameOver => !this.PossibleMoves.Any();
    }
}