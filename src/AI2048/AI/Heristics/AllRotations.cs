namespace AI2048.AI.Heristics
{
    using System;

    using AI2048.AI.SearchTree;

    public class AllRotations<T> : IHeuristic<T>
        where T : IComparable<T>
    {
        private readonly IHeuristic<T> heuristic;

        public AllRotations(IHeuristic<T> heuristic)
        {
            this.heuristic = heuristic;
        }

        public T Evaluate(Node<T> node)
        {
            var result = this.heuristic.Evaluate(node);

            for (var i = 0; i < 3; i++)
            {
                node = node.RotateCw();

                var rotationResult = this.heuristic.Evaluate(node);

                if (rotationResult.CompareTo(result) > 0)
                {
                    result = rotationResult;
                }
            }

            return result;
        }
    }
}