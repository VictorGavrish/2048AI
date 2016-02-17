namespace AI2048.AI.Searchers
{
    public interface IConfigurableDepthSearcher : ISearcher
    {
        void SetDepth(int depth);
    }
}