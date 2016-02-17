namespace AI2048.AI.Strategy
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using AI2048.AI.Searchers;
    using AI2048.Game;

    using NodaTime;

    public class CombinedStrategy
    {
        private readonly IEnumerable<ISearcher> searchers;

        public CombinedStrategy(IEnumerable<ISearcher> searchers)
        {
            this.searchers = searchers;
        }

        public Move MakeDecision()
        {
            var startTime = SystemClock.Instance.Now;

            Console.WriteLine("Start move calculation");

            var searchResults = this.GetSearchResults();

            var evaluationDictionary = CombineSearchResults(searchResults);

            var endTime = SystemClock.Instance.Now;

            Console.Clear();

            Console.WriteLine("End move calcualtion, time taken: {0}", endTime - startTime);

            Console.WriteLine();
            
            foreach (var searchResult in searchResults)
            {
                Console.WriteLine(searchResult);
            }

            var decision = evaluationDictionary.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).First();

            return decision;
        }

        private IList<SearchResult> GetSearchResults()
        {
            var tasks = this.searchers.Select(searcher => Task.Factory.StartNew(searcher.Search));

            return tasks.Select(task => task.Result).ToList();
        }

        private static IDictionary<Move, double> CombineSearchResults(IEnumerable<SearchResult> searchResults)
        {
            var result = new Dictionary<Move, double>
            {
                { Move.Up, double.NegativeInfinity }, 
                { Move.Down, double.NegativeInfinity }, 
                { Move.Left, double.NegativeInfinity }, 
                { Move.Right, double.NegativeInfinity }
            };

            foreach (var searchResult in searchResults)
            {
                foreach (var key in searchResult.Evaluations.Keys)
                {
                    result[key] = double.IsNegativeInfinity(result[key]) ? searchResult.Evaluations[key] : result[key] + searchResult.Evaluations[key];
                }
            }

            return result;
        }
    }
}