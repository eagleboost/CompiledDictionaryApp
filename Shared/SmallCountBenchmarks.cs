namespace Shared;

using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

public class SmallCountBenchmarks
{
  private readonly Dictionary<string, int> _normalDictionary = new();
  private readonly CompiledDictionary<string, int> _compiledDictionary = new();
  private string[] _array;
    
  [Params(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)]
  public int Count { get; set; }

  public string TargetKey { get; set; }

  [GlobalSetup]
  public void Setup()
  {
    _array= new string[Count];
    // Seed the dictionaries with values
    foreach (var i in Enumerable.Range(0, Count))
    {
      var key = $"key_{i}";

      _normalDictionary[key] = i;
      _compiledDictionary[key] = i;
      _array[i] = key;
    }
      
    // Recompile lookup
    _compiledDictionary.Compile();

    // Try to get the middle element
    TargetKey = $"key_{Count / 2}";
  }

  [Benchmark(Description = "Standard dictionary", Baseline = true)]
  public void NormalLookup()
  {
    foreach (var i in _array)
    {
      _ = _normalDictionary[i];
    }
  }

  [Benchmark(Description = "Compiled dictionary")]
  public void CompiledLookup()
  {
    foreach (var i in _array)
    {
      _ = _compiledDictionary[i];
    }
  }

  [Benchmark(Description = "Array")]
  public void ArrayLookup()
  {
    for (var j = 0; j < Count; j++)
    {
      var key = _array[j];
      for (var i = 0; i < Count; i++)
      {
        if (_array[i] == key)
        {
        }
      }
    }
  }
}