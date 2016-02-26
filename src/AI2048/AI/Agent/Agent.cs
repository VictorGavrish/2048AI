namespace AI2048.AI.Agent
{
    using System.Collections.Generic;

    using AI2048.AI.Heristics;
    using AI2048.AI.Searchers;
    using AI2048.AI.SearchTree;
    using AI2048.AI.Strategy;
    using AI2048.Game;

    public class Agent
    {
        private readonly SearchTree searchTree;

        public Agent(LogarithmicGrid startingGrid)
        {
            this.searchTree = new SearchTree(new VictorHeuristic(), startingGrid);
        }

        public List<LogarithmicGrid> History { get; } = new List<LogarithmicGrid>();

        public Move MakeDecision()
        {
            if (this.searchTree.RootNode.GameOver)
            {
                throw new GameOverException();
            }

            var probabilityLimitedExpectoMaxer = new ProbabilityLimitedExpectiMaxer(this.searchTree.RootNode);

            var strategy = new SimpleStrategy(probabilityLimitedExpectoMaxer);

            var decision = strategy.MakeDecision();

            return decision;
        }

        public void UpdateGrid(LogarithmicGrid grid)
        {
            this.History.Add(grid);
            this.searchTree.MoveRoot(grid);
        }
    }
}