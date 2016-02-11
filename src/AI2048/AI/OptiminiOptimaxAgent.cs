namespace AI2048.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class OptiminiOptimaxAgent : Agent
    {
        public const int MaxDepth = 3;

        public OptiminiOptimaxAgent(Func<Grid, long> heurstk)
            : base(heurstk)
        {
        }

        public override Move MakeDecision(Grid state)
        {
            var simulationResults = new Dictionary<Move, long>();
            foreach (var move in this.Moves)
            {
                simulationResults.Add(move, this.evaluateMove(state, move));
            }

            var decision = simulationResults.OrderByDescending(p => p.Value).First();

            // Console.WriteLine(String.Join(" ", simulationResults.Select(p => p.Value.ToString()).ToArray()) + ">" + decision.Value);
            return decision.Key;
        }

        public long evaluateMove(Grid state, Move move)
        {
            var newState = GameLogic.MakeMove(state, move);
            if (newState == state)
            {
                return long.MinValue;
            }

            return this.maxNodeValue(newState, 0);
        }

        public long maxNodeValue(Grid state, int currDepth)
        {
            if (currDepth >= MaxDepth)
            {
                return this.Heuristic(state);
            }

            var value = long.MinValue;
            foreach (var move in this.Moves)
            {
                var newState = GameLogic.MakeMove(state, move);
                if (newState == state)
                {
                    continue;
                }

                long newVal;
                if (state.EmptyCellsNo > 3)
                {
                    newVal = this.randomNodeValue(newState, currDepth + 1);
                }
                else
                {
                    newVal = this.minNodeValue(newState, currDepth + 1);
                }

                value = Math.Max(value, newVal);
            }

            return value;
        }

        public long randomNodeValue(Grid state, int currDepth)
        {
            var value = 0L;
            var nextStates = GameLogic.NextPossibleWorldStates(state);
            if (nextStates.Count == 0)
            {
                return long.MinValue;
            }

            foreach (var nextState in nextStates)
            {
                var newVal = this.maxNodeValue(nextState, currDepth + 1);
                value += newVal / nextStates.Count;
            }

            return value;
        }

        public long minNodeValue(Grid state, int currDepth)
        {
            var value = long.MaxValue;
            var nextStates = GameLogic.NextPossibleWorldStates(state);
            if (nextStates.Count == 0)
            {
                return long.MinValue;
            }

            foreach (var nextState in nextStates)
            {
                var newVal = this.maxNodeValue(nextState, currDepth + 1);
                value = Math.Min(value, newVal);
            }

            return value;
        }
    }
}