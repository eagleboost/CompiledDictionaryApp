namespace Tests;

using Shared;

public class CompiledHashSetTests
{
  [Test]
  public void Test_01_Contains_True()
  {
    var hashSet = new CompiledHashSet<int>();
    var values = Enumerable.Range(0, 1000).ToArray();
    foreach (var i in values)
    {
      hashSet.Add(i);
    }
    
    hashSet.Compile();
    foreach (var i in values)
    {
      Assert.That(hashSet.Contains(i), Is.True);
    }
  } 
  
  [Test]
  public void Test_02_Contains_False()
  {
    var hashSet = new CompiledHashSet<int>();
    var values = Enumerable.Range(0, 1000).ToArray();
    foreach (var i in values)
    {
      hashSet.Add(i);
    }
    
    hashSet.Compile();
    values = Enumerable.Range(1001, 2000).ToArray();
    foreach (var i in values)
    {
      Assert.That(hashSet.Contains(i), Is.False);
    }
  }  
  
  [Test]
  public void Test_03_Reset()
  {
    var hashSet = new CompiledHashSet<int> { 0 };
    hashSet.Compile();
    Assert.That(hashSet.Contains(0), Is.EqualTo(true));
    hashSet.Clear();
    hashSet.Compile();
    Assert.That(hashSet.Contains(0), Is.EqualTo(false));
  }
  
  [Test]
  public void Test_04_Collision()
  {
    var hashSet = new CompiledHashSet<Model>();
    var model1 = new Model(1);
    var model2 = new Model(1);
    
    ////Same HashCode, different object
    Assert.That(model1.GetHashCode(), Is.EqualTo(model2.GetHashCode()));
    
    Assert.That(hashSet.Add(model1), Is.EqualTo(true));
    Assert.That(hashSet.Add(model2), Is.EqualTo(true));
    
    hashSet.Compile();
    
    Assert.That(hashSet.Contains(model1), Is.EqualTo(true));
    Assert.That(hashSet.Contains(model2), Is.EqualTo(true));
  }
  
  [Test]
  public void Test_5_CaseInsensitive()
  {
    var hashSet = new CompiledHashSet<string>(StringComparer.OrdinalIgnoreCase) { "A" };
    hashSet.Compile();
    Assert.That(hashSet.Contains("a"), Is.EqualTo(true));
  }
}