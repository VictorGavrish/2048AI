namespace AI2048.AI.Victor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AI2048.Game;

    public class CombinedStrategy
    {
        private const int RiskOfDeathSearchDepth = 13;

        private const int SafeFreeCellsCount = 3;

        private const long DeathEvaluation = -1000000000;

        private readonly MaximizingNode rootMaximizingNode;

        public CombinedStrategy(MaximizingNode rootMaximizingNode)
        {
            this.rootMaximizingNode = rootMaximizingNode;
        }

        public Move MakeDecision()
        {
            var sw = new Stopwatch();
            Console.WriteLine("Start calculation");
            sw.Start();
            var evaluationDictionary = this.GetEvaluationDictionary();
            sw.Stop();
            LogEvaluation(this.rootMaximizingNode, evaluationDictionary);
            Console.WriteLine("End calcualtion, taken: {0}", sw.Elapsed);
            Console.WriteLine($"Max pruned: {maxCount}");
            Console.WriteLine($"Max pruned: {minCount}");

            maxCount = minCount = 0;

            var decision = evaluationDictionary.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).First();

            return decision;
        }

        private IDictionary<Move, double> GetPositionEvaluation()
        {
            var searchDepth = this.GetPositionEvaluationSearchDepth();

            var positionEvaluationResult = this.rootMaximizingNode.Children.ToDictionary(
                kvp => kvp.Key,
                kvp => GetPositionEvaluation(kvp.Value, searchDepth, double.NegativeInfinity, double.PositiveInfinity));

            return positionEvaluationResult;
        }

        private IDictionary<Move, bool> GetRiskOfDeathEvaluation()
        {
            var result = this.rootMaximizingNode.Children.ToDictionary(
                kvp => kvp.Key,
                kvp => GetRiskOfDeath(kvp.Value, RiskOfDeathSearchDepth));

            return result;
        }

        private IDictionary<Move, double> GetEvaluationDictionary()
        {
            var riskOfDeathTask = Task.Factory.StartNew(this.GetRiskOfDeathEvaluation);
            var positionEvaluationTask = Task.Factory.StartNew(this.GetPositionEvaluation);

            var riskOfDeathDictionary = riskOfDeathTask.Result;

            var positionEvaluationDictionary = positionEvaluationTask.Result;

            var result = new Dictionary<Move, double>
            {
                { Move.Up, double.NegativeInfinity },
                { Move.Down, double.NegativeInfinity },
                { Move.Left, double.NegativeInfinity },
                { Move.Right, double.NegativeInfinity }
            };

            foreach (var move in this.rootMaximizingNode.PossibleMoves)
            {
                result[move] = riskOfDeathDictionary[move] ? DeathEvaluation : positionEvaluationDictionary[move];
            }

            return result;
        }

        private int GetPositionEvaluationSearchDepth()
        {
            var depth = 3;

            Console.WriteLine("Searching with depth {0}", depth);

            return depth;
        }

        private static bool GetRiskOfDeath(MaximizingNode maximizingNode, int depth)
        {
            if (maximizingNode.GameOver)
            {
                return true;
            }

            if (depth == 0 || maximizingNode.EmptyCellCount >= SafeFreeCellsCount || maximizingNode.EmptyCellCount >= depth * 2)
            {
                return false;
            }

            return maximizingNode.Children.Values.All(child => GetRiskOfDeath(child, depth - 1));
        }

        private static bool GetRiskOfDeath(MinimizingNode minimizingNode, int depth)
        {
            if (minimizingNode.GameOver)
            {
                return true;
            }

            if (depth == 0 || minimizingNode.EmptyCellCount >= SafeFreeCellsCount || minimizingNode.EmptyCellCount >= depth * 2)
            {
                return false;
            }

            return minimizingNode.Children.Any(child => GetRiskOfDeath(child, depth - 1));
        }

        private static int maxCount = 0;
        
        private static double GetPositionEvaluation(MaximizingNode maximizingNode, int depth, double alpha, double beta)
        {
            if (maximizingNode.GameOver)
            {
                return DeathEvaluation;
            }

            var max = double.NegativeInfinity;
            foreach (var child in maximizingNode.Children.Values)
            {
                max = Math.Max(max, GetPositionEvaluation(child, depth - 1, alpha, beta));
                alpha = Math.Max(alpha, max);

                if (beta <= alpha)
                {
                    Interlocked.Increment(ref maxCount);
                    break;
                }
            }

            return max;
        }

        private static int minCount = 0;

        private static double GetPositionEvaluation(MinimizingNode minimizingNode, int depth, double alpha, double beta)
        {
            if (depth <= 0)
            {
                return minimizingNode.HeuristicValue;
            }

            var min = double.PositiveInfinity;
            foreach (var child in minimizingNode.Children)
            {
                min = Math.Min(min, GetPositionEvaluation(child, depth, alpha, beta));

                if (min <= DeathEvaluation)
                {
                    break;
                }

                beta = Math.Min(beta, min);
                
                if (beta <= alpha)
                {
                    Interlocked.Increment(ref minCount);
                    break;
                }
            }

            return min;
        }

        private static void LogEvaluation(MaximizingNode node, IDictionary<Move, double> evaluationDictionary)
        {
            Console.WriteLine(node.State.ToString());

            Console.WriteLine(
                "Left:  {0}",
                evaluationDictionary.ContainsKey(Move.Left) ? evaluationDictionary[Move.Left].ToString() : "null");
            Console.WriteLine(
                "Right: {0}",
                evaluationDictionary.ContainsKey(Move.Right) ? evaluationDictionary[Move.Right].ToString() : "null");
            Console.WriteLine(
                "Up:    {0}",
                evaluationDictionary.ContainsKey(Move.Up) ? evaluationDictionary[Move.Up].ToString() : "null");
            Console.WriteLine(
                "Down:  {0}",
                evaluationDictionary.ContainsKey(Move.Down) ? evaluationDictionary[Move.Down].ToString() : "null");
        }
    }
}