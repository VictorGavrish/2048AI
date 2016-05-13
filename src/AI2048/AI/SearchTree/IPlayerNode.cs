namespace AI2048.AI.SearchTree
{
    using System.Collections.Generic;

    using AI2048.Game;

    public interface IPlayerNode
    {
        IDictionary<Move, IComputerNode> Children { get; }

        bool GameOver { get; }

        double HeuristicValue { get; }

        LogarithmicGrid Grid { get; }
    }
}