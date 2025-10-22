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

  private static readonly Dictionary<string, string> _asyncToSyncMethodMap = new()
  {
    [nameof(EntityFrameworkQueryableExtensions.FirstAsync)] = nameof(Queryable.First),
    [nameof(EntityFrameworkQueryableExtensions.FirstOrDefaultAsync)] = nameof(Queryable.FirstOrDefault),
    [nameof(EntityFrameworkQueryableExtensions.SingleAsync)] = nameof(Queryable.Single),
    [nameof(EntityFrameworkQueryableExtensions.SingleOrDefaultAsync)] = nameof(Queryable.SingleOrDefault),
    [nameof(EntityFrameworkQueryableExtensions.LastAsync)] = nameof(Queryable.Last),
    [nameof(EntityFrameworkQueryableExtensions.LastOrDefaultAsync)] = nameof(Queryable.LastOrDefault),
    [nameof(EntityFrameworkQueryableExtensions.AnyAsync)] = nameof(Queryable.Any),
    [nameof(EntityFrameworkQueryableExtensions.AllAsync)] = nameof(Queryable.All),
    [nameof(EntityFrameworkQueryableExtensions.CountAsync)] = nameof(Queryable.Count),
    [nameof(EntityFrameworkQueryableExtensions.LongCountAsync)] = nameof(Queryable.LongCount),
    [nameof(EntityFrameworkQueryableExtensions.MinAsync)] = nameof(Queryable.Min),
    [nameof(EntityFrameworkQueryableExtensions.MaxAsync)] = nameof(Queryable.Max),
    [nameof(EntityFrameworkQueryableExtensions.SumAsync)] = nameof(Queryable.Sum),
    [nameof(EntityFrameworkQueryableExtensions.AverageAsync)] = nameof(Queryable.Average),
    [nameof(EntityFrameworkQueryableExtensions.ToListAsync)] = nameof(Enumerable.ToList),
    [nameof(EntityFrameworkQueryableExtensions.ToArrayAsync)] = nameof(Enumerable.ToArray)
  };

  protected override Expression VisitMethodCall(MethodCallExpression node)
  {
    if (node.Method.DeclaringType == typeof(EntityFrameworkQueryableExtensions) &&
        _asyncToSyncMethodMap.TryGetValue(node.Method.Name, out var syncName))
    {
      var visitedArgs = node.Arguments.Select(Visit).ToArray();
      var asyncParameters = node.Method.GetParameters();

      var filteredArgs = new List<Expression>(visitedArgs.Length);
      for (var i = 0; i < visitedArgs.Length; i++)
      {
        if (asyncParameters[i].ParameterType == typeof(CancellationToken)) continue;
        filteredArgs.Add(visitedArgs[i]);
      }

      var syncMethod = ResolveSyncMethod(node.Method, syncName, filteredArgs);
      return Expression.Call(syncMethod, filteredArgs);
    }

    return base.VisitMethodCall(node);
  }

  private static MethodInfo ResolveSyncMethod(MethodInfo asyncMethod, string syncName, IReadOnlyList<Expression> arguments)
  {
    var genericArguments = asyncMethod.GetGenericArguments();

    var targetType = syncName switch
    {
      nameof(Enumerable.ToList) => typeof(Enumerable),
      nameof(Enumerable.ToArray) => typeof(Enumerable),
      _ => typeof(Queryable)
    };

    var methods = targetType
      .GetMethods(BindingFlags.Public | BindingFlags.Static)
      .Where(m => m.Name == syncName && m.GetGenericArguments().Length == genericArguments.Length)
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
