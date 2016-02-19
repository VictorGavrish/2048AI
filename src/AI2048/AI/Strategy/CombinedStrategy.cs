namespace AI2048.AI.Strategy
{
    using System;
    using System.Globalization;
    using System.Linq;

    using AI2048.AI.Searchers;
    using AI2048.AI.Searchers.Models;
    using AI2048.Game;

    using NodaTime;

    public class CombinedStrategy : IStrategy
    {
        private readonly ISearcher moveFilteringSearcher;

        private readonly IConfigurableMovesSearcher searcher;

        public CombinedStrategy(ISearcher moveFilteringSearcher, IConfigurableMovesSearcher searcher)
        {
            this.moveFilteringSearcher = moveFilteringSearcher;
            this.searcher = searcher;
        }

        public Move MakeDecision()
        {
            var startTime = SystemClock.Instance.Now;

            Console.WriteLine("Start next move calculation...");

            var searchResults = this.GetSearchResult();

            var elapsed = SystemClock.Instance.Now - startTime;

            Console.WriteLine("End move calcualtion, time taken: {0}", elapsed.ToString("M:ss.fff", CultureInfo.InvariantCulture));
            Console.WriteLine();

            var decision =
                searchResults.MoveEvaluations.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).First();

            return decision;
        }

        private SearchResult GetSearchResult()
        {
            var filteringSearchResults = this.moveFilteringSearcher.Search();

            var safeMoves = filteringSearchResults.MoveEvaluations
                .Where(kvp => kvp.Value > ExhaustiveDeathAvoider<double>.DeathEvaluation)
                .Select(kvp => kvp.Key)
                .ToArray();

            if (safeMoves.Any())
            {
                this.searcher.SetAvailableMoves(safeMoves);
            }

            if (safeMoves.Length == 1)
            {
                Console.Clear();
                Console.WriteLine(filteringSearchResults);
                return filteringSearchResults;
            }

            var searchResult = this.searcher.Search();

            Console.Clear();
            Console.WriteLine(filteringSearchResults);
            Console.WriteLine(searchResult);

            return searchResult;
        }
    }
}