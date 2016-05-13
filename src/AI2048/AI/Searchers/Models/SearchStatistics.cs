namespace AI2048.AI.Searchers.Models
{
    using System;
    using System.Globalization;
    using System.Text;

    using NodaTime;

    public class SearchStatistics
    {
        public Duration SearchDuration { get; set; }

        public int SearchDepth { get; set; }

        public int NodeCount { get; set; }
        public int TerminalNodeCount { get; set; }
        public double TerminalNodeFraction => this.TerminalNodeCount / (double)this.NodeCount;

        public int KnownPlayerNodes { get; set; }
        public int KnownComputerNodes { get; set; }
        public int KnownNodes => this.KnownComputerNodes + this.KnownPlayerNodes;
        public double KnownNodesFraction => this.KnownNodes / (double)this.NodeCount;
        public double KnownPlayerNodeFraction => this.KnownPlayerNodes / (double)this.KnownNodes;
        public double KnownComputerNodeFraction => this.KnownComputerNodes / (double)this.KnownNodes;

        public int RootNodeGrandchildren { get; set; }

        public long EstimatedTotalNodes => (long)Math.Pow(this.RootNodeGrandchildren, this.SearchDepth);

        public long NodesPerSecond => (long)(this.NodeCount * (1 / this.SearchDuration.ToTimeSpan().TotalSeconds));

        public long KnownNodesPerSecond => (long)(this.KnownNodes * (1 / this.SearchDuration.ToTimeSpan().TotalSeconds));

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Statistics:");
            sb.AppendLine($"Search duration:           {this.SearchDuration.ToString("ss.fff", CultureInfo.InvariantCulture),8} sec");
            sb.AppendLine($"Search depth:              {this.SearchDepth,8}");
            sb.AppendLine($"Nodes traversed:           {this.NodeCount,8}");
            sb.AppendLine($"Terminal nodes traversed:  {this.TerminalNodeCount,8} | {this.TerminalNodeFraction,7:#0.00%} of nodes traversed");
            sb.AppendLine($"Unique nodes:              {this.KnownNodes,8} | {this.KnownNodesFraction,7:#0.00%} of nodes traversed");
            sb.AppendLine($"Unique player nodes:       {this.KnownPlayerNodes,8} | {this.KnownPlayerNodeFraction,7:#0.00%} of unique nodes");
            sb.AppendLine($"Unique computer nodes:     {this.KnownComputerNodes,8} | {this.KnownComputerNodeFraction,7:#0.00%} of unique nodes");
            sb.AppendLine($"Nodes per second:          {this.NodesPerSecond,8}");
            sb.AppendLine($"Unique nodes per second:   {this.KnownNodesPerSecond,8}");
            sb.AppendLine($"Root node grandchildren:   {this.RootNodeGrandchildren,8}");

            return sb.ToString();
        }
    }
}