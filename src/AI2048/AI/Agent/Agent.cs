namespace AI2048.AI.Agent
{
    using System;
    using System.Collections.Generic;

    using AI2048.AI.Heristics;
    using AI2048.AI.Searchers;
    using AI2048.AI.Searchers.ResultAnalyzers;
    using AI2048.AI.SearchTree;
    using AI2048.AI.Strategy;
    using AI2048.Game;

    using NodaTime;

    public class Agent
    {
        public List<LogarithmicGrid> History { get; } = new List<LogarithmicGrid>();

        public Move MakeDecision(LogarithmicGrid grid)
        {
            Console.WriteLine(grid.ToString());
            
            this.History.Add(grid);

            var heuristic = new AllRotations(new VictorHeuristic());

            var rootNode = new MaximizingNode(grid, heuristic);
            if (rootNode.GameOver)
            {
                throw new GameOverException();
            }

            var searchAnalyzer = new MinDurationSearchResultAnalyzer(Duration.FromMilliseconds(400));
            var exhaustiveDeathAvoider = new ExhaustiveDeathAvoider(rootNode);
            var alphaBetaMiniMaxer = new AlphaBetaMiniMaxer(rootNode);
            var dynamicDepthSearcher = new DynamicDepthSearcherWrapper<AlphaBetaMiniMaxer>(alphaBetaMiniMaxer, searchAnalyzer);

            var strategy = new CombinedStrategy(exhaustiveDeathAvoider, dynamicDepthSearcher);

            var decision = strategy.MakeDecision();

            return decision;
        }
    }
}