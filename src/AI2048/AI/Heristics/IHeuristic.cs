namespace AI2048.AI.Heristics
{
    using System;

    using AI2048.AI.SearchTree;

    public interface IHeuristic<T> 
        where T : IComparable<T>
    {
        T Evaluate(Node<T> node);
    }
}