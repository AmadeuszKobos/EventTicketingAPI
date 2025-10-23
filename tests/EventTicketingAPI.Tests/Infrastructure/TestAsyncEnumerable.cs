using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace EventTicketingAPI.Tests.Infrastructure;

internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
{
  public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
  public TestAsyncEnumerable(Expression expression) : base(expression) { }

  public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
      new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

  IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
}

internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
{
  private readonly IEnumerator<T> _inner;

  public TestAsyncEnumerator(IEnumerator<T> inner) => _inner = inner;

  public ValueTask DisposeAsync()
  {
    _inner.Dispose();
    return ValueTask.CompletedTask;
  }

  public ValueTask<bool> MoveNextAsync() => new(_inner.MoveNext());

  public T Current => _inner.Current;
}

internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
{
  private readonly IQueryProvider _inner;

  public TestAsyncQueryProvider(IQueryProvider inner) => _inner = inner;

  public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);

  public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
      new TestAsyncEnumerable<TElement>(expression);

  public object? Execute(Expression expression) => _inner.Execute(expression);

  public TResult Execute<TResult>(Expression expression)
  {
    var rewritten = AsyncMethodRewriter.Default.Visit(expression);
    return _inner.Execute<TResult>(rewritten);
  }

  public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default) =>
      Task.FromResult(Execute<TResult>(expression));

  public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) =>
      new TestAsyncEnumerable<TResult>(expression);
}

internal sealed class AsyncMethodRewriter : ExpressionVisitor
{
  public static AsyncMethodRewriter Default { get; } = new();

  private sealed record SyncTarget(Type TargetType, string MethodName, bool ConvertSourceToEnumerable = false);

  private static readonly Dictionary<string, SyncTarget> _asyncToSyncMethodMap = new()
  {
    [nameof(EntityFrameworkQueryableExtensions.FirstAsync)] = new(typeof(Queryable), nameof(Queryable.First)),
    [nameof(EntityFrameworkQueryableExtensions.FirstOrDefaultAsync)] = new(typeof(Queryable), nameof(Queryable.FirstOrDefault)),
    [nameof(EntityFrameworkQueryableExtensions.SingleAsync)] = new(typeof(Queryable), nameof(Queryable.Single)),
    [nameof(EntityFrameworkQueryableExtensions.SingleOrDefaultAsync)] = new(typeof(Queryable), nameof(Queryable.SingleOrDefault)),
    [nameof(EntityFrameworkQueryableExtensions.LastAsync)] = new(typeof(Queryable), nameof(Queryable.Last)),
    [nameof(EntityFrameworkQueryableExtensions.LastOrDefaultAsync)] = new(typeof(Queryable), nameof(Queryable.LastOrDefault)),
    [nameof(EntityFrameworkQueryableExtensions.AnyAsync)] = new(typeof(Queryable), nameof(Queryable.Any)),
    [nameof(EntityFrameworkQueryableExtensions.AllAsync)] = new(typeof(Queryable), nameof(Queryable.All)),
    [nameof(EntityFrameworkQueryableExtensions.CountAsync)] = new(typeof(Queryable), nameof(Queryable.Count)),
    [nameof(EntityFrameworkQueryableExtensions.LongCountAsync)] = new(typeof(Queryable), nameof(Queryable.LongCount)),
    [nameof(EntityFrameworkQueryableExtensions.MinAsync)] = new(typeof(Queryable), nameof(Queryable.Min)),
    [nameof(EntityFrameworkQueryableExtensions.MaxAsync)] = new(typeof(Queryable), nameof(Queryable.Max)),
    [nameof(EntityFrameworkQueryableExtensions.SumAsync)] = new(typeof(Queryable), nameof(Queryable.Sum)),
    [nameof(EntityFrameworkQueryableExtensions.AverageAsync)] = new(typeof(Queryable), nameof(Queryable.Average)),
    [nameof(EntityFrameworkQueryableExtensions.ToListAsync)] = new(typeof(Enumerable), nameof(Enumerable.ToList), true),
    [nameof(EntityFrameworkQueryableExtensions.ToArrayAsync)] = new(typeof(Enumerable), nameof(Enumerable.ToArray), true)
  };

  protected override Expression VisitMethodCall(MethodCallExpression node)
  {
    if (node.Method.DeclaringType == typeof(EntityFrameworkQueryableExtensions) &&
        _asyncToSyncMethodMap.TryGetValue(node.Method.Name, out var syncTarget))
    {
      var visitedArgs = node.Arguments.Select(Visit).ToArray();
      var asyncParameters = node.Method.GetParameters();

      var filteredArgs = new List<Expression>(visitedArgs.Length);
      for (var i = 0; i < visitedArgs.Length; i++)
      {
        if (asyncParameters[i].ParameterType == typeof(CancellationToken)) continue;
        filteredArgs.Add(visitedArgs[i]);
      }

      if (syncTarget.ConvertSourceToEnumerable && filteredArgs.Count > 0)
      {
        var generics = node.Method.GetGenericArguments();
        filteredArgs[0] = Expression.Call(
          typeof(Queryable),
          nameof(Queryable.AsEnumerable),
          generics,
          filteredArgs[0]);
      }

      var syncMethod = ResolveSyncMethod(node.Method, syncTarget, filteredArgs);
      return Expression.Call(syncMethod, filteredArgs);
    }

    return base.VisitMethodCall(node);
  }

  private static MethodInfo ResolveSyncMethod(MethodInfo asyncMethod, SyncTarget syncTarget, IReadOnlyList<Expression> arguments)
  {
    var genericArguments = asyncMethod.GetGenericArguments();

    var methods = syncTarget.TargetType
      .GetMethods(BindingFlags.Public | BindingFlags.Static)
      .Where(m => m.Name == syncTarget.MethodName && m.GetGenericArguments().Length == genericArguments.Length)
      .Select(m => m.IsGenericMethod ? m.MakeGenericMethod(genericArguments) : m);

    foreach (var method in methods)
    {
      var parameters = method.GetParameters();
      if (parameters.Length != arguments.Count) continue;

      var compatible = true;
      for (var i = 0; i < parameters.Length; i++)
      {
        if (!parameters[i].ParameterType.IsAssignableFrom(arguments[i].Type))
        {
          compatible = false;
          break;
        }
      }

      if (compatible)
        return method;
    }

    throw new InvalidOperationException($"Unable to locate synchronous counterpart for {asyncMethod.Name}.");
  }
}
