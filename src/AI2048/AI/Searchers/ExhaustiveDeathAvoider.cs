namespace AI2048.AI.Searchers
{
    using System.Linq;

    using AI2048.AI.Searchers.Models;
    using AI2048.AI.SearchTree;

    using NodaTime;

    public class ExhaustiveDeathAvoider : ISearcher
    {
        public const double DeathEvaluation = -1000000000;

        private readonly MaximizingNode rootNode;

        private readonly int safeFreeCellsCount;

        private readonly int searchDepth;

        private readonly SearchStatistics searchStatistics;

        public ExhaustiveDeathAvoider(MaximizingNode rootNode, int searchDepth = 13, int safeFreeCellsCount = 3)
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
                kvp => this.GetRiskOfDeath(kvp.Value, this.searchDepth) ? (double)DeathEvaluation : 0);

            var endTime = SystemClock.Instance.Now;

            this.searchStatistics.SearchDuration = endTime - startTime;

            return new SearchResult
            {
                SearcherName = nameof(ExhaustiveDeathAvoider),
                MoveEvaluations = evaluationResult,
                SearchStatistics = this.searchStatistics
            };
        }

        private bool GetRiskOfDeath(MaximizingNode maximizingNode, int depth)
        {
            this.searchStatistics.NodesTraversed++;

            if (maximizingNode.GameOver)
            {
                this.searchStatistics.TerminalNodeCount++;
                return true;
            }

            if (depth == 0 || maximizingNode.EmptyCellCount >= this.safeFreeCellsCount
                || maximizingNode.EmptyCellCount >= depth * 2)
            {
                this.searchStatistics.TerminalNodeCount++;
                return false;
            }

            return maximizingNode.Children.Values.All(child => this.GetRiskOfDeath(child, depth - 1));
        }

        private bool GetRiskOfDeath(MinimizingNode minimizingNode, int depth)
        {
            this.searchStatistics.NodesTraversed++;

            return minimizingNode.Children.Any(child => this.GetRiskOfDeath(child, depth - 1));
        }
    }
}