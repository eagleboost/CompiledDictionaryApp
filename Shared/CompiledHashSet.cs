namespace Shared;

using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

/// <summary>
/// CompiledHashSet
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed partial class CompiledHashSet<T> : IReadOnlyCollection<T>, ISet<T>, IDeserializationCallback, ISerializable
{
  private readonly HashSet<T> _inner;
  private readonly Impl _impl;

  public CompiledHashSet(IEqualityComparer<T>? comparer = null)
  {
    _inner = new HashSet<T>(comparer);
    _impl = new Impl(_inner, comparer ?? EqualityComparer<T>.Default);
  }

  public void Compile() => _impl.Compile();
  
  public bool Contains(T item) => _impl.Contains(item);

  public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)_inner).GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  void ICollection<T>.Add(T item) => _inner.Add(item);

  public void ExceptWith(IEnumerable<T> other) => _inner.ExceptWith(other);

  public void IntersectWith(IEnumerable<T> other) => _inner.IntersectWith(other);

  bool ISet<T>.IsProperSubsetOf(IEnumerable<T> other) => _inner.IsProperSubsetOf(other);

  public bool IsProperSupersetOf(IEnumerable<T> other) => _inner.IsProperSupersetOf(other);

  public bool IsSubsetOf(IEnumerable<T> other) => _inner.IsSubsetOf(other);

  public bool IsSupersetOf(IEnumerable<T> other) => _inner.IsSupersetOf(other);

  public bool Overlaps(IEnumerable<T> other) => _inner.Overlaps(other);

  public bool SetEquals(IEnumerable<T> other) => _inner.SetEquals(other);

  public void SymmetricExceptWith(IEnumerable<T> other) => _inner.SymmetricExceptWith(other);

  public void UnionWith(IEnumerable<T> other) => _inner.UnionWith(other);

  public bool Add(T item) => _inner.Add(item);

  public void Clear() => _inner.Clear();
  
  public void CopyTo(T[] array, int arrayIndex) => _inner.CopyTo(array, arrayIndex);

  public bool Remove(T item) => _inner.Remove(item);

  int ICollection<T>.Count => _inner.Count;

  public bool IsReadOnly => ((ICollection<T>)_inner).IsReadOnly;

  int IReadOnlyCollection<T>.Count => _inner.Count;

  public void OnDeserialization(object sender) => _inner.Clear();

  public void GetObjectData(SerializationInfo info, StreamingContext context) => _inner.Clear();
}