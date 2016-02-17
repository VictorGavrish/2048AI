namespace AI2048.AI.Searchers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.SearchTree;

    using NodaTime;

    public class AlphaBetaMiniMaxer : ISearcher
    {
        private const double DeathEvaluation = -1000000000;

        private const int MinNodeTraversed = 15000;

        private static readonly Duration MinTimeSpent = Duration.FromMilliseconds(200);

        private readonly MaximizingNode rootNode;

        private readonly AlphaBetaSearchStatistics searchStatistics;

        private readonly int searchDepth;

        private readonly int grandChildrenPerChild;

        public AlphaBetaMiniMaxer(MaximizingNode rootNode, int minSearchDepth = 5)
        {
            this.rootNode = rootNode;
            this.searchDepth = minSearchDepth;
            this.grandChildrenPerChild = (int)rootNode.Children.Values.Average(c => c.Children.Count());

            this.searchStatistics = new AlphaBetaSearchStatistics
            {
                SearchDepth = minSearchDepth,
                RootNodeGrandchildren = rootNode.Children.Values.Sum(c => c.Children.Count())
            };
        }

        public SearchResult Search()
        {
            var startTime = SystemClock.Instance.Now;

            var evaluationResult = this.rootNode.Children.ToDictionary(
                kvp => kvp.Key, 
                kvp =>
                this.GetPositionEvaluation(kvp.Value, this.searchDepth, double.NegativeInfinity, double.PositiveInfinity));

            if ((this.searchStatistics.NodesTraversed < MinNodeTraversed || SystemClock.Instance.Now - startTime < MinTimeSpent)
                && !evaluationResult.All(kvp => kvp.Value <= DeathEvaluation))
            {
                var betterSearcher = new AlphaBetaMiniMaxer(this.rootNode, this.searchDepth + 1);
                var results = betterSearcher.Search();
                results.SearchStatistics.SearchDuration = SystemClock.Instance.Now - startTime;
                return results;
            }

            this.searchStatistics.SearchDuration = SystemClock.Instance.Now - startTime;

            return new SearchResult
            {
                SearcherName = nameof(AlphaBetaMiniMaxer), 
                Evaluations = evaluationResult, 
                SearchStatistics = this.searchStatistics,
            };
        }

        private double GetPositionEvaluation(MaximizingNode maximizingNode, int depth, double alpha, double beta)
        {
            this.searchStatistics.NodesTraversed++;

            if (maximizingNode.GameOver)
            {
                this.searchStatistics.TerminalNodeCount++;
                return DeathEvaluation;
            }

            if (depth == 0)
            {
                this.searchStatistics.TerminalNodeCount++;
                return maximizingNode.HeuristicValue;
            }

            IEnumerable<MinimizingNode> children = maximizingNode.Children.Values;
            if (depth > 1)
            {
                children = children.OrderByDescending(c => c.HeuristicValue);
            }

            var max = double.NegativeInfinity;
            foreach (var child in children)
            {
                max = Math.Max(max, this.GetPositionEvaluation(child, depth - 1, alpha, beta));
                alpha = Math.Max(alpha, max);

                if (beta <= alpha)
                {
                    this.searchStatistics.MaximizingNodeBranchesPruned++;
                    this.searchStatistics.EstimatedNodesPruned += this.GetEstimatedNodesPruned(depth, true);
                    break;
                }
            }

            return max;
        }

        private double GetPositionEvaluation(MinimizingNode minimizingNode, int depth, double alpha, double beta)
        {
            this.searchStatistics.NodesTraversed++;

            if (depth == 0)
            {
                this.searchStatistics.TerminalNodeCount++;
                return minimizingNode.HeuristicValue;
            }

            var children = minimizingNode.Children;
            if (depth > 1)
            {
                children = children.OrderBy(c => c.HeuristicValue);
            }

            var min = double.PositiveInfinity;
            foreach (var child in children)
            {
                min = Math.Min(min, this.GetPositionEvaluation(child, depth - 1, alpha, beta));

                if (min <= DeathEvaluation)
                {
                    break;
                }

                beta = Math.Min(beta, min);

                if (beta <= alpha)
                {
                    this.searchStatistics.MinimizingNodeBranchesPruned++;
                    this.searchStatistics.EstimatedNodesPruned += this.GetEstimatedNodesPruned(depth, false);
                    break;
                }
            }

            return min;
        }

        private int GetEstimatedNodesPruned(int depth, bool maximizing)
        {
            var result = maximizing ? 2 : this.grandChildrenPerChild / 2;

            for (var i = depth - 2; i >= 0; i -= 2)
            {
                result *= maximizing ? this.grandChildrenPerChild : 3;
            }

            for (var i = depth - 1; i >= 0; i -= 2)
            {
                result *= maximizing ? 3 : this.grandChildrenPerChild;
            }

            return result;
        }
    }
}