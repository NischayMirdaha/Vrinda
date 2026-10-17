using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace VrindavanProfile.Web.Services.Caching;

public class CacheInvalidationInterceptor : DbCommandInterceptor
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CacheInvalidationInterceptor> _logger;

    public CacheInvalidationInterceptor(IMemoryCache cache, ILogger<CacheInvalidationInterceptor> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        InvalidateCacheIfNeeded(command.CommandText);
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        InvalidateCacheIfNeeded(command.CommandText);
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        InvalidateCacheIfNeeded(command.CommandText);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        InvalidateCacheIfNeeded(command.CommandText);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    private void InvalidateCacheIfNeeded(string commandText)
    {
        if (string.IsNullOrWhiteSpace(commandText)) return;

        var upperText = commandText.TrimStart().ToUpperInvariant();
        if (upperText.StartsWith("UPDATE") || 
            upperText.StartsWith("INSERT") || 
            upperText.StartsWith("DELETE"))
        {
            if (_cache is MemoryCache concreteCache)
            {
                _logger.LogInformation("Database mutation query detected ({CommandType}). Invalidating in-memory cache.", upperText.Substring(0, Math.Min(6, upperText.Length)));
                concreteCache.Clear();
            }
        }
    }
}
