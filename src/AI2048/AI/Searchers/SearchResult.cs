namespace AI2048.AI.Searchers
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using AI2048.Game;

    public class SearchResult
    {
        public IDictionary<Move, double> Evaluations { get; set; }

        public string SearcherName { get; set; }

        public SearchStatistics SearchStatistics { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Search result for searcher {this.SearcherName}:");
            sb.Append(EvaluationToString(this.Evaluations));
            sb.Append(this.SearchStatistics);

            return sb.ToString();
        }

        private static string EvaluationToString(IDictionary<Move, double> evaluationDictionary)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Evalution results:");
            sb.AppendFormat(
                "Left:  {0}", 
                evaluationDictionary.ContainsKey(Move.Left)
                    ? evaluationDictionary[Move.Left].ToString(CultureInfo.InvariantCulture)
                    : "null").AppendLine();
            sb.AppendFormat(
                "Right: {0}", 
                evaluationDictionary.ContainsKey(Move.Right)
                    ? evaluationDictionary[Move.Right].ToString(CultureInfo.InvariantCulture)
                    : "null").AppendLine();
            sb.AppendFormat(
                "Up:    {0}", 
                evaluationDictionary.ContainsKey(Move.Up)
                    ? evaluationDictionary[Move.Up].ToString(CultureInfo.InvariantCulture)
                    : "null").AppendLine();
            sb.AppendFormat(
                "Down:  {0}", 
                evaluationDictionary.ContainsKey(Move.Down)
                    ? evaluationDictionary[Move.Down].ToString(CultureInfo.InvariantCulture)
                    : "null").AppendLine();

            return sb.ToString();
        }
    }
}