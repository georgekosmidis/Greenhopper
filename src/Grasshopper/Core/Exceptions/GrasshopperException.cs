namespace Grasshopper.Core.Cache;


/// <summary>
/// Represents a Cache Manager exception.
/// </summary>
public class GrasshopperException : Exception
{
    /// <summary>
    /// Creates a new instance of the <see cref="GrasshopperException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public GrasshopperException(string? message) : base(message) { }

    /// <summary>
    /// Creates a new instance of the <see cref="GrasshopperException"/> class.
    /// </summary>
    public GrasshopperException()
    {
    }

    /// <summary>
    /// Creates a new instance of the <see cref="GrasshopperException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The exception that caused the cache manager exception.</param>
    public GrasshopperException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
