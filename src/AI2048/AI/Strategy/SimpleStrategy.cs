namespace AI2048.AI.Strategy
{
    using System;
    using System.Globalization;
    using System.Linq;

    using AI2048.AI.Searchers;
    using AI2048.Game;

    using NodaTime;

    public class SimpleStrategy : IStrategy
    {
        private readonly ISearcher searcher;

        public SimpleStrategy(ISearcher searcher)
        {
            this.searcher = searcher;
        }

        public Move MakeDecision()
        {
            var startTime = SystemClock.Instance.Now;

            Console.WriteLine("Start next move calculation...");

            var searchResults = this.searcher.Search();

            var elapsed = SystemClock.Instance.Now - startTime;

            Console.Clear();

            Console.WriteLine("End move calcualtion, time taken: {0}", elapsed.ToString("M:ss.fff", CultureInfo.InvariantCulture));
            Console.WriteLine();

            Console.WriteLine(searchResults);

            var decision = searchResults.BestMove;

            return decision;
        }
    }
}