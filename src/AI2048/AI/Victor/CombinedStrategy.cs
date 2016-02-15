namespace AI2048.AI.Victor
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using AI2048.Game;

    public class CombinedStrategy
    {
        private const int RiskOfDeathSearchDepth = 5;

        private const int SafeFreeCellsCount = 2;

        private readonly Node rootNode;

        public CombinedStrategy(Node rootNode)
        {
            this.rootNode = rootNode;
        }

        public Move MakeDecision()
        {
            var sw = new Stopwatch();
            Console.WriteLine("Start calculation");
            sw.Start();
            var evaluationDictionary = this.GetEvaluationDictionary();
            sw.Stop();
            Console.WriteLine("End calcualtion, taken: {0}", sw.Elapsed);
            this.LogEvaluation(evaluationDictionary);

            var decision = evaluationDictionary.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).First();

            return decision;
        }

        private IDictionary<Move, long> GetEvaluationDictionary()
        {
            var riskOfDeathDictionary = new ConcurrentDictionary<Move, long>();
            var riskOfDeathTasks = this.rootNode.PossibleMoves.Select(
                move => Task.Run(
                    () =>
                        {
                            var hasRisk =
                                this.rootNode.ChildNodesByMove[move].Any(
                                    node => GetRiskOfDeath(node, RiskOfDeathSearchDepth));
                            riskOfDeathDictionary.TryAdd(move, hasRisk ? -1000000000 : 0);
                        })).ToArray();

            var searchDepth = this.GetPositionEvaluationSearchDepth();

            var positionEvaluationDictionary = new ConcurrentDictionary<Move, long>();
            var positionEvaluationTasks = this.rootNode.PossibleMoves.Select(
                move => Task.Run(
                    () =>
                        {
                            var positionEvaluation =
                                this.rootNode.ChildNodesByMove[move].Min(
                                    node => GetPositionEvaluation(node, searchDepth - 1));
                            positionEvaluationDictionary.TryAdd(move, positionEvaluation);
                        })).ToArray();

            Task.WaitAll(riskOfDeathTasks);
            Task.WaitAll(positionEvaluationTasks);

            var result = new Dictionary<Move, long>
            {
                { Move.Up, long.MinValue },
                { Move.Down, long.MinValue },
                { Move.Left, long.MinValue },
                { Move.Right, long.MinValue }
            };

            foreach (var kvp in riskOfDeathDictionary)
            {
                result[kvp.Key] = kvp.Value;
            }

            foreach (var kvp in positionEvaluationDictionary)
            {
                result[kvp.Key] += kvp.Value;
            }

            return result;
        }

        private int GetPositionEvaluationSearchDepth()
        {
            var searchDepth = 2;

            if (this.rootNode.EmptyCellCount < 8)
            {
                searchDepth = 3;
            }

            if (this.rootNode.EmptyCellCount < 4)
            {
                searchDepth = 4;
            }

            if (this.rootNode.EmptyCellCount < 2)
            {
                searchDepth = 5;
            }

            return searchDepth;
        }

        private static bool GetRiskOfDeath(Node node, int depth)
        {
            if (node.GameOver)
            {
                return true;
            }

            if (depth == 0 || node.EmptyCellCount > SafeFreeCellsCount)
            {
                return false;
            }

            return node.ChildNodesByMove.Values.All(cn => cn.Any(n => GetRiskOfDeath(n, depth - 1)));
        }

        private static long GetPositionEvaluation(Node node, int depth)
        {
            if (node.GameOver)
            {
                return -1000000000000;
            }

            if (depth == 0)
            {
                return EvaluatePosition(node);
            }

            return node.ChildNodesByMove.Values.Max(cn => cn.Min(n => GetPositionEvaluation(node, depth - 1)));
        }

        private static int EvaluatePosition(Node node)
        {
            var sum = node.State.Sum(cell => cell.Value * cell.Value * (cell.X + cell.Y));
            var cells = node.EmptyCellCount;

            return sum + cells;
        }

        private void LogEvaluation(IDictionary<Move, long> evaluationDictionary)
        {
            Console.WriteLine(this.rootNode.State.ToString());

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