namespace AI2048.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.AI.Victor;
    using AI2048.Game;

    public class OptiminiOptimaxAgent : Agent
    {
        public const int MaxDepth = 3;

        private readonly Func<Grid, long> heuristic;

        public OptiminiOptimaxAgent(Func<Grid, long> heurstk)
        {
            this.heuristic = heurstk;
        }

        public override Move MakeDecision(Grid state)
        {
            var simulationResults = Moves.ToDictionary(move => move, move => this.EvaluateMove(state, move));

            var decision = simulationResults.OrderByDescending(p => p.Value).First();

            // Console.WriteLine(String.Join(" ", simulationResults.Select(p => p.Value.ToString()).ToArray()) + ">" + decision.Value);
            return decision.Key;
        }

        public long EvaluateMove(Grid state, Move move)
        {
            var newState = GameLogic.MakeMove(state, move);
            return newState == state ? long.MinValue : this.MaxNodeValue(newState, 0);
        }

        public long MaxNodeValue(Grid state, int currDepth)
        {
            if (currDepth >= MaxDepth)
            {
                return this.heuristic(state);
            }

            var value = long.MinValue;
            foreach (var move in Moves)
            {
                var newState = GameLogic.MakeMove(state, move);
                if (newState == state)
                {
                    continue;
                }

                var newVal = state.EmptyCellsNo > 3 
                    ? this.RandomNodeValue(newState, currDepth + 1) 
                    : this.MinNodeValue(newState, currDepth + 1);

                value = Math.Max(value, newVal);
            }

            return value;
        }

        public long RandomNodeValue(Grid state, int currDepth)
        {
            var value = 0L;
            var nextStates = GameLogic.NextPossibleWorldStates(state);
            if (nextStates.Count() == 0)
            {
                return long.MinValue;
            }

            foreach (var nextState in nextStates)
            {
                var newVal = this.MaxNodeValue(nextState, currDepth + 1);
                
                value += newVal / nextStates.Count();
            }

            return value;
        }

        public long MinNodeValue(Grid state, int currDepth)
        {
            var value = long.MaxValue;
            var nextStates = GameLogic.NextPossibleWorldStates(state);

            if (nextStates.Count() == 0)
            {
                return long.MinValue;
            }

            foreach (var nextState in nextStates)
            {
                var newVal = this.MaxNodeValue(nextState, currDepth + 1);
                value = Math.Min(value, newVal);
            }

            return value;
        }
    }
}