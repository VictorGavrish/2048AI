namespace AI2048.AI.Heristics
{
    using AI2048.AI.SearchTree;

    public class OvolveHeuristic : IHeuristic
    {
        public double Evaluate(IPlayerNode node)
        {
            var result = Heuristics.GetMonotonicity(node) + Heuristics.GetEmptyCellEvalution(node.Grid);

            return result;
        }
    }
}