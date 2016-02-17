namespace AI2048.AI.Heristics
{
    using System;

    using AI2048.AI.SearchTree;

    public class AllRotations : IHeuristic
    {
        private readonly IHeuristic heuristic;

        public AllRotations(IHeuristic heuristic)
        {
            this.heuristic = heuristic;
        }

        public double Evaluate(Node node)
        {
            var result = double.NegativeInfinity;

            var i = 0;
            do
            {
                result = Math.Max(result, this.heuristic.Evaluate(node));

                node = node.RotateCw();
            }
            while (i++ < 4);

            return result;
        }
    }
}