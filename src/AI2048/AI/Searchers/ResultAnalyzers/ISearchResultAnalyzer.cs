namespace AI2048.AI.Searchers.ResultAnalyzers
{
    using AI2048.AI.Searchers.Models;

    public interface ISearchResultAnalyzer
    {
        bool ShouldIncreaseSearchDepth(SearchResult searchResult);
    }
}