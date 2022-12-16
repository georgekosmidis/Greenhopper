namespace Greenhopper.Core.Cache;


/// <summary>
/// Represents a Cache Manager exception.
/// </summary>
public class CacheException : Exception
{
    /// <summary>
    /// Creates a new instance of the <see cref="GreenhopperException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public CacheException(string message) : base(message) { }

    /// <summary>
    /// Creates a new instance of the <see cref="GreenhopperException"/> class.
    /// </summary>
    public CacheException()
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="GreenhopperException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused the cache manager exception.</param>
    public CacheException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
