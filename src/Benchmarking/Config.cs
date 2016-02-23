namespace Benchmarking
{
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Jobs;

    public class Config : ManualConfig
    {
        public Config()
        {
            this.Add(Job.RyuJitX64);
        }
    }
}