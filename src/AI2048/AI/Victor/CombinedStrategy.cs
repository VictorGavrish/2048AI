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
        private const int RiskOfDeathSearchDepth = 13;

        private const int SafeFreeCellsCount = 3;

        private const long DeathEvaluation = -1000000000000000000;

        private readonly PlayerNode rootPlayerNode;

        public CombinedStrategy(PlayerNode rootPlayerNode)
        {
            this.rootPlayerNode = rootPlayerNode;
        }

        public Move MakeDecision()
        {
            var sw = new Stopwatch();
            Console.WriteLine("Start calculation");
            sw.Start();
            var evaluationDictionary = this.GetEvaluationDictionary();
            sw.Stop();
            Console.WriteLine("End calcualtion, taken: {0}", sw.Elapsed);
            LogEvaluation(this.rootPlayerNode, evaluationDictionary);

            var decision = evaluationDictionary.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key).First();

            return decision;
        }

        private IDictionary<Move, long> GetEvaluationDictionarySynchronous()
        {
            var riskOfDeathResult = this.rootPlayerNode.Children.ToDictionary(
                kvp => kvp.Key,
                kvp => GetRiskOfDeath(kvp.Value, RiskOfDeathSearchDepth));

            var searchDepth = this.GetPositionEvaluationSearchDepth();

            IEnumerable<KeyValuePair<Move, ComputerNode>> children = this.rootPlayerNode.Children;

            var safeChildren = riskOfDeathResult.Where(kvp => !kvp.Value);

            if (safeChildren.Any())
            {
                children = children.Where(kvp => safeChildren.Any(safekvp => safekvp.Key == kvp.Key));
            }

            var positionEvaluationResult = children.ToDictionary(
                kvp => kvp.Key,
                kvp => GetPositionEvaluation(kvp.Value, searchDepth));

            return positionEvaluationResult;
        }

        private IDictionary<Move, long> GetEvaluationDictionary()
        {
            var riskOfDeathDictionary = new ConcurrentDictionary<Move, bool>();
            
            var riskOfDeathTasks = this.rootPlayerNode.PossibleMoves.Select(
                move => Task.Run(
                    () =>
                        {
                            var hasRisk =
                                this.rootPlayerNode.Children[move].Children.Any(
                                    node => GetRiskOfDeath(node, RiskOfDeathSearchDepth));
                            riskOfDeathDictionary.TryAdd(move, hasRisk);
                        })).ToArray();

            var searchDepth = this.GetPositionEvaluationSearchDepth();

            var positionEvaluationDictionary = new ConcurrentDictionary<Move, long>();
            var positionEvaluationTasks = this.rootPlayerNode.PossibleMoves.Select(
                move => Task.Run(
                    () =>
                        {
                            var positionEvaluation =
                                this.rootPlayerNode.Children[move].Children.Min(
                                    node => GetPositionEvaluation(node, searchDepth - 1));
                            positionEvaluationDictionary.TryAdd(move, positionEvaluation);
                        })).ToArray();

            Task.WaitAll(riskOfDeathTasks);

            if (!riskOfDeathDictionary.All(kvp => kvp.Value))
            {
                foreach (var kvp in riskOfDeathDictionary.Where(kvp => kvp.Value))
                {
                    this.rootPlayerNode.Children[kvp.Key].MakeTerminal();
                }
            }

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
                result[kvp.Key] = kvp.Value ? DeathEvaluation : 0;
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

            if (this.rootPlayerNode.EmptyCellCount < 10)
            {
                searchDepth = 4;
            }

            if (this.rootPlayerNode.EmptyCellCount < 2)
            {
                searchDepth = 6;
            }

            return searchDepth;
        }

        private static bool GetRiskOfDeath(PlayerNode playerNode, int depth)
        {
            if (playerNode.IsTernminal)
            {
                return true;
            }

            if (depth == 0 || playerNode.EmptyCellCount >= SafeFreeCellsCount || playerNode.EmptyCellCount >= depth * 2)
            {
                return false;
            }

            return playerNode.Children.Values.All(child => GetRiskOfDeath(child, depth - 1));
        }

        private static bool GetRiskOfDeath(ComputerNode computerNode, int depth)
        {
            if (computerNode.IsTerminal)
            {
                return true;
            }

            if (depth == 0 || computerNode.EmptyCellCount >= SafeFreeCellsCount || computerNode.EmptyCellCount >= depth * 2)
            {
                return false;
            }

            return computerNode.Children.Any(child => GetRiskOfDeath(child, depth - 1));
        }

        private static long GetPositionEvaluation(PlayerNode playerNode, int depth)
        {
            if (playerNode.IsTernminal)
            {
                return DeathEvaluation;
            }

            return playerNode.Children.Values.Max(child => GetPositionEvaluation(child, depth - 1));
        }

        private static long GetPositionEvaluation(ComputerNode computerNode, int depth)
        {
            if (computerNode.IsTerminal)
            {
                return DeathEvaluation;
            }

            if (depth <= 0)
            {
                return EvaluatePosition(computerNode);
            }

            var min = long.MaxValue;
            foreach (var evaluation in computerNode.Children.Select(child => GetPositionEvaluation(child, depth - 1)))
            {
                if (evaluation < min)
                {
                    min = evaluation;
                }

                if (min <= DeathEvaluation)
                {
                    return min;
                }
            }

            return min;
        }

        private static long EvaluatePosition(ComputerNode node)
        {
            var sum = node.State.Sum(cell => cell.Value * cell.Value * (cell.X + cell.Y));
            var cells = node.EmptyCellCount;

            return sum + cells;
        }

        private static void LogEvaluation(PlayerNode node, IDictionary<Move, long> evaluationDictionary)
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