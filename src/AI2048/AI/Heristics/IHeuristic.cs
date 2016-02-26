namespace AI2048.AI.Heristics
{
    using System;

    using AI2048.AI.SearchTree;

    public interface IHeuristic
    {
        double Evaluate(PlayerNode node);

        double Evaluate(ComputerNode node);
    }
}