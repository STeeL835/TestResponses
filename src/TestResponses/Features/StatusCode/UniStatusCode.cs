using System.Net;

namespace TestResponses;

/// <summary>
/// Represents an expected HTTP status code or range of status codes for TestResponse assertions.
/// </summary>
public record UniStatusCode
{
    /// <summary>
    /// Creates a UniStatusCode from a single <see cref="HttpStatusCode" /> value.
    /// </summary>
    public UniStatusCode(HttpStatusCode statusCode) : this((int) statusCode) { }
    
    /// <summary>
    /// Creates a UniStatusCode from a single numeric status code.
    /// </summary>
    public UniStatusCode(int statusCode) : this(statusCode..statusCode) { }

    /// <summary>
    /// Creates a UniStatusCode from a range of status codes.
    /// </summary>
    public UniStatusCode(Range statusCode)
    {
        if (statusCode.Start.IsFromEnd || statusCode.End.IsFromEnd)
            throw new ArgumentException("Expected status code can't use 'from end' index", nameof(statusCode));
        
        if (statusCode.Start.Value is < 100 or > 599 ||
            statusCode.End.Value is < 100 or > 599)
            throw new ArgumentException("Expected status codes must be within range of HTTP status codes (100-599)", nameof(statusCode));

        if (statusCode.Start.Value > statusCode.End.Value)
            throw new ArgumentException("Expected status codes range was inverted (start was bigger than end)", nameof(statusCode));
        
        Range = statusCode;
    }

    /// <summary>
    /// The underlying status code range represented by this instance.
    /// </summary>
    public Range Range { get; }

    /// <summary>
    /// Indicates whether the expected value represents a single status code.
    /// </summary>
    public bool IsSingleValue => Range.Start.Value == Range.End.Value;
    
    public static implicit operator UniStatusCode (HttpStatusCode statusCode) => new (statusCode);
    public static implicit operator UniStatusCode (int statusCode) => new (statusCode);
    public static implicit operator UniStatusCode (Range statusCodes) => new (statusCodes);
    
    /// <summary>
    /// Returns whether the given HTTP status code is included in the expected range.
    /// </summary>
    /// <param name="statusCode">HTTP status code to test.</param>
    public bool IsMatch(HttpStatusCode statusCode) => IsMatch((int)statusCode);

    /// <summary>
    /// Returns whether the given numeric HTTP status code is included in the expected range.
    /// </summary>
    /// <param name="statusCode">Numeric HTTP status code to test.</param>
    public bool IsMatch(int statusCode)
    {
        var startBoundarySatisfied = Range.Start.Value <= statusCode;
        var endBoundarySatisfied = statusCode <= Range.End.Value;

        return startBoundarySatisfied && endBoundarySatisfied;
    }

    public override string ToString() => IsSingleValue ? Range.Start.Value.ToString() : Range.ToString();
}