namespace AI2048.AI.SearchTree
{
    using System.Collections.Generic;

    using AI2048.Game;

    public interface IComputerNode
    {
        IEnumerable<IPlayerNode> ChildrenWith2 { get; }

        IEnumerable<IPlayerNode> ChildrenWith4 { get; }

        IEnumerable<IPlayerNode> Children { get; }

        int Sum { get; }

        int EmptyCellCount { get; }

        LogarithmicGrid Grid { get; }

        SearchTree SearchTree { get; }
    }
}