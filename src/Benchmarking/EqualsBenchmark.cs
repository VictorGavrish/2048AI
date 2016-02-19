namespace Benchmarking
{
    using System;

    using AI2048.Game;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Jobs;

    [Config(typeof(Config))]
    public class EqualsBenchmark
    {
        private class Config : ManualConfig
        {
            public Config()
            {
                this.Add(Job.RyuJitX64);
            }
        }

        private readonly byte[,] grid1 = new byte[4, 4];

        private readonly byte[,] grid2 = new byte[4, 4];

        private readonly byte[,] grid3 = new byte[4, 4];
        
        private readonly LogarithmicGrid logarithmicGrid1;

        private readonly LogarithmicGrid logarithmicGrid2;

        private readonly LogarithmicGrid logarithmicGrid3;

        public EqualsBenchmark()
        {
            var random = new Random();

            var buffer = new byte[16];

            random.NextBytes(buffer);
            Buffer.BlockCopy(buffer, 0, this.grid1, 0, 16);
            this.logarithmicGrid1 = new LogarithmicGrid(this.grid1);

            random.NextBytes(buffer);
            Buffer.BlockCopy(buffer, 0, this.grid2, 0, 16);
            this.logarithmicGrid2 = new LogarithmicGrid(this.grid2);
            
            Buffer.BlockCopy(buffer, 0, this.grid3, 0, 16);
            this.logarithmicGrid3 = new LogarithmicGrid(this.grid3);
        }

        [Benchmark]
        public bool NaiveEqualsFalse() => this.logarithmicGrid1.NaiveEquals(this.logarithmicGrid2);
        [Benchmark]
        public bool NaiveEqualsTrue() => this.logarithmicGrid2.NaiveEquals(this.logarithmicGrid3);

        [Benchmark]
        public bool HardcodedEqualsFalse() => this.logarithmicGrid1.HardcodedEquals(this.logarithmicGrid2);
        [Benchmark]
        public bool HardcodedEqualsTrue() => this.logarithmicGrid2.HardcodedEquals(this.logarithmicGrid3);

        [Benchmark]
        public bool MemcmpEqualsFalse() => this.logarithmicGrid1.MemcmpEquals(this.logarithmicGrid2);
        [Benchmark]
        public bool MemcmpEqualsTrue() => this.logarithmicGrid2.MemcmpEquals(this.logarithmicGrid3);

        [Benchmark]
        public bool UnsafeEqualsFalse() => this.logarithmicGrid1.UnsafeEquals(this.logarithmicGrid2);
        [Benchmark]
        public bool UnsafeEqualsTrue() => this.logarithmicGrid2.UnsafeEquals(this.logarithmicGrid3);

    }
}