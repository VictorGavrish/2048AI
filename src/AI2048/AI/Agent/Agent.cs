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

            var heuristic = new VictorHeuristic();
            var rootNode = new MaximizingNode<double>(grid, heuristic);

            if (rootNode.GameOver)
            {
                throw new GameOverException();
            }

            var searchAnalyzer = new MinDurationSearchResultAnalyzer(Duration.FromMilliseconds(50));
            var exhaustiveDeathAvoider = new ExhaustiveDeathAvoider<double>(rootNode, 15);
            var alphaBetaMiniMaxer = new AlphaBetaMiniMaxer(rootNode, 3);
            var expectoMaxer = new ExpectoMaxer(rootNode, 3);
            var dynamicDepthSearcher = new DynamicDepthSearcherWrapper<ExpectoMaxer>(expectoMaxer, searchAnalyzer);

            //var strategy = new CombinedStrategy(exhaustiveDeathAvoider, alphaBetaMiniMaxer);
            var strategy = new SimpleStrategy(dynamicDepthSearcher);

            var decision = strategy.MakeDecision();

            return decision;
        }
    }
}