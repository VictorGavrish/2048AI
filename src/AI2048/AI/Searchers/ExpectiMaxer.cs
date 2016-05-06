namespace AI2048.AI.Searchers
{
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.Searchers.Models;
    using AI2048.AI.SearchTree;
    using AI2048.Game;

    using NodaTime;
    
    public class ExpectiMaxer : ISearcher
    {
        private static readonly double MinEvaluation = -1000000000;

        private readonly PlayerNode rootNode;

        private readonly SearchStatistics searchStatistics;

        private readonly int searchDepth;

        public ExpectiMaxer(PlayerNode rootNode, int minSearchDepth = 3)
        {
            this.rootNode = rootNode;
            this.searchDepth = minSearchDepth;

            this.searchStatistics = new SearchStatistics
            {
                RootNodeGrandchildren = this.rootNode.Children.Values.Sum(c => c.Children.Count())
            };
        }

        public SearchResult Search()
        {
            var startTime = SystemClock.Instance.Now;

            var evaluationResult = this.InitializeEvaluation();

            this.searchStatistics.SearchExhaustive = evaluationResult.All(kvp => kvp.Value <= MinEvaluation + this.searchDepth);
            this.searchStatistics.SearchDuration = SystemClock.Instance.Now - startTime;
            this.searchStatistics.SearchDepth = this.searchDepth;
            this.searchStatistics.KnownPlayerNodes = this.rootNode.SearchTree.KnownPlayerNodesBySum.Sum(kvp => kvp.Value.Count);
            this.searchStatistics.KnownComputerNodes = this.rootNode.SearchTree.KnownComputerNodesBySum.Sum(kvp => kvp.Value.Count);

            var result = new SearchResult
            {
                RootGrid = this.rootNode.Grid,
                BestMove = evaluationResult.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).First(),
                BestMoveEvaluation = evaluationResult.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Value).First(),
                SearcherName = nameof(ExpectiMaxer),
                MoveEvaluations = evaluationResult,
                SearchStatistics = this.searchStatistics
            };

            return result;
        }

        private IDictionary<Move, double> InitializeEvaluation()
        {
            return this.rootNode.Children
                .ToDictionary(
                    child => child.Key,
                    child => this.GetPositionEvaluation(child.Value, this.searchDepth));
        }

        private double GetPositionEvaluation(PlayerNode playerNode, int depth)
        {
            this.searchStatistics.NodeCount++;

            if (playerNode.GameOver)
            {
                this.searchStatistics.TerminalNodeCount++;
                return MinEvaluation + this.searchDepth - depth;
            }

            if (depth == 0)
            {
                this.searchStatistics.TerminalNodeCount++;
                return playerNode.HeuristicValue;
            }

            return playerNode.Children.Values.Max(child => this.GetPositionEvaluation(child, depth));
        }

        private double GetPositionEvaluation(ComputerNode computerNode, int depth)
        {
            this.searchStatistics.NodeCount++;

            var resultWith2 = computerNode.ChildrenWith2.Average(child => this.GetPositionEvaluation(child, depth - 1));
            var resultWith4 = computerNode.ChildrenWith4.Average(child => this.GetPositionEvaluation(child, depth - 1));

            var result = resultWith2 * 0.9 + resultWith4 * 0.1;
            
            return result;
        }
    }
}