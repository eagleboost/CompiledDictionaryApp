namespace Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static System.Linq.Expressions.Expression;

public sealed partial class CompiledHashSet<T>
{
  private class Impl
  {
    private static readonly MethodInfo GetHashCodeMethod = typeof(IEqualityComparer<T>).GetMethod(nameof(GetHashCode))!;
    private readonly HashSet<T> _inner;
    private readonly IEqualityComparer<T> _comparer;
    private Func<T, bool> _contains;

    public Impl(HashSet<T> inner, IEqualityComparer<T> comparer)
    {
      _inner = inner;
      _comparer = comparer;
      Reset(out _contains);
    }
    
    public void Compile()
    {
      if (_inner.Count == 0)
      {
        Reset(out _contains);
        return;
      }

      CompileContains();
    }
    
    public bool Contains(T item) => _contains(item);
    
    public void Clear()
    {
      _inner.Clear();
      Reset(out _contains);
    }
    
    private void CompileContains()
    {
      var itemParameter = Parameter(typeof(T));
      var defaultCase = ReturnValue(false);
      var body = CreateSwitchBody(itemParameter, defaultCase, _ => ReturnValue(true));
      var lambda = Lambda<Func<T, bool>>(body, itemParameter);
      _contains = lambda.Compile();
      return;
      
      Expression ReturnValue(bool found) => Constant(found, typeof(bool));
    }
    
    private SwitchExpression CreateSwitchBody(Expression itemParameter, Expression defaultCase, Func<T, Expression> switchCaseBodyFunc)
    {
      // Expression that gets the item's hash code using the comparer
      var keyGetHashCodeCall = Call(Constant(_comparer), GetHashCodeMethod, itemParameter);
      
      return Switch(
        keyGetHashCodeCall, // switch condition
        defaultCase, // default case
        null, // use default comparer
        _inner // switch cases
          .GroupBy(p => _comparer.GetHashCode(p))
          .Select(g =>
          {
            if (g.Count() == 1)
            {
              return CreateSwitchCase(g.Key, g.Single());
            }

            return SwitchCase(
              Switch(
                itemParameter, // switch on the actual key
                defaultCase,
                null,
                g.Select(p => CreateSwitchCase(p, p))
              ),
              Constant(g.Key)
            );
          })
      );

      SwitchCase CreateSwitchCase(object hashCode, T value) => SwitchCase(switchCaseBodyFunc(value), Constant(hashCode));
    }
    
    private static void Reset(out Func<T, bool> contains)
    {
      contains = _ => false;
    }
  }
}