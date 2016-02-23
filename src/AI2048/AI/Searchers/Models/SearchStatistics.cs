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

        public int KnownPlayerNodes { get; set; }
        public int KnownComputerNodes { get; set; }
        public int KnownNodes => this.KnownComputerNodes + this.KnownPlayerNodes;

        public int TerminalNodeCount { get; set; }

        public int RootNodeGrandchildren { get; set; }

        public long EstimatedTotalNodes => (long)Math.Pow(this.RootNodeGrandchildren, this.SearchDepth);

        public long NodesPerSecond => (long)(this.NodesTraversed * (1 / this.SearchDuration.ToTimeSpan().TotalSeconds));

        public long KnownNodesPerSecond => (long)(this.KnownNodes * (1 / this.SearchDuration.ToTimeSpan().TotalSeconds));
        
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Statistics:");
            sb.AppendLine($"Search duration:                 {this.SearchDuration.ToString("M:ss.fff", CultureInfo.InvariantCulture)}");
            sb.AppendLine($"Search depth:                    {this.SearchDepth}");
            sb.AppendLine($"Nodes traversed:                 {this.NodesTraversed}");
            sb.AppendLine($"Terminal nodes traversed:        {this.TerminalNodeCount} ({this.TerminalNodeCount / (double)this.NodesTraversed:P})");
            sb.AppendLine($"Known unique nodes:              {this.KnownNodes}  ({this.KnownNodes / (double)this.NodesTraversed:P})");
            sb.AppendLine($"Known player unique nodes:       {this.KnownPlayerNodes}  ({this.KnownPlayerNodes / (double)this.KnownNodes:P})");
            sb.AppendLine($"Known computer unique nodes:     {this.KnownComputerNodes}  ({this.KnownComputerNodes / (double)this.KnownNodes:P})");
            sb.AppendLine($"Nodes per second:                {this.NodesPerSecond}");
            sb.AppendLine($"Known nodes per second:          {this.KnownNodesPerSecond}");
            sb.AppendLine($"Root node grandchildren:         {this.RootNodeGrandchildren}");

            return sb.ToString();
        }
    }
}