namespace AI2048.AI.Searchers.Models
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using AI2048.Game;

    public class SearchResult
    {
        public LogarithmicGrid RootGrid { get; set; }

        public Move BestMove { get; set; }

        public double BestMoveEvaluation { get; set; }

        public IDictionary<Move, double> MoveEvaluations { get; set; }

        public string SearcherName { get; set; }

        public SearchStatistics SearchStatistics { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(this.RootGrid.ToString());
            sb.AppendLine($"Search result for searcher {this.SearcherName}:");
            sb.AppendLine($"Best move: {this.BestMove}");
            sb.AppendLine($"Best move score: {this.BestMoveEvaluation}");
            sb.Append(EvaluationToString(this.MoveEvaluations));
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