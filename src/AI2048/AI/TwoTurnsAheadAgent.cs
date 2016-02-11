namespace AI2048.AI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AI2048.Game;

    public class TwoTurnsAheadAgent : Agent
    {
        public TwoTurnsAheadAgent(Func<Grid, long> heurstk)
            : base(heurstk)
        {
        }

        public override Move MakeDecision(Grid state)
        {
            var simulationResults = new Dictionary<Move, long>();
            foreach (var move in this.Moves)
            {
                var newState = GameLogic.MakeMove(state, move);
                if (newState == state)
                {
                    continue; // don't make unnecessary moves
                }

                simulationResults.Add(move, this.MakeMoveDecision(newState).Value);
            }

            var decision = simulationResults.OrderByDescending(p => p.Value).First();
            Console.WriteLine(
                string.Join(" ", simulationResults.Select(p => p.Value.ToString()).ToArray()) + ">" + decision.Value);

            return decision.Key;
        }

        public KeyValuePair<Move, long> MakeMoveDecision(Grid state)
        {
            var simulationResults = new Dictionary<Move, long>();
            foreach (var move in this.Moves)
            {
                var newState = GameLogic.MakeMove(state, move);
                if (newState == state)
                {
                    continue; // don't make unnecessary moves
                }

                simulationResults.Add(move, this.Heuristic(newState));
            }

            var decision = simulationResults.OrderByDescending(p => p.Value).First();

            // Console.WriteLine(String.Join(" ", simulationResults.Select(p=>p.Value.ToString()).ToArray()) + ">" + decision.Value);
            return decision;
        }
    }
}