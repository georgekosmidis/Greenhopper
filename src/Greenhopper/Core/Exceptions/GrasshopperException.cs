namespace Greenhopper.Core.Cache;


/// <summary>
/// Represents a Cache Manager exception.
/// </summary>
public class GreenhopperException : Exception
{
    /// <summary>
    /// Creates a new instance of the <see cref="GreenhopperException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public GreenhopperException(string? message) : base(message) { }

    /// <summary>
    /// Creates a new instance of the <see cref="GreenhopperException"/> class.
    /// </summary>
    public GreenhopperException()
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="GreenhopperException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused the cache manager exception.</param>
    public GreenhopperException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
