namespace AI2048.AI.Searchers
{
    using System.Globalization;
    using System.Text;

    public class AlphaBetaSearchStatistics : SearchStatistics
    {
        public int MaximizingNodeBranchesPruned { get; set; }

        public int MinimizingNodeBranchesPruned { get; set; }

        public int EstimatedNodesPruned { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Statistics:");
            sb.AppendLine($"Search duration:                 {this.SearchDuration.ToString("M:ss.fff", CultureInfo.InvariantCulture)}");
            sb.AppendLine($"Search depth:                    {this.SearchDepth}");
            sb.AppendLine($"Estimated total nodes:           {this.EstimatedTotalNodes}");
            sb.AppendLine($"Nodes traversed:                 {this.NodesTraversed} ({this.NodesTraversed / (double)this.EstimatedTotalNodes:P})");
            sb.AppendLine($"Terminal nodes found:            {this.TerminalNodeCount}");
            sb.AppendLine($"Maximizing node branches pruned: {this.MaximizingNodeBranchesPruned}");
            sb.AppendLine($"Minimizing node branches pruned: {this.MinimizingNodeBranchesPruned}");
            sb.AppendLine($"Estimated nodes pruned:          {this.EstimatedNodesPruned}");
            sb.AppendLine($"Root node grandchildren:         {this.RootNodeGrandchildren}");

            return sb.ToString();
        }
    }
}