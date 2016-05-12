namespace AI2048.AI.SearchTree
{
    using System.Collections.Generic;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    public interface ISearchTree
    {
        IHeuristic Heuristic { get; }

        IPlayerNode RootNode { get; }

        IDictionary<int, IDictionary<LogarithmicGrid, IPlayerNode>> KnownPlayerNodesBySum { get; }

        IDictionary<int, IDictionary<LogarithmicGrid, IComputerNode>> KnownComputerNodesBySum { get; }

        void MoveRoot(LogarithmicGrid newGrid);
    }
}