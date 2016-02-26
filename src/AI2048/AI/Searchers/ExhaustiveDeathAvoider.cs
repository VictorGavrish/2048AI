namespace AI2048.AI.Searchers
{
    using System;
    using System.Linq;

    using AI2048.AI.Searchers.Models;
    using AI2048.AI.SearchTree;

    using NodaTime;

    public class ExhaustiveDeathAvoider : ISearcher
    {
        public const double DeathEvaluation = -1000000000;

        private readonly PlayerNode rootNode;

        private readonly int safeFreeCellsCount;

        private readonly int searchDepth;

        private readonly SearchStatistics searchStatistics;

        public ExhaustiveDeathAvoider(PlayerNode rootNode, int searchDepth = 7, int safeFreeCellsCount = 3)
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
                SearcherName = nameof(ExhaustiveDeathAvoider),
                MoveEvaluations = evaluationResult,
                SearchStatistics = this.searchStatistics
            };
        }

        private bool GetRiskOfDeath(PlayerNode playerNode, int depth)
        {
            this.searchStatistics.NodeCount++;

            if (playerNode.GameOver)
            {
                this.searchStatistics.TerminalNodeCount++;
                return true;
            }

            if (depth == 0 || playerNode.EmptyCellCount >= this.safeFreeCellsCount
                || playerNode.EmptyCellCount >= depth)
            {
                this.searchStatistics.TerminalNodeCount++;
                return false;
            }

            return playerNode.Children.Values.All(child => this.GetRiskOfDeath(child, depth));
        }

        private bool GetRiskOfDeath(ComputerNode computerNode, int depth)
        {
            this.searchStatistics.NodeCount++;

            return computerNode.Children.Any(child => this.GetRiskOfDeath(child, depth - 1));
        }
    }
}