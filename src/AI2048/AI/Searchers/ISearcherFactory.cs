namespace AI2048.AI.Searchers
{
    using AI2048.AI.SearchTree;

    public interface ISearcherFactory
    {
        ISearcher Build(ISearchTree searchTree);
    }
}