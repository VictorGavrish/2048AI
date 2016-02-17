namespace AI2048.AI.Searchers
{
    using System.Globalization;
    using System.Text;

    using NodaTime;

    public class SearchStatistics
    {
        public Duration SearchDuration { get; set; }

        public int SearchDepth { get; set; }

        public int NodesTraversed { get; set; }

        public int TerminalNodeCount { get; set; }

        public int RootNodeGrandchildren { get; set; }
        
        public long EstimatedTotalNodes
        {
            get
            {
                long result = 1;
                for (var i = this.SearchDepth; i >= 0; i -= 2)
                {
                    result *= this.RootNodeGrandchildren;
                }

                if (this.SearchDepth % 2 != 0)
                {
                    result *= 3;
                }

                return result;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Statistics:");
            sb.AppendLine($"Search duration:                 {this.SearchDuration.ToString("M:ss.fff", CultureInfo.InvariantCulture)}");
            sb.AppendLine($"Search depth:                    {this.SearchDepth}");
            sb.AppendLine($"Estimated total nodes:           {this.EstimatedTotalNodes}");
            sb.AppendLine($"Nodes traversed:                 {this.NodesTraversed} ({this.NodesTraversed / (double)this.EstimatedTotalNodes:P})");
            sb.AppendLine($"Terminal nodes found:            {this.TerminalNodeCount}");
            sb.AppendLine($"Root node grandchildren:         {this.RootNodeGrandchildren}");

            return sb.ToString();
        }
    }
}