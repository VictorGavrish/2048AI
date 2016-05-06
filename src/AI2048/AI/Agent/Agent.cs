namespace AI2048.AI.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using AI2048.AI.Heristics;
    using AI2048.AI.Searchers;
    using AI2048.AI.SearchTree;
    using AI2048.Game;

    using NodaTime;

    public class Agent
    {
        private readonly SearchTree searchTree;

        public Agent(LogarithmicGrid startingGrid)
        {
            this.searchTree = new SearchTree(new VictorHeuristic(), startingGrid);
        }

        public List<LogarithmicGrid> History { get; } = new List<LogarithmicGrid>();

        public Move MakeDecision()
        {
            if (this.searchTree.RootNode.GameOver)
            {
                throw new GameOverException();
            }

            var probabilityLimitedExpectoMaxer = new ProbabilityLimitedExpectiMaxer(this.searchTree.RootNode);

            var startTime = SystemClock.Instance.Now;

            Console.WriteLine("Start next move calculation...");

            var searchResults = probabilityLimitedExpectoMaxer.Search();

            var elapsed = SystemClock.Instance.Now - startTime;

            Console.Clear();

            Console.WriteLine("End move calcualtion, time taken: {0}", elapsed.ToString("ss.fff", CultureInfo.InvariantCulture));
            Console.WriteLine();

            Console.WriteLine(searchResults);

            var decision = searchResults.BestMove;

            return decision;
        }

        public void UpdateGrid(LogarithmicGrid grid)
        {
            this.History.Add(grid);
            this.searchTree.MoveRoot(grid);
        }
    }
}