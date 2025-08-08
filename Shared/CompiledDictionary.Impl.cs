namespace Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AgileObjects.ReadableExpressions;
using static System.Linq.Expressions.Expression;

public partial class CompiledDictionary<TKey, TValue>
{
  private class Impl
  {
    private static readonly MethodInfo GetHashCodeMethod = typeof(IEqualityComparer<TKey>).GetMethod(nameof(GetHashCode))!; 
    private readonly IDictionary<TKey, TValue> _inner;
    private readonly IEqualityComparer<TKey> _comparer;
    private static readonly Type TypeValue = typeof(TValue);
    private delegate bool TryGetValueDelegate(TKey value, out TValue result);
    
    private Func<TKey, TValue> _lookup ;
    private TryGetValueDelegate _tryGetValue;
    private Func<TKey, bool> _containsKey;

    public Impl(IDictionary<TKey, TValue> inner, IEqualityComparer<TKey> comparer)
    {
      _inner = inner;
      _comparer = comparer;
      Reset(out _lookup, out _tryGetValue, out _containsKey);
    }
    
    public void Compile()
    {
      if (_inner.Count == 0)
      {
        Reset(out _lookup, out _tryGetValue, out _containsKey);
        return;
      }

      CompileLookup();
      CompileTryGetValue();
      CompileContainsKey();
    }

    public bool TryGetValue(TKey key, out TValue value) => _tryGetValue(key, out value);
    
    public TValue this[TKey key] => _lookup(key);

    public bool ContainsKey(TKey key) => _containsKey(key);
    
    public void Clear()
    {
      _inner.Clear();
      Reset(out _lookup, out _tryGetValue, out _containsKey);
    }
    
    private void CompileLookup()
    {
      var keyParameter = Parameter(typeof(TKey));
      var defaultCase = ThrowException();
      var body = CreateSwitchBody(keyParameter, defaultCase, v => Constant(v, TypeValue));
      var lambda = Lambda<Func<TKey, TValue>>(body, keyParameter);
      var str = lambda.ToReadableString();
      _lookup = lambda.Compile();
      return;

      UnaryExpression ThrowException()
      {
        var keyToStringCall = Call(keyParameter, typeof(object).GetMethod(nameof(ToString))!);
        var exceptionCtor = typeof(KeyNotFoundException).GetConstructor([typeof(string)]);
        var unaryExpression = Throw(New(exceptionCtor!, keyToStringCall), TypeValue);
        return unaryExpression;
      }
    }

    private void CompileTryGetValue()
    {
      var keyParameter = Parameter(typeof(TKey));
      var valueParameter = Parameter(TypeValue.MakeByRefType());
      var defaultCase = ReturnValueBlock(false, default!);
      var body = CreateSwitchBody(keyParameter, defaultCase, v => ReturnValueBlock(true, v));
      var lambda = Lambda<TryGetValueDelegate>(body, keyParameter, valueParameter);
      _tryGetValue = lambda.Compile();
      return;

      BlockExpression ReturnValueBlock(bool found, TValue value)
      {
        return Block(Assign(valueParameter, Constant(value, TypeValue)), Constant(found));
      }
    }

    private void CompileContainsKey()
    {
      var keyParameter = Parameter(typeof(TKey));
      var defaultCase = ReturnValue(false);
      var body = CreateSwitchBody(keyParameter, defaultCase, _ => ReturnValue(true));
      var lambda = Lambda<Func<TKey, bool>>(body, keyParameter);
      _containsKey = lambda.Compile();
      return;

      Expression ReturnValue(bool found) => Constant(found, typeof(bool));
    }
    
    private SwitchExpression CreateSwitchBody(Expression keyParameter, Expression defaultCase, Func<TValue, Expression> switchCaseBodyFunc)
    {
      // Expression that gets the key's hash code using the comparer
      var keyGetHashCodeCall = Call(Constant(_comparer), GetHashCodeMethod, keyParameter);
      
      return Switch(
        keyGetHashCodeCall, // switch condition
        defaultCase, // default case
        null, // use default comparer
        _inner // switch cases
          .GroupBy(p => _comparer.GetHashCode(p.Key))
          .Select(g =>
          {
            if (g.Count() == 1)
            {
              return CreateSwitchCase(g.Key, g.Single().Value);
            }

            return SwitchCase(
              Switch(
                keyParameter, // switch on the actual key
                defaultCase,
                null,
                g.Select(p => CreateSwitchCase(p.Key, p.Value))
              ),
              Constant(g.Key)
            );
          })
      );

      SwitchCase CreateSwitchCase(object hashCode, TValue value) => SwitchCase(switchCaseBodyFunc(value), Constant(hashCode));
    }

    private static void Reset(out Func<TKey, TValue> lookup, out TryGetValueDelegate tryGetValue, out Func<TKey, bool> containsKey)
    {
      lookup = _ => throw new KeyNotFoundException("Dictionary is empty");
      tryGetValue = (_, out value) => { value = default!; return false; };
      containsKey = _ => false;
    }
  }
}