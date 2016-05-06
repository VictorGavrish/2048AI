namespace AI2048.AI.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

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

            this.timings.Add(1, Duration.Zero);
            if (startingGrid.Flatten().Any(i => i == 2))
            {
                this.timings.Add(2, Duration.Zero);
            }
        }

        public List<LogarithmicGrid> History { get; } = new List<LogarithmicGrid>();

        private Instant start = SystemClock.Instance.Now;

        private Dictionary<int, Duration> timings = new Dictionary<int, Duration>();

        public Move MakeDecision()
        {
            if (this.searchTree.RootNode.GameOver)
            {
                throw new GameOverException();
            }

            this.PrintTimings();

            ISearcher searcher = new ProbabilityLimitedExpectiMaxer(this.searchTree.RootNode);

            var startTime = SystemClock.Instance.Now;

            Console.WriteLine("Start next move calculation...");

            var searchResults = searcher.Search();

            var elapsed = SystemClock.Instance.Now - startTime;

            Console.Clear();

            Console.WriteLine("End move calcualtion, time taken: {0}", elapsed.ToString("ss.fff", CultureInfo.InvariantCulture));
            Console.WriteLine();

            Console.WriteLine(searchResults);

            var decision = searchResults.BestMove;

            return decision;
        }

        private void PrintTimings()
        {
            var lastMax = this.timings.Keys.Max();

            if (this.searchTree.RootNode.Grid.Flatten().Any(i => i == lastMax + 1))
            {
                this.timings.Add(lastMax + 1, SystemClock.Instance.Now - this.start);
            }

            foreach (var kvp in this.timings)
            {
                var humanValue = kvp.Key == 0 ? 0 : 2 << (kvp.Key - 1);
                Console.WriteLine($"{humanValue, 5}: {kvp.Value.ToString("mm:ss.fff", CultureInfo.InvariantCulture)}");
            }

            Console.WriteLine($"Time passed this game: {(SystemClock.Instance.Now - this.start).ToString("mm:ss.fff", CultureInfo.InvariantCulture)}");
        }

        public void UpdateGrid(LogarithmicGrid grid)
        {
            this.History.Add(grid);
            this.searchTree.MoveRoot(grid);
        }
    }
}