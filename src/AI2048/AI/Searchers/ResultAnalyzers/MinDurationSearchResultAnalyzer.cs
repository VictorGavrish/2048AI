namespace AI2048.AI.Searchers.ResultAnalyzers
{
    using AI2048.AI.Searchers.Models;

    using NodaTime;

    public class MinDurationSearchResultAnalyzer : ISearchResultAnalyzer
    {
        private readonly Duration minSearchDuration;

        public MinDurationSearchResultAnalyzer(Duration minSearchDuration)
        {
            this.minSearchDuration = minSearchDuration;
        }

        public bool ShouldIncreaseSearchDepth(SearchResult searchResult)
        {
            return !searchResult.SearchStatistics.SearchExhaustive && searchResult.SearchStatistics.SearchDuration < this.minSearchDuration;
        }
    }
}