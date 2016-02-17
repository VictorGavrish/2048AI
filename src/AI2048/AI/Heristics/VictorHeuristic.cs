namespace AI2048.AI.Heristics
{
    using System;
    using System.Linq;

    using AI2048.AI.SearchTree;

    public class VictorHeuristic : IHeuristic
    {
        public double Evaluate(Node node)
        {
            var result = node.Grid.Sum(cell => Math.Pow(2, cell.Value * 2) * (cell.X + cell.Y)) + node.EmptyCellCount;

            return result;
        }
    }
}