namespace Shared;

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

public class HashSetBenchmarks(string targetKey)
{
  private readonly HashSet<string> _normalHashSet = new();
  private readonly CompiledHashSet<string> _compiledHashSet = new();
  private string _targetKey = targetKey;

  [Params(6, 500, 2000)]
  public int Count { get; set; }
  
  [GlobalSetup]
  public void Setup()
  {
    // Seed the dictionaries with values
    foreach (var i in Enumerable.Range(0, Count))
    {
      var key = $"key_{i}";

      _normalHashSet.Add(key);
      _compiledHashSet.Add(key);
    }
      
    // Recompile lookup
    _compiledHashSet.Compile();

    // Try to get the middle element
    _targetKey = $"key_{Count / 2}";
  }

  [Benchmark(Description = "Standard HashSet", Baseline = true)]
  public bool NormalLookup() => _normalHashSet.Contains(_targetKey);

  [Benchmark(Description = "Compiled HashSet")]
  public bool CompiledLookup() => _compiledHashSet.Contains(_targetKey);
}
