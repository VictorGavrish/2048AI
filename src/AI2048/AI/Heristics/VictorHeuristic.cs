namespace AI2048.AI.Heristics
{
    using System.Runtime.CompilerServices;

    using AI2048.AI.SearchTree;

    public class VictorHeuristic : IHeuristic<double>
    {
        public double Evaluate(Node<double> node)
        {
            var result = node.EmptyCellCount;

            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    result += TwoToThePower(node.Grid[x, y] * 2) * (x + y);
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int TwoToThePower(int power) => 1 << power;
    }
}