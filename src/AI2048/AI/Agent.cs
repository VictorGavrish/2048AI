namespace AI2048.AI
{
    using System;

    using AI2048.Game;

    public abstract class Agent
    {
        protected Move[] Moves { get; } = { Move.Up, Move.Left, Move.Down, Move.Right };

        protected readonly Func<Grid, long> Heuristic;

        protected Agent(Func<Grid, long> heurstk)
        {
            this.Heuristic = heurstk;
        }

        public abstract Move MakeDecision(Grid state);
    }
}