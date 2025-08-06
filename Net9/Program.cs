// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Shared;

// BenchmarkRunner.Run<Benchmarks>();
// BenchmarkRunner.Run<SmallCountDictionaryBenchmarks>();
BenchmarkRunner.Run<HashSetBenchmarks>();