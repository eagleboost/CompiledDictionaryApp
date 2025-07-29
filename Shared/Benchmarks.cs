namespace Shared
{
  using System.Collections.Generic;
  using System.Linq;
  using BenchmarkDotNet.Attributes;

  public class Benchmarks
  {
    private readonly Dictionary<string, int> _normalDictionary = new();

    private readonly CompiledDictionary<string, int> _compiledDictionary = new();
    
    [Params(2000, 4000)]
    public int Count { get; set; }

    public string TargetKey { get; set; }

    [GlobalSetup]
    public void Setup()
    {
      // Seed the dictionaries with values
      foreach (var i in Enumerable.Range(0, Count))
      {
        var key = $"key_{i}";

        _normalDictionary[key] = i;
        _compiledDictionary[key] = i;
      }
      
      // Recompile lookup
      _compiledDictionary.Compile();

      // Try to get the middle element
      TargetKey = $"key_{Count / 2}";
    }

    [Benchmark(Description = "Standard dictionary", Baseline = true)]
    public int NormalLookup() => _normalDictionary[TargetKey];

    [Benchmark(Description = "Compiled dictionary")]
    public int CompiledLookup() => _compiledDictionary[TargetKey];

    [Benchmark(Description = "Standard TryGetValue")]
    public int NormalTryGetValue() => _normalDictionary.TryGetValue(TargetKey, out var value) ? value : -1;

    [Benchmark(Description = "Compiled TryGetValue")]
    public int CompiledTryGetValue() => _compiledDictionary.TryGetValue(TargetKey, out var value) ? value : -1;

    [Benchmark(Description = "Standard ContainsKey")]
    public bool NormalContainsKey() => _normalDictionary.ContainsKey(TargetKey);

    [Benchmark(Description = "Compiled ContainsKey")]
    public bool CompiledContainsKey() => _compiledDictionary.ContainsKey(TargetKey);
  }
}