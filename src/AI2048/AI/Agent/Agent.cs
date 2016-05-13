namespace AI2048.AI.Agent
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using AI2048.AI.Heristics;
    using AI2048.AI.Searchers;
    using AI2048.AI.Searchers.Models;
    using AI2048.AI.SearchTree;
    using AI2048.Game;

    using NodaTime;

    public class Agent
    {
        private readonly SearchTree searchTree;

        public Agent(LogarithmicGrid startingGrid)
        {
            this.searchTree = new SearchTree(new VictorHeuristic(), startingGrid);

            this.Timings.Add(1, Duration.Zero);
            if (startingGrid.Flatten().Any(i => i == 2))
            {
                this.Timings.Add(2, Duration.Zero);
            }
        }

        public List<LogarithmicGrid> History { get; } = new List<LogarithmicGrid>();

        public Instant Start { get; } = SystemClock.Instance.Now;

        public Dictionary<int, Duration> Timings { get; } = new Dictionary<int, Duration>();

        public SearchResult MakeDecision()
        {
            if (this.searchTree.RootNode.GameOver)
            {
                throw new GameOverException();
            }

            var lastMax = this.Timings.Keys.Max();
            if (this.searchTree.RootNode.Grid.Flatten().Any(i => i == lastMax + 1))
            {
                this.Timings.Add(lastMax + 1, SystemClock.Instance.Now - this.Start);
            }

            ISearcher searcher = new ProbabilityLimitedExpectiMaxer(this.searchTree);

            var searchResults = searcher.Search();

            return searchResults;
        }

        public void UpdateGrid(LogarithmicGrid grid)
        {
            this.History.Add(grid);
            this.searchTree.MoveRoot(grid);
        }
    }
}