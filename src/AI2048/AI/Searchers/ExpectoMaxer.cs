namespace AI2048.AI.Searchers
{
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.Searchers.Models;
    using AI2048.AI.SearchTree;
    using AI2048.Game;

    using NodaTime;
    
    public class ExpectoMaxer : IConfigurableDepthSearcher, IConfigurableMovesSearcher
    {
        private static readonly double MinEvaluation = -1000000000;

        private readonly MaximizingNode rootNode;

        private readonly SearchStatistics searchStatistics;

        private IEnumerable<Move> allowedMoves;

        private int searchDepth;

        public ExpectoMaxer(MaximizingNode rootNode, int minSearchDepth = 3)
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
            this.searchStatistics.KnownPlayerNodes = this.rootNode.SearchTree.KnownPlayerNodes.Count;
            this.searchStatistics.KnownComputerNodes = this.rootNode.SearchTree.KnownComputerNodes.Count;

            var result = new SearchResult
            {
                RootGrid = this.rootNode.Grid,
                BestMove = evaluationResult.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).First(),
                BestMoveEvaluation = evaluationResult.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Value).First(),
                SearcherName = nameof(ExpectoMaxer),
                MoveEvaluations = evaluationResult,
                SearchStatistics = this.searchStatistics
            };

            return result;
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
            return this.rootNode.Children
                .Where(kvp => this.allowedMoves?.Contains(kvp.Key) ?? true)
                .ToDictionary(
                    child => child.Key,
                    child => this.GetPositionEvaluation(child.Value, this.searchDepth));
        }

        private double GetPositionEvaluation(MaximizingNode maximizingNode, int depth)
        {
            this.searchStatistics.NodeCount++;

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

        private double GetPositionEvaluation(MinimizingNode minimizingNode, int depth)
        {
            this.searchStatistics.NodeCount++;

            var resultWith2 = minimizingNode.ChildrenWith2.Average(child => this.GetPositionEvaluation(child, depth - 1));
            var resultWith4 = minimizingNode.ChildrenWith4.Average(child => this.GetPositionEvaluation(child, depth - 1));

            var result = resultWith2 * 0.9 + resultWith4 * 0.1;
            
            return result;
        }
    }
}