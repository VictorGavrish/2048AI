namespace AI2048.AI.Searchers
{
    using AI2048.AI.SearchTree;

    public class ProbabilityLimitedExpectiMaxerFactory : ISearcherFactory
    {
        public ISearcher Build(ISearchTree searchTree)
        {
            return new ProbabilityLimitedExpectiMaxer(searchTree);
        }
    }
}