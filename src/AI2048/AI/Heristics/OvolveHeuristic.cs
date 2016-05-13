namespace AI2048.AI.Heristics
{
    using AI2048.AI.SearchTree;

    public class OvolveHeuristic : IHeuristic
    {
        public double Evaluate(IPlayerNode node)
        {
            if (node.GameOver)
            {
                return -10000000;
            }

            var result = Heuristics.GetMonotonicity(node.Grid) * 10 + Heuristics.GetEmptyCellCount(node.Grid);

            return result;
        }
    }
}