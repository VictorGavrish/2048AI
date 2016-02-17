namespace Benchmarking
{
    using System;

    using AI2048.Game;

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

            var grid1 = new byte[4, 4];

            Buffer.BlockCopy(buffer, 0, grid1, 0, 16);
        }
         
    }
}