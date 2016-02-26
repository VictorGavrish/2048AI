namespace AI2048.AI.SearchTree
{
    using System.Collections.Generic;

    using AI2048.AI.Heristics;
    using AI2048.Game;

    public class SearchTree
    {
        public IHeuristic Heuristic { get; }

        public MaximizingNode RootNode { get; private set; }

        public SearchTree(IHeuristic heuristic, LogarithmicGrid startingGrid)
        {
            this.Heuristic = heuristic;
            this.RootNode = new MaximizingNode(startingGrid, this);

            this.KnownPlayerNodes = new Dictionary<LogarithmicGrid, MaximizingNode>();
            this.KnownComputerNodes = new Dictionary<LogarithmicGrid, MinimizingNode>();
        }

        public void MoveRoot(LogarithmicGrid newGrid)
        {
            this.RootNode = new MaximizingNode(newGrid, this);

            this.KnownPlayerNodes = new Dictionary<LogarithmicGrid, MaximizingNode>();
            this.KnownComputerNodes = new Dictionary<LogarithmicGrid, MinimizingNode>();
        }

        public IDictionary<LogarithmicGrid, MaximizingNode> KnownPlayerNodes { get; private set; }

        public IDictionary<LogarithmicGrid, MinimizingNode> KnownComputerNodes { get; private set; }
    }
}