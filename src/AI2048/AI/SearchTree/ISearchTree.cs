namespace AI2048.AI.SearchTree
{
    using AI2048.Game;

    public interface ISearchTree
    {
        IPlayerNode RootNode { get; }

        int KnownPlayerNodeCount { get; }

        int KnownComputerNodeCount { get; }

        void MoveRoot(LogarithmicGrid newGrid);
    }
}