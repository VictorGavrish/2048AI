namespace AI2048.AI.SearchTree
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    public class MaximizingNode : Node
    {
        private static readonly Move[] Moves = { Move.Up, Move.Left, Move.Down, Move.Right };

        public MaximizingNode(LogarithmicGrid grid, MinimizingNode parentNode, IHeuristic heuristic)
            : base(heuristic)
        {
            this.Grid = grid;
            this.parentNode = parentNode;

            this.possibleMovesLazy = new Lazy<IEnumerable<Move>>(this.GetPossibleMoves);
            this.childrenByMoveLazy = new Lazy<Dictionary<Move, MinimizingNode>>(this.GetChildrenByMove);
        }

        public MaximizingNode(LogarithmicGrid grid, IHeuristic heuristic)
            : this(grid, null, heuristic)
        {
            this.MakeRoot();
        }

        private MinimizingNode parentNode;
        private bool isRootNode;
        public MaximizingNode RootMaximizingNode => this.isRootNode ? this : this.parentNode.RootMaximizingNode;

        private Dictionary<LogarithmicGrid, MaximizingNode> knownPlayerNodes;
        public Dictionary<LogarithmicGrid, MaximizingNode> KnownPlayerNodes => this.knownPlayerNodes ?? this.parentNode.KnownPlayerNodes;

        private Dictionary<LogarithmicGrid, MinimizingNode> knownComputerNodes;
        public Dictionary<LogarithmicGrid, MinimizingNode> KnownComputerNodes => this.knownComputerNodes ?? this.parentNode.KnownComputerNodes;

        public void MakeRoot()
        {
            this.isRootNode = true;
            this.parentNode = null;
            this.knownPlayerNodes = new Dictionary<LogarithmicGrid, MaximizingNode>();
            this.knownComputerNodes = new Dictionary<LogarithmicGrid, MinimizingNode>();
        }

        public Dictionary<Move, MinimizingNode> Children => this.childrenByMoveLazy.Value;
        private readonly Lazy<Dictionary<Move, MinimizingNode>> childrenByMoveLazy;
        private Dictionary<Move, MinimizingNode> GetChildrenByMove()
        {
            var dictionary = new Dictionary<Move, MinimizingNode>();

            foreach (var move in this.PossibleMoves)
            {
                var newState = this.Grid.MakeMove(move);

                MinimizingNode minimizingNode;

                if (!this.KnownComputerNodes.TryGetValue(newState, out minimizingNode))
                {
                    minimizingNode = new MinimizingNode(newState, this, this.Heuristic);
                    this.KnownComputerNodes.Add(newState, minimizingNode);
                }

                dictionary.Add(move, minimizingNode);
            }

            return dictionary;
        }

        public IEnumerable<Move> PossibleMoves => this.possibleMovesLazy.Value;
        private readonly Lazy<IEnumerable<Move>> possibleMovesLazy;
        private IEnumerable<Move> GetPossibleMoves() => Moves.Where(move => !this.Grid.Equals(this.Grid.MakeMove(move)));

        public bool GameOver => !this.PossibleMoves.Any();
    }
}