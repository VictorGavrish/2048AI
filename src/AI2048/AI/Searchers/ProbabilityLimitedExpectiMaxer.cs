namespace AI2048.AI.Searchers
{
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.Searchers.Models;
    using AI2048.AI.SearchTree;
    using AI2048.Game;

    using NodaTime;

    public class ProbabilityLimitedExpectiMaxer : ISearcher
    {
        private const double ProbabilityOf2 = 0.9;

        private const double ProbabilityOf4 = 0.1;

        private static readonly double MinEvaluation = -1000000000;

        private readonly PlayerNode rootNode;

        private readonly SearchStatistics searchStatistics;

        private IEnumerable<Move> allowedMoves;

        private readonly double minProbability;

        private readonly int maxSearchDepth;

        public ProbabilityLimitedExpectiMaxer(PlayerNode rootNode, double minProbability = 0.01, int maxSearchDepth = 8)
        {
            this.rootNode = rootNode;
            this.minProbability = minProbability;
            this.maxSearchDepth = maxSearchDepth;

            this.searchStatistics = new SearchStatistics
            {
                RootNodeGrandchildren = this.rootNode.Children.Values.Sum(c => c.Children.Count())
            };
        }

        public SearchResult Search()
        {
            var startTime = SystemClock.Instance.Now;
            var knownPlayerNodesStart = this.rootNode.SearchTree.KnownPlayerNodes.Count;
            var knownComputerNodesStart = this.rootNode.SearchTree.KnownComputerNodes.Count;

            var evaluationResult = this.InitializeEvaluation();

            this.searchStatistics.SearchExhaustive = evaluationResult.All(kvp => kvp.Value <= MinEvaluation + this.maxSearchDepth);
            this.searchStatistics.SearchDepth = this.maxSearchDepth;
            this.searchStatistics.SearchDuration = SystemClock.Instance.Now - startTime;
            this.searchStatistics.KnownPlayerNodes = this.rootNode.SearchTree.KnownPlayerNodes.Count
                                                     - knownPlayerNodesStart;
            this.searchStatistics.KnownComputerNodes = this.rootNode.SearchTree.KnownComputerNodes.Count
                                                       - knownComputerNodesStart;

            var result = new SearchResult
            {
                RootGrid = this.rootNode.Grid,
                BestMove = evaluationResult.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).First(),
                BestMoveEvaluation = evaluationResult.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Value).First(),
                SearcherName = nameof(ProbabilityLimitedExpectiMaxer),
                MoveEvaluations = evaluationResult,
                SearchStatistics = this.searchStatistics
            };

            return result;
        }

        public void SetAvailableMoves(IEnumerable<Move> moves)
        {
            this.allowedMoves = moves;
        }

        private IDictionary<Move, double> InitializeEvaluation()
        {
            return this.rootNode.Children
                .Where(kvp => this.allowedMoves?.Contains(kvp.Key) ?? true)
                .ToDictionary(
                    child => child.Key,
                    child => this.GetPositionEvaluation(child.Value, this.maxSearchDepth, 1));
        }

        private double GetPositionEvaluation(PlayerNode playerNode, int depth, double probability)
        {
            this.searchStatistics.NodeCount++;

            if (playerNode.GameOver)
            {
                this.searchStatistics.TerminalNodeCount++;
                return MinEvaluation + this.maxSearchDepth - depth;
            }

            if (probability < this.minProbability || depth == 0)
            {
                this.searchStatistics.TerminalNodeCount++;
                return playerNode.HeuristicValue;
            }

            return playerNode.Children.Values.Max(child => this.GetPositionEvaluation(child, depth, probability));
        }

        private double GetPositionEvaluation(ComputerNode computerNode, int depth, double probability)
        {
            this.searchStatistics.NodeCount++;

            var childrenCount = computerNode.ChildrenWith2.Count();

            var resultWith2 = computerNode.ChildrenWith2.Average(child => 
                this.GetPositionEvaluation(child, depth - 1, probability * ProbabilityOf2 / childrenCount));
            var resultWith4 = computerNode.ChildrenWith4.Average(child => 
                this.GetPositionEvaluation(child, depth - 1, probability * ProbabilityOf4 / childrenCount));

            var result = resultWith2 * ProbabilityOf2 + resultWith4 * ProbabilityOf4;

            return result;
        }
    }
}