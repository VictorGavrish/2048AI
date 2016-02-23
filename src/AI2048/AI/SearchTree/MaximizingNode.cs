﻿namespace AI2048.AI.SearchTree
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    public class MaximizingNode<T> : Node<T> where T : IComparable<T>
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Move[] Moves = { Move.Up, Move.Left, Move.Down, Move.Right };

        public MaximizingNode(LogarithmicGrid grid, MinimizingNode<T> parentNode, IHeuristic<T> heuristic)
            : base(heuristic)
        {
            this.Grid = grid;
            this.parentNode = parentNode;

            this.heuristicLazy = new Lazy<T>(() => this.Heuristic.Evaluate(this), false);
            this.possibleStatesLazy = new Lazy<IEnumerable<KeyValuePair<Move, LogarithmicGrid>>>(this.GetPossibleStates, false);
            this.childrenByMoveLazy = new Lazy<IDictionary<Move, MinimizingNode<T>>>(this.GetChildrenByMove, false);
            this.gameOverLazy = new Lazy<bool>(this.GetGameOver, false);
        }

        public MaximizingNode(LogarithmicGrid grid, IHeuristic<T> heuristic)
            : this(grid, null, heuristic)
        {
            this.MakeRoot();
        }

        public double Probabilty { get; set; }

        private MinimizingNode<T> parentNode;
        private bool isRootNode;
        public MaximizingNode<T> RootMaximizingNode => this.isRootNode ? this : this.parentNode.RootMaximizingNode;

        private IDictionary<LogarithmicGrid, MaximizingNode<T>> knownPlayerNodes;
        public IDictionary<LogarithmicGrid, MaximizingNode<T>> KnownPlayerNodes => this.knownPlayerNodes ?? this.parentNode.KnownPlayerNodes;

        private IDictionary<LogarithmicGrid, MinimizingNode<T>> knownComputerNodes;
        public IDictionary<LogarithmicGrid, MinimizingNode<T>> KnownComputerNodes => this.knownComputerNodes ?? this.parentNode.KnownComputerNodes;

        public void MakeRoot()
        {
            this.isRootNode = true;
            this.parentNode = null;
            this.knownPlayerNodes = new Dictionary<LogarithmicGrid, MaximizingNode<T>>();
            this.knownComputerNodes = new Dictionary<LogarithmicGrid, MinimizingNode<T>>();
        }

        private readonly Lazy<T> heuristicLazy;
        public T HeuristicValue => this.heuristicLazy.Value;

        public IDictionary<Move, MinimizingNode<T>> Children => this.childrenByMoveLazy.Value;
        private readonly Lazy<IDictionary<Move, MinimizingNode<T>>> childrenByMoveLazy;
        private IDictionary<Move, MinimizingNode<T>> GetChildrenByMove()
        {
            var dictionary = new Dictionary<Move, MinimizingNode<T>>();

            foreach (var kvp in this.PossibleStates)
            {
                MinimizingNode<T> minimizingNode;
                if (!this.KnownComputerNodes.TryGetValue(kvp.Value, out minimizingNode))
                {
                    minimizingNode = new MinimizingNode<T>(kvp.Value, this, this.Heuristic);
                    this.KnownComputerNodes.Add(kvp.Value, minimizingNode);
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