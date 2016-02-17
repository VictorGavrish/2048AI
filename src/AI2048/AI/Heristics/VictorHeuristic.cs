namespace AI2048.AI.Heristics
{
    using System;
    using System.Linq;

    using AI2048.AI.SearchTree;

    public class VictorHeuristic : IHeuristic
    {
        public double Evaluate(Node node)
        {
            double result = node.EmptyCellCount;

            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    result += Math.Pow(2, node.Grid[x, y] * 2) * (x + y);
                }
            }

            return result;
        }
    }
}