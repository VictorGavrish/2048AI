namespace AI2048.AI.Victor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class PlayerNode
    {
        private static readonly Move[] Moves = { Move.Up, Move.Left, Move.Down, Move.Right };

        private static readonly double Log2 = Math.Log(2);

        public PlayerNode(Grid state, ComputerNode parentNode)
        {
            this.State = state;
            this.parentNode = parentNode;

            this.possibleMovesLazy = new Lazy<Move[]>(this.GetPossibleMoves);
            this.emptyCellCountLazy = new Lazy<int>(this.GetEmptyCellCount);
            this.childrenByMoveLazy = new Lazy<Dictionary<Move, ComputerNode>>(this.GetChildrenByMove);

            this.emptyCellEvalutionLazy = new Lazy<double>(this.GetEmptyCellEvalution);
            this.maxValueEvalutionLazy = new Lazy<double>(this.GetMaxValueEvalution);
            this.monotonicityLazy = new Lazy<double>(this.GetMonotonicity);
        }

        public PlayerNode(Grid state)
            : this(state, null)
        {
        }

        public Grid State { get; }

        private ComputerNode parentNode;
        private bool isRootNode;
        public PlayerNode RootPlayerNode => this.isRootNode ? this : this.parentNode.RootPlayerNode;

        private ConcurrentDictionary<Grid, PlayerNode> knownPlayerNodes;
        public IDictionary<Grid, PlayerNode> KnownPlayerNodes => this.knownPlayerNodes ?? this.parentNode.KnownPlayerNodes;

        private ConcurrentDictionary<Grid, ComputerNode> knownComputerNodes;
        public IDictionary<Grid, ComputerNode> KnownComputerNodes => this.knownComputerNodes ?? this.parentNode.KnownComputerNodes;

        public void MakeRoot()
        {
            this.isRootNode = true;
            this.parentNode = null;
            this.knownPlayerNodes = new ConcurrentDictionary<Grid, PlayerNode>();
            this.knownComputerNodes = new ConcurrentDictionary<Grid, ComputerNode>();
        }

        public Dictionary<Move, ComputerNode> Children => this.childrenByMoveLazy.Value;
        private readonly Lazy<Dictionary<Move, ComputerNode>> childrenByMoveLazy;
        private Dictionary<Move, ComputerNode> GetChildrenByMove()
        {
            var dictionary = new Dictionary<Move, ComputerNode>();

            foreach (var move in this.PossibleMoves)
            {
                var newState = GameLogic.MakeMove(this.State, move);

                ComputerNode computerNode;
                if (!this.KnownComputerNodes.TryGetValue(newState, out computerNode))
                {
                    computerNode = new ComputerNode(newState, this);
                    this.KnownComputerNodes.Add(newState, computerNode);
                }

                dictionary.Add(move, computerNode);
            }

            return dictionary;
        }

        public int EmptyCellCount => this.emptyCellCountLazy.Value;
        private readonly Lazy<int> emptyCellCountLazy;
        private int GetEmptyCellCount() => this.State.EmptyCellsNo;

        public Move[] PossibleMoves => this.possibleMovesLazy.Value;
        private readonly Lazy<Move[]> possibleMovesLazy;
        private Move[] GetPossibleMoves() => Moves.Where(move => this.State != GameLogic.MakeMove(this.State, move)).ToArray();

        public bool GameOver => !this.PossibleMoves.Any();

        public bool IsTernminal => this.GameOver || (this.parentNode?.IsTerminal ?? false);


        #region evaluations

        private readonly Lazy<double> emptyCellEvalutionLazy;
        public double EmptyCellEvalution => this.emptyCellEvalutionLazy.Value;

        private readonly Lazy<double> maxValueEvalutionLazy;
        public double MaxValueEvaluation => this.maxValueEvalutionLazy.Value;

        private readonly Lazy<double> monotonicityLazy;
        public double Monotonicity => this.monotonicityLazy.Value;

        private double GetEmptyCellEvalution()
        {
            return Math.Log(this.EmptyCellCount);
        }

        private double GetMaxValueEvalution()
        {
            return Math.Log(this.State.Flatten().Max()) / Log2;
        }

        public double GetMonotonicity()
        {
            double down = 0;
            double up = 0;
            for (var x = 0; x < 4; x++)
            {
                var current = 0;
                var next = current + 1;
                while (next < 4)
                {
                    while (next < 4 && this.State[x, next] == 0)
                    {
                        next++;
                    }

                    if (next >= 4)
                    {
                        next--;
                    }

                    var currentValue = this.State[x, current] != 0 ? Math.Log(this.State[x, current]) / Log2 : 0;
                    var nextValue = this.State[x, next] != 0 ? Math.Log(this.State[x, next] / Log2) : 0;

                    if (current > nextValue)
                    {
                        down += nextValue - currentValue;
                    }
                    else if (nextValue > currentValue)
                    {
                        up += currentValue - nextValue;
                    }

                    current = next;
                    next++;
                }
            }

            double right = 0;
            double left = 0;
            for (var y = 0; y < 4; y++)
            {
                var current = 0;
                var next = current + 1;

                while (next < 4)
                {
                    while (next < 4 && this.State[next, y] == 0)
                    {
                        next++;
                    }

                    if (next >= 4)
                    {
                        next--;
                    }

                    var currentValue = this.State[current, y] != 0 ? Math.Log(this.State[current, y]) / Log2 : 0;
                    var nextValue = this.State[next, y] != 0 ? Math.Log(this.State[next, y] / Log2) : 0;

                    if (currentValue > nextValue)
                    {
                        right += nextValue - currentValue;
                    }
                    else if (nextValue > currentValue)
                    {
                        left += currentValue - nextValue;
                    }

                    current = next;
                    next++;
                }
            }

            return Math.Max(up, down) + Math.Max(left, right);
        }
        #endregion
    }
}