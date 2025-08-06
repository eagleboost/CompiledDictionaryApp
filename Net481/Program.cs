namespace Net481
{
  using BenchmarkDotNet.Running;
  using Shared;

  internal class Program
  {
    public static void Main(string[] args)
    {
      // BenchmarkRunner.Run<Benchmarks>();
      // BenchmarkRunner.Run<SmallCountDictionaryBenchmarks>();
      BenchmarkRunner.Run<HashSetBenchmarks>();
    }
  }
}