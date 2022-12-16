using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Greenhopper.Core.Cache;

/// <summary>
///  An abstraction of the Cache Manager required.
/// </summary>
public interface ICacheManager
{
    /// <summary>
    /// Gets the cache entry associated with this key, if present. 
    /// If not, it executes the encapsulated async <paramref name="task"/>,
    /// and stores the result.
    /// </summary>
    /// <typeparam name="T">The type of the object to be cached.</typeparam>
    /// <param name="key">The associated key of the cache entry.</param>
    /// <param name="task">The async encapsulated method that returns the object to be cached. 
    /// It is executed only when a cached entry under the given <paramref name="key"/> is not present.</param>
    /// <param name="expirationSeconds">Sliding expiration in seconds</param>
    /// <remarks>
    /// <see cref="MemoryCache"/> is a non distributed implementation of <see cref="IMemoryCache"/>,
    /// that requires to be added to the <see cref="IServiceCollection"/> with <see cref="MemoryCacheServiceCollectionExtensions.AddMemoryCache(IServiceCollection)"/>
    /// </remarks>
    /// <returns>Returns an object of type <typeparamref name="T"/> associated with the <paramref name="key"/>. </returns>
    Task<T> AddOrGetExisting<T>(string key, Func<Task<T>> task, int expirationSeconds = 5 * 60);

    /// <summary>
    /// Removes the object associated with the given key.
    /// </summary>
    /// <typeparam name="T">The type of the object to be cached.</typeparam>
    /// <param name="key">The associated key of the cached entry.</param>
    void Remove<T>(string key);
}