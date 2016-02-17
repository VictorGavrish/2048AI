namespace AI2048.AI.Agent
{
    using System;
    using System.Collections.Generic;

    using AI2048.AI.Heristics;
    using AI2048.AI.Searchers;
    using AI2048.AI.SearchTree;
    using AI2048.AI.Strategy;
    using AI2048.Game;

    public class Agent
    {
        public List<LogarithmicGrid> History { get; } = new List<LogarithmicGrid>();

        public Move MakeDecision(LogarithmicGrid grid)
        {
            Console.WriteLine(grid.ToString());
            
            this.History.Add(grid);

            var heuristic = new VictorHeuristic();

            var rootNode = new MaximizingNode(grid, heuristic);
            if (rootNode.GameOver)
            {
                throw new GameOverException();
            }

            var searchers = new ISearcher[]
            {
                new AlphaBetaMiniMaxer(new MaximizingNode(grid, heuristic)),
                new ExhaustiveDeathAvoider(new MaximizingNode(grid, heuristic))
            };

            var strategy = new CombinedStrategy(searchers);
            var decision = strategy.MakeDecision();

            return decision;
        }
    }
}