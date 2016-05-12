namespace Benchmarking
{
    using AI2048.AI.SearchTree;
    using AI2048.Game;

    using BenchmarkDotNet.Attributes;

    [Config(typeof(Config))]
    public class TraverseBenchmark
    {
        private const string StartgingNode = @"
                     1024  256   64    0
                       64   32    8    2
                        4   16   16    8
                        2    4    2    4
                    ";

        private readonly SearchTree searchTree; 

        private const int SearchDepth = 5;

        public TraverseBenchmark()
        {
            var rootGrid = LogarithmicGrid.Parse(StartgingNode);

            this.searchTree = new SearchTree(null, rootGrid);
        }

        [Benchmark]
        public void Traverse()
        {
            foreach (var child in this.searchTree.RootNode.Children.Values)
            {
                this.Visit(child, SearchDepth);
            }
        }

        private void Visit(IComputerNode node, int searchDepth)
        {
            foreach (var child in node.Children)
            {
                this.Visit(child, searchDepth - 1);
            }
        }

        private void Visit(IPlayerNode node, int searchDepth)
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