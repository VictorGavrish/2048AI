namespace AI2048.AI.SearchTree
{
    using System.Collections.Generic;

    using AI2048.Game;

    public interface IPlayerNode
    {
        double HeuristicValue { get; }

        IDictionary<Move, IComputerNode> Children { get; }

        IEnumerable<KeyValuePair<Move, LogarithmicGrid>> PossibleStates { get; }

        bool GameOver { get; }

        int Sum { get; }

        int EmptyCellCount { get; }

        LogarithmicGrid Grid { get; }

        SearchTree SearchTree { get; }
    }
}