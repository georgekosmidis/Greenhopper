using Greenhopper.Core.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace Greenhopper.Core.Cache;

/// <summary>
///  Represents a local in-memory cache whose values are not serialized.
/// </summary>
public class MemoryCacheManager : ICacheManager, IDisposable
{
    private static SemaphoreSlim semaphore = new(1, 1);
    private IMemoryCache _cache;
    private bool _disposed;

    /// <summary>
    /// Creates a new instance of the <see cref="MemoryCacheManager"/> class.
    /// </summary>
    /// <param name="cache">The instance of an implementation of <see cref="IMemoryCache"/>.</param>
    public MemoryCacheManager(IMemoryCache cache)
    {
        ArgumentNullException.ThrowIfNull(cache);

        _cache = cache;
    }

    private string GetKey<T>(string key) => typeof(T).FullName + "-" + key;

    ///<inheritdoc/>
    public async Task<T> AddOrGetExisting<T>(string key, Func<Task<T>> task, int expirationSeconds = 5 * 60)
    {
        CheckDisposed();
        ExceptionExtensions.ThrowIfNull(task);
        ExceptionExtensions.ThrowIfNullOrWhiteSpace(key);
        ExceptionExtensions.ThrowIfOutsideBounds(expirationSeconds, 1, int.MaxValue);
        ExceptionExtensions.ThrowIfOutsideBounds(expirationSeconds, 1, int.MaxValue);

        key = GetKey<T>(key);

        var item = _cache.Get(key);
        if (item == default)
        {
            semaphore.Wait();
            try
            {
                item = _cache.Get(key);
                if (item == default)
                {
                    item = await task();
                    _cache.Set(key, item, DateTime.UtcNow.AddSeconds(expirationSeconds));
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        return (T)item!;

    }

    ///<inheritdoc/>
    public void Remove<T>(string key)
    {
        CheckDisposed();
        ExceptionExtensions.ThrowIfNullOrWhiteSpace(key);

        _cache.Remove(GetKey<T>(key));
    }

    ///<inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _cache.Dispose();
            }

            _disposed = true;
        }
    }

    ///<inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void CheckDisposed()
    {
        if (_disposed)
        {
            Throw();
        }

        static void Throw() => throw new ObjectDisposedException(typeof(MemoryCacheManager).FullName);
    }
}
