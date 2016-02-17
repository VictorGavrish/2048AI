namespace AI2048.AI.Searchers
{
    using System.Collections.Generic;

    using AI2048.AI.Searchers.Models;
    using AI2048.AI.Searchers.ResultAnalyzers;
    using AI2048.Game;

    public class DynamicDepthSearcherWrapper<T> : IConfigurableMovesSearcher where T : IConfigurableDepthSearcher, IConfigurableMovesSearcher
    {
        private readonly T searcher;

        private readonly ISearchResultAnalyzer searchResultAnalyzer;

        public DynamicDepthSearcherWrapper(T searcher, ISearchResultAnalyzer searchResultAnalyzer)
        {
            this.searcher = searcher;
            this.searchResultAnalyzer = searchResultAnalyzer;
        }

        public SearchResult Search()
        {
            var result = this.searcher.Search();

            while (this.searchResultAnalyzer.ShouldIncreaseSearchDepth(result))
            {
                this.searcher.SetDepth(result.SearchStatistics.SearchDepth + 1);
                result = this.searcher.Search();
            }

            return result;
        }

        public void SetAvailableMoves(IEnumerable<Move> moves)
        {
            this.searcher.SetAvailableMoves(moves);
        }
    }
}