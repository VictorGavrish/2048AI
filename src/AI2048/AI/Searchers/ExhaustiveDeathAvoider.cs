namespace AI2048.AI.Searchers
{
    using System;
    using System.Linq;

    using AI2048.AI.Searchers.Models;
    using AI2048.AI.SearchTree;

    using NodaTime;

    public class ExhaustiveDeathAvoider<T> : ISearcher where T : IComparable<T>
    {
        public const double DeathEvaluation = -1000000000;

        private readonly MaximizingNode<T> rootNode;

        private readonly int safeFreeCellsCount;

        private readonly int searchDepth;

        private readonly SearchStatistics searchStatistics;

        public ExhaustiveDeathAvoider(MaximizingNode<T> rootNode, int searchDepth = 7, int safeFreeCellsCount = 3)
        {
            this.rootNode = rootNode;
            this.searchDepth = searchDepth;
            this.safeFreeCellsCount = safeFreeCellsCount;

            this.searchStatistics = new SearchStatistics
            {
                SearchDepth = searchDepth,
                RootNodeGrandchildren = rootNode.Children.Values.Sum(c => c.Children.Count()),
                SearchExhaustive = true
            };
        }

        public SearchResult Search()
        {
            var startTime = SystemClock.Instance.Now;

            var evaluationResult = this.rootNode.Children.ToDictionary(
                kvp => kvp.Key, 
                kvp => this.GetRiskOfDeath(kvp.Value, this.searchDepth) ? DeathEvaluation : 0);

            var endTime = SystemClock.Instance.Now;

            this.searchStatistics.SearchDuration = endTime - startTime;

            return new SearchResult
            {
                SearcherName = nameof(ExhaustiveDeathAvoider<T>),
                MoveEvaluations = evaluationResult,
                SearchStatistics = this.searchStatistics
            };
        }

        private bool GetRiskOfDeath(MaximizingNode<T> maximizingNode, int depth)
        {
            this.searchStatistics.NodesTraversed++;

            if (maximizingNode.GameOver)
            {
                this.searchStatistics.TerminalNodeCount++;
                return true;
            }

            if (depth == 0 || maximizingNode.EmptyCellCount >= this.safeFreeCellsCount
                || maximizingNode.EmptyCellCount >= depth)
            {
                this.searchStatistics.TerminalNodeCount++;
                return false;
            }

            return maximizingNode.Children.Values.All(child => this.GetRiskOfDeath(child, depth));
        }

        private bool GetRiskOfDeath(MinimizingNode<T> minimizingNode, int depth)
        {
            this.searchStatistics.NodesTraversed++;

            return minimizingNode.Children.Any(child => this.GetRiskOfDeath(child, depth - 1));
        }
    }
}