namespace AI2048.AI.Victor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class Node
    {
        private static Move[] Moves { get; } = { Move.Up, Move.Left, Move.Down, Move.Right };

        private Node(Grid state, Node parentNode)
        {
            this.State = state;
            this.parentNode = parentNode;

            this.childNodesByMoveLazy = new Lazy<Dictionary<Move, Node[]>>(this.GetPossibleNodesByMove);
            this.childNodesLazy = new Lazy<Node[]>(() => this.ChildNodesByMove.SelectMany(kvp => kvp.Value).ToArray());
            this.possibleMovesLazy = new Lazy<Move[]>(this.GetPossibleMoves);
            this.emptyCellCountLazy = new Lazy<int>(() => this.State.EmptyCellsNo);
        }

        public Node(Grid state)
            : this(state, null)
        {
        }

        public Grid State { get; }

        private Node parentNode;
        private bool isRootNode;
        public Node RootNode => this.isRootNode ? this : this.parentNode.RootNode;

        private ConcurrentDictionary<Grid, Node> knownNodes;
        public IDictionary<Grid, Node> KnownNodes => this.knownNodes ?? this.parentNode.KnownNodes;

        public void MakeRoot()
        {
            this.isRootNode = true;
            this.parentNode = null;
            this.knownNodes = new ConcurrentDictionary<Grid, Node>();
        }

        private readonly Lazy<Node[]> childNodesLazy;
        public Node[] ChildNodes => this.childNodesLazy.Value;

        private readonly Lazy<Dictionary<Move, Node[]>> childNodesByMoveLazy;
        public Dictionary<Move, Node[]> ChildNodesByMove => this.childNodesByMoveLazy.Value;

        private readonly Lazy<int> emptyCellCountLazy;
        public int EmptyCellCount => this.emptyCellCountLazy.Value;

        private readonly Lazy<Move[]> possibleMovesLazy;
        public Move[] PossibleMoves => this.possibleMovesLazy.Value;

        public bool GameOver => !this.PossibleMoves.Any();

        private Move[] GetPossibleMoves()
        {
            return Moves.Where(move => this.State != GameLogic.MakeMove(this.State, move)).ToArray();
        }

        private IEnumerable<Node> GetPossibleNodes(Move move)
        {
            var possibleStates = GameLogic.MakeMovePossibleStates(this.State, move);
            foreach (var possibleState in possibleStates)
            {
                Node node;
                if (!this.KnownNodes.TryGetValue(possibleState, out node))
                {
                    node = new Node(possibleState, this);
                    this.KnownNodes.Add(possibleState, node);
                }
                yield return node;
            }
        }

        private Dictionary<Move, Node[]> GetPossibleNodesByMove()
        {
            return this.PossibleMoves.ToDictionary(move => move, move => this.GetPossibleNodes(move).ToArray());
        }
    }
}