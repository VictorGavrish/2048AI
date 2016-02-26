namespace Benchmarking
{
    using AI2048.AI.SearchTree;
    using AI2048.Game;

    using BenchmarkDotNet.Attributes;

    [Config(typeof(Config))]
    public class TraverseBenchmark
    {
        private readonly string startgingNode = @"
                     1024  256   64    0
                       64   32    8    2
                        4   16   16    8
                        2    4    2    4
                    ";

        private readonly MaximizingNode rootNode; 

        private const int SearchDepth = 4;

        public TraverseBenchmark()
        {
            var rootGrid = LogarithmicGrid.Parse(this.startgingNode);

            this.rootNode = new MaximizingNode(rootGrid, null);
        }

        [Benchmark]
        public void Traverse()
        {
            foreach (var child in this.rootNode.Children.Values)
            {
                this.Visit(child, SearchDepth);
            }
        }

        private void Visit(MinimizingNode node, int searchDepth)
        {
            foreach (var child in node.Children)
            {
                this.Visit(child, searchDepth - 1);
            }
        }

        private void Visit(MaximizingNode node, int searchDepth)
        {
            if (searchDepth == 0)
            {
                return;
            }

            foreach (var child in node.Children.Values)
            {
                this.Visit(child, searchDepth);
            }
        }
    }
}