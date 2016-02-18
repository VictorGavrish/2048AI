namespace Benchmarking
{
    using System;

    using AI2048.Game;

    using BenchmarkDotNet.Attributes;

    public class EqualsBenchmark
    {
        private readonly LogarithmicGrid logarithmicGrid1;

        private readonly LogarithmicGrid logarithmicGrid2;

        private readonly LogarithmicGrid logarithmicGrid3;

        public EqualsBenchmark()
        {
            var random = new Random();

            var buffer = new byte[16];

            random.NextBytes(buffer);
            var grid = new byte[4, 4];
            Buffer.BlockCopy(buffer, 0, grid, 0, 16);
            this.logarithmicGrid1 = new LogarithmicGrid(grid);

            random.NextBytes(buffer);
            grid = new byte[4, 4];
            Buffer.BlockCopy(buffer, 0, grid, 0, 16);
            this.logarithmicGrid2 = new LogarithmicGrid(grid);

            grid = new byte[4,4];
            Buffer.BlockCopy(buffer, 0, grid, 0, 16);
            this.logarithmicGrid3 = new LogarithmicGrid(grid);
        }

        [Benchmark]
        public bool EqualsFalse() => this.logarithmicGrid1.Equals(this.logarithmicGrid2);

        [Benchmark]
        public bool EqualsTrue() => this.logarithmicGrid2.Equals(this.logarithmicGrid3);

    }
}