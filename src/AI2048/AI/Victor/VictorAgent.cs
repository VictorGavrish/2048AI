namespace AI2048.AI.Victor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using AI2048.Game;

    public class VictorAgent : Agent
    {
        private MaximizingNode rootMaximizingNode;

        public List<Grid> History { get; } = new List<Grid>();

        private static int evaluations = 0;

        private static double EvaluatePosition(Node node)
        {
            Interlocked.Increment(ref evaluations);

            //var result = node.Monotonicity * 1500 + node.MaxValueEvaluation * 1000 + node.EmptyCellEvalution * 2000 + node.Smoothness * 200;

            var result = node.State.Sum(cell => Math.Pow(2, cell.Value * 2) * (cell.X + cell.Y)) + node.EmptyCellCount;

            //var result = node.State.Sum(cell => cell.Value * cell.Value * (cell.X + cell.Y)) + node.EmptyCellCount;

            return result;
        }

        public override Move MakeDecision(Grid state)
        {
            this.History.Add(state);

            var logState = new LogGrid(state.CloneMatrix());

            var heuristic = new Func<Node, double>(EvaluatePosition);

            this.rootMaximizingNode = //this.rootMaximizingNode?.Children.SelectMany(kvp => kvp.Value.Children).FirstOrDefault(n => n.State == state) ??
                new MaximizingNode(logState, heuristic);
            this.rootMaximizingNode.MakeRoot();

            if (this.rootMaximizingNode.GameOver)
            {
                throw new GameOverException();
            }
            
            var strategy = new CombinedStrategy(this.rootMaximizingNode);
            var decision = strategy.MakeDecision();

            Console.WriteLine($"Evaluations: {evaluations}");
            evaluations = 0;

            return decision;
        }
    }
}