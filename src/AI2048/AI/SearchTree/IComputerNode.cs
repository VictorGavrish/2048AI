namespace AI2048.AI.SearchTree
{
    using System.Collections.Generic;

    using AI2048.Game;

    public interface IComputerNode
    {
        IEnumerable<IPlayerNode> ChildrenWith2 { get; }

        IEnumerable<IPlayerNode> ChildrenWith4 { get; }

        IEnumerable<IPlayerNode> Children { get; }

        LogarithmicGrid Grid { get; }
    }
}