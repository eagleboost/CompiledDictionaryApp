namespace Tests;

using Shared;

public partial class CompiledDictionaryTests
{
  [Test]
  public void Test_01_Indexer()
  {
    var dict = new CompiledDictionary<int, string>();
    var values = Enumerable.Range(0, 10000).ToArray();
    foreach (var i in values)
    {
      dict[i] = $"Value {i}";
    }
    
    dict.Compile();
    foreach (var i in values)
    {
      Assert.That(dict[i], Is.EqualTo($"Value {i}"));
    }
  }
  
  [Test]
  public void Test_02_Indexer_Exception()
  {
    var dict = new CompiledDictionary<int, int>();
    var values = Enumerable.Range(0, 4).ToArray();
    foreach (var i in values)
    {
      dict[i] = i;
    }
    
    dict.Compile();
    values = Enumerable.Range(5, 8).ToArray();
    foreach (var i in values)
    {
      Assert.Throws<KeyNotFoundException>(() => _ = dict[i]);
    }
  }
  
  [Test]
  public void Test_03_TryGetValue_True()
  {
    var dict = new CompiledDictionary<int, int>();
    var values = Enumerable.Range(0, 1000).ToArray();
    foreach (var i in values)
    {
      dict[i] = i;
    }
    
    dict.Compile();
    foreach (var i in values)
    {
      var ret = dict.TryGetValue(i, out var value);
      Assert.That(ret, Is.True);
      Assert.That(value, Is.EqualTo(i));
    }
  }  
  
  [Test]
  public void Test_04_TryGetValue_False()
  {
    var dict = new CompiledDictionary<int, int>();
    var values = Enumerable.Range(0, 1000).ToArray();
    foreach (var i in values)
    {
      dict[i] = i;
    }
    
    dict.Compile();
    
    values = Enumerable.Range(1001, 2000).ToArray();
    foreach (var i in values)
    {
      var ret = dict.TryGetValue(i, out var value);
      Assert.That(ret, Is.False);
      Assert.That(value, Is.EqualTo(0));
    }
  }

  [Test]
  public void Test_05_ContainsKey_True()
  {
    var dict = new CompiledDictionary<int, int>();
    var values = Enumerable.Range(0, 1000).ToArray();
    foreach (var i in values)
    {
      dict[i] = i;
    }
    
    dict.Compile();
    foreach (var i in values)
    {
      Assert.That(dict.ContainsKey(i), Is.True);
    }
  } 
  
  [Test]
  public void Test_06_ContainsKey_False()
  {
    var dict = new CompiledDictionary<int, int>();
    var values = Enumerable.Range(0, 1000).ToArray();
    foreach (var i in values)
    {
      dict[i] = i;
    }
    
    dict.Compile();
    values = Enumerable.Range(1001, 2000).ToArray();
    foreach (var i in values)
    {
      Assert.That(dict.ContainsKey(i), Is.False);
    }
  }  
  
  [Test]
  public void Test_07_Uncompiled()
  {
    var dict = new CompiledDictionary<int, int>();
    var values = Enumerable.Range(0, 3).ToArray();
    foreach (var i in values)
    {
      dict[i] = i;
    }
    
    foreach (var i in values)
    {
      Assert.Throws<KeyNotFoundException>(() => _ = dict[i]);
    }
  }
  
  [Test]
  public void Test_08_Collision()
  {
    var dict = new CompiledDictionary<Model, Model>();
    var model1 = new Model(1);
    var model2 = new Model(1);
    
    ////Same HashCode, different object
    Assert.That(model1.GetHashCode(), Is.EqualTo(model2.GetHashCode()));
    
    dict[model1] = model1;
    dict[model2] = model2;

    Assert.That(dict, Has.Count.EqualTo(2));
    
    dict.Compile();
    
    Assert.That(dict[model1], Is.EqualTo(model1));
    Assert.That(dict[model2], Is.EqualTo(model2));
  }
  
  [Test]
  public void Test_09_Reset()
  {
    var dict = new CompiledDictionary<int, string> { [0] = "Value 0" };
    dict.Compile();
    Assert.That(dict[0], Is.EqualTo("Value 0"));
    dict.Clear();
    dict.Compile();
    Assert.Throws<KeyNotFoundException>(() => _ = dict[0]);
  }
  
  [Test]
  public void Test_10_CaseInsensitive()
  {
    var dict = new CompiledDictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
      ["A"] = "Value A"
    };
    dict.Compile();
    Assert.That(dict["a"], Is.EqualTo("Value A"));
  }
}