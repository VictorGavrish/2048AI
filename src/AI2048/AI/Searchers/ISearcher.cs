namespace AI2048.AI.Searchers
{
    using AI2048.AI.Searchers.Models;

    public interface ISearcher
    {
        SearchResult Search();
    }
}