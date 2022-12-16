using Greenhopper.Core.Cache;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Greenhopper.Core.Exceptions;

/// <summary>
/// Helper methods that throw exception based on a specific failed check.
/// </summary>
internal static class ExceptionExtensions
{

    /// <summary>
    /// Throws an exception if <paramref name="argument"/> 
    /// smaller than <paramref name="minInclusive"/> (inclusive > allowed) 
    /// or bigger than <paramref name="maxExclusive"/> (exclusive > not allowed).
    /// </summary>
    /// <param name="argument">The reference type argument to validate as non-null or filled with white spaces only.</param>
    /// <param name="minInclusive">The minimum inclusive allowed value for the <paramref name="argument"/>.</param>
    /// <param name="maxExclusive">The maximum exclusive allowed balue for the <paramref name="argument"/>.</param>
    /// <param name="paramName">The name of the parameter with which argument corresponds.</param>
    public static void ThrowIfOutsideBounds([NotNull] int? argument, int minInclusive, int maxExclusive, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIfNull(argument);

        if (argument.Value < minInclusive)
        {
            Throw($"{paramName} is smaller than {minInclusive}.");
        }
        if (argument.Value >= maxExclusive)
        {
            Throw($"{paramName} is bigger or equal to {maxExclusive}.");
        }
    }

    /// <summary>
    /// Throws an exception if <paramref name="argument"/> 
    /// smaller than <paramref name="minInclusive"/> (inclusive > allowed) 
    /// or bigger than <paramref name="maxExclusive"/> (exclusive > not allowed).
    /// </summary>
    /// <param name="argument">The reference type argument to validate as non-null or filled with white spaces only.</param>
    /// <param name="minInclusive">The minimum inclusive allowed value for the <paramref name="argument"/>.</param>
    /// <param name="maxExclusive">The maximum exclusive allowed balue for the <paramref name="argument"/>.</param>
    /// <param name="paramName">The name of the parameter with which argument corresponds.</param>
    public static void ThrowIfOutsideBounds([NotNull] DateTimeOffset? argument, DateTimeOffset minInclusive, DateTimeOffset maxExclusive, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIfNull(argument);

        if (argument.Value < minInclusive)
        {
            Throw($"{paramName} is smaller than {minInclusive}.");
        }
        if (argument.Value >= maxExclusive)
        {
            Throw($"{paramName} is bigger or equal to {maxExclusive}.");
        }
    }

    /// <summary>
    /// Throws an exception if <paramref name="argument"/> is null or contains white spaces only.
    /// </summary>
    /// <param name="argument">The reference type argument to validate as non-null or filled with white spaces only.</param>
    /// <param name="paramName">The name of the parameter with which argument corresponds.</param>
    public static void ThrowIfNullOrWhiteSpace([NotNull] string? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (string.IsNullOrWhiteSpace(argument))
        {
            Throw($"{paramName} is null or filled with whitespaces only.");
        }
    }

    /// <summary>
    /// Throws an exception if <paramref name="argument"/> is null or default.
    /// </summary>
    /// <param name="argument">The reference type argument to validate as non-null.</param>
    /// <param name="paramName">The name of the parameter with which argument corresponds.</param>
    public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        if (argument is null)
        {
            Throw($"{paramName} is null.");
        }
    }

    /// <summary>
    /// Throws an exception if the <paramref name="argument"/> is an empty list.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="argument">The reference type argument to validate as a non-empty list.</param>
    /// <param name="paramName">The name of the parameter with which argument corresponds.</param>
    public static void ThrowIfNullOrEmpty<T>([NotNull] IEnumerable<T>? argument, [CallerArgumentExpression("argument")] string? paramName = null)
    {
        ThrowIfNull(argument, paramName);

        if (!argument.Any())
        {
            Throw($"{paramName} don't has any items.");
        }
    }



    /// <summary>
    /// Throws an <seealso cref="GreenhopperException"/> when called.
    /// </summary>
    /// <param name="message">The message for the exception.</param>
    /// <exception cref="GreenhopperException">A generic exception for every <see cref="ExceptionExtensions"/> method.</exception>
    [DoesNotReturn]
    private static void Throw(string? message)
    {
        throw new GreenhopperException(message);
    }
}
