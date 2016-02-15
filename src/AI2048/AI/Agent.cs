namespace AI2048.AI
{
    using AI2048.Game;

    public abstract class Agent
    {
        protected static Move[] Moves { get; } = { Move.Up, Move.Left, Move.Down, Move.Right };

        public abstract Move MakeDecision(Grid state);
    }
}