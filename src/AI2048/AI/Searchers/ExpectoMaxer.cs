namespace AI2048.AI.Searchers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.Searchers.Models;
    using AI2048.AI.SearchTree;
    using AI2048.Game;

    using NodaTime;

    public class ExpectoMaxer : IConfigurableDepthSearcher, IConfigurableMovesSearcher
    {
        private static readonly double MinEvaluation = -1000000000;

        private readonly MaximizingNode<double> rootNode;

        private SearchStatistics searchStatistics;

        private IEnumerable<Move> allowedMoves;

        private int searchDepth;

        public ExpectoMaxer(MaximizingNode<double> rootNode, int minSearchDepth = 3)
        {
            this.rootNode = rootNode;
            this.searchDepth = minSearchDepth;
        }

        public SearchResult Search()
        {
            this.searchStatistics = new SearchStatistics
            {
                RootNodeGrandchildren = this.rootNode.Children.Values.Sum(c => c.Children.Count())
            };

            var startTime = SystemClock.Instance.Now;

            var evaluationResult = this.InitializeEvaluation();

            this.searchStatistics.SearchExhaustive =
                evaluationResult.All(kvp => kvp.Value <= MinEvaluation + this.searchDepth);
            this.searchStatistics.SearchDuration = SystemClock.Instance.Now - startTime;
            this.searchStatistics.SearchDepth = this.searchDepth;

            return new SearchResult
            {
                SearcherName = nameof(ExpectoMaxer),
                MoveEvaluations = evaluationResult,
                SearchStatistics = this.searchStatistics
            };
        }

        public void SetDepth(int depth)
        {
            this.searchDepth = depth;
        }

        public void SetAvailableMoves(IEnumerable<Move> moves)
        {
            this.allowedMoves = moves;
        }

        private IDictionary<Move, double> InitializeEvaluation()
        {
            var result = new Dictionary<Move, double>();

            var alpha = MinEvaluation;

            var max = double.NegativeInfinity;
            foreach (var child in this.rootNode.Children.Where(child => this.allowedMoves?.Contains(child.Key) ?? true))
            {
                this.searchStatistics.NodesTraversed++;

                max = Math.Max(max, this.GetPositionEvaluation(child.Value, this.searchDepth));
                alpha = Math.Max(alpha, max);

                result.Add(child.Key, max);
            }

            return result;
        }

        private double GetPositionEvaluation(MaximizingNode<double> maximizingNode, int depth)
        {
            this.searchStatistics.NodesTraversed++;

            if (maximizingNode.GameOver)
            {
                this.searchStatistics.TerminalNodeCount++;
                return MinEvaluation + this.searchDepth - depth;
            }

            if (depth == 0)
            {
                this.searchStatistics.TerminalNodeCount++;
                return maximizingNode.HeuristicValue;
            }

            return maximizingNode.Children.Values.Max(child => this.GetPositionEvaluation(child, depth));
        }

        private double GetPositionEvaluation(MinimizingNode<double> minimizingNode, int depth)
        {
            this.searchStatistics.NodesTraversed++;

            return minimizingNode.Children.Average(child => this.GetPositionEvaluation(child, depth - 1));
        }
    }
}