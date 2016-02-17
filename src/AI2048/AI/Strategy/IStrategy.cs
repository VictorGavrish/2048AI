namespace AI2048.AI.Strategy
{
    using AI2048.Game;

    public interface IStrategy
    {
        Move MakeDecision();
    }
}