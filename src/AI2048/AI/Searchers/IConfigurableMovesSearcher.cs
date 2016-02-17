namespace AI2048.AI.Searchers
{
    using System.Collections.Generic;

    using AI2048.Game;

    public interface IConfigurableMovesSearcher : ISearcher
    {
        void SetAvailableMoves(IEnumerable<Move> moves);
    }
}