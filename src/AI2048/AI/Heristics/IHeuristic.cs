namespace AI2048.AI.Heristics
{
    using AI2048.AI.SearchTree;

    public interface IHeuristic
    {
        double Evaluate(Node node);
    }
}