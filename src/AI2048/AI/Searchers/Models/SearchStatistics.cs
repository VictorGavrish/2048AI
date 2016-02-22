namespace AI2048.AI.Searchers.Models
{
    using System;
    using System.Globalization;
    using System.Text;

    using NodaTime;

    public class SearchStatistics
    {
        public bool SearchExhaustive { get; set; }

        public Duration SearchDuration { get; set; }

        public int SearchDepth { get; set; }

        public int NodesTraversed { get; set; }

        public int TerminalNodeCount { get; set; }

        public int RootNodeGrandchildren { get; set; }

        public long EstimatedTotalNodes => (long)Math.Pow(this.RootNodeGrandchildren, this.SearchDepth);

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Statistics:");
            sb.AppendLine($"Search duration:                 {this.SearchDuration.ToString("M:ss.fff", CultureInfo.InvariantCulture)}");
            sb.AppendLine($"Search depth:                    {this.SearchDepth}");
            sb.AppendLine($"Estimated total nodes:           {this.EstimatedTotalNodes}");
            sb.AppendLine($"Nodes traversed:                 {this.NodesTraversed} ({this.NodesTraversed / (double)this.EstimatedTotalNodes:P})");
            sb.AppendLine($"Terminal nodes found:            {this.TerminalNodeCount} ({this.TerminalNodeCount / (double)this.EstimatedTotalNodes:P})");
            sb.AppendLine($"Root node grandchildren:         {this.RootNodeGrandchildren}");

            return sb.ToString();
        }
    }
}