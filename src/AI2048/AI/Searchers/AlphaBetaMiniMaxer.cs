namespace AI2048.AI.Searchers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.Searchers.Models;
    using AI2048.AI.SearchTree;
    using AI2048.Game;

    using NodaTime;

    public class AlphaBetaMiniMaxer : IConfigurableDepthSearcher, IConfigurableMovesSearcher
    {
        private const double MinEvaluation = -1000000000000;

        private const double MaxEvaluation = 1000000000000;
        
        private readonly MaximizingNode<double> rootNode;

        private AlphaBetaSearchStatistics searchStatistics;

        private IEnumerable<Move> allowedMoves;

        private int searchDepth;

        public AlphaBetaMiniMaxer(MaximizingNode<double> rootNode, int minSearchDepth = 3)
        {
            this.rootNode = rootNode;
            this.searchDepth = minSearchDepth;
        }

        public SearchResult Search()
        {
            this.searchStatistics = new AlphaBetaSearchStatistics
            {
                RootNodeGrandchildren = this.rootNode.Children.Values.Sum(c => c.Children.Count())
            };

            var startTime = SystemClock.Instance.Now;

            var evaluationResult = this.InitializeEvaluation();

            this.searchStatistics.SearchExhaustive = evaluationResult.All(kvp => kvp.Value <= MinEvaluation + this.searchDepth);
            this.searchStatistics.SearchDuration = SystemClock.Instance.Now - startTime;
            this.searchStatistics.SearchDepth = this.searchDepth;

            return new SearchResult
            {
                SearcherName = nameof(AlphaBetaMiniMaxer),
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
            foreach (var child in this.rootNode.Children
                .Where(child => this.allowedMoves?.Contains(child.Key) ?? true)
                .OrderByDescending(child => child.Value.HeuristicValue))
            {
                this.searchStatistics.NodesTraversed++;

                max = Math.Max(max, this.GetPositionEvaluation(child.Value, this.searchDepth, alpha, MaxEvaluation));
                alpha = Math.Max(alpha, max);

                result.Add(child.Key, max);
            }

            return result;
        }

        private double GetPositionEvaluation(MaximizingNode<double> maximizingNode, int depth, double alpha, double beta)
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

            IEnumerable<MinimizingNode<double>> children = maximizingNode.Children.Values;
            if (depth > 1)
            {
                children = children.OrderByDescending(c => c.HeuristicValue);
            }

            var max = double.NegativeInfinity;
            foreach (var child in children)
            {
                max = Math.Max(max, this.GetPositionEvaluation(child, depth, alpha, beta));
                alpha = Math.Max(alpha, max);

                if (beta <= alpha)
                {
                    this.searchStatistics.MaximizingNodeBranchesPruned++;
                    break;
                }
            }

            return max;
        }

        private double GetPositionEvaluation(MinimizingNode<double> minimizingNode, int depth, double alpha, double beta)
        {
            this.searchStatistics.NodesTraversed++;

            var children = minimizingNode.Children;
            if (depth > 1)
            {
                children = children.OrderBy(c => c.HeuristicValue);
            }

            var min = double.PositiveInfinity;
            foreach (var child in children)
            {
                min = Math.Min(min, this.GetPositionEvaluation(child, depth - 1, alpha, beta));

                beta = Math.Min(beta, min);

                if (beta <= alpha)
                {
                    this.searchStatistics.MinimizingNodeBranchesPruned++;
                    break;
                }
            }

            return min;
        }
    }
}