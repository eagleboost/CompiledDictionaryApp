namespace Shared;

using System.Collections;
using System.Collections.Generic;

public partial class CompiledDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
  private readonly IDictionary<TKey, TValue> _inner;
  private readonly Impl _impl;
  
  public CompiledDictionary(IEqualityComparer<TKey>? comparer = null)
  {
    _inner = new Dictionary<TKey, TValue>(comparer);
    _impl = new Impl(_inner, comparer ?? EqualityComparer<TKey>.Default);
  }

  public void Compile() => _impl.Compile();

  public bool ContainsKey(TKey key) => _impl.ContainsKey(key);

  public TValue this[TKey key]
  {
    get => _impl[key];
    set => _inner[key] = value;
  }
  
  public void Clear() => _impl.Clear();
  
  public void Add(TKey key, TValue value) => _inner.Add(key, value);

  public bool Remove(TKey key) => _inner.Remove(key);

  public bool TryGetValue(TKey key, out TValue value) => _impl.TryGetValue(key, out value);

  public ICollection<TKey> Keys => _inner.Keys;

  public ICollection<TValue> Values => _inner.Values;

  public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _inner.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_inner).GetEnumerator();

  public void Add(KeyValuePair<TKey, TValue> item) => _inner.Add(item);
  
  public bool Contains(KeyValuePair<TKey, TValue> item) => _inner.Contains(item);

  public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);

  public bool Remove(KeyValuePair<TKey, TValue> item) => _inner.Remove(item);

  public int Count => _inner.Count;

  public bool IsReadOnly => _inner.IsReadOnly;
}