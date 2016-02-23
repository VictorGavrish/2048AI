namespace AI2048.AI.Searchers.Models
{
    using System.Globalization;
    using System.Text;

    public class AlphaBetaSearchStatistics : SearchStatistics
    {
        public int MaximizingNodeBranchesPruned { get; set; }

        public int MinimizingNodeBranchesPruned { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(base.ToString());
            sb.AppendLine($"Estimated total nodes:           {this.EstimatedTotalNodes}");
            sb.AppendLine($"Maximizing node branches pruned: {this.MaximizingNodeBranchesPruned}");
            sb.AppendLine($"Minimizing node branches pruned: {this.MinimizingNodeBranchesPruned}");

            return sb.ToString();
        }
    }
}