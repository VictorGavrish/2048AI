namespace AI2048.AI.Heristics
{
    using System;

    using AI2048.AI.SearchTree;

    public interface IHeuristic
    {
        double Evaluate(MaximizingNode node);

        double Evaluate(MinimizingNode node);
    }
}