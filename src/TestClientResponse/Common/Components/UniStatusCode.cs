using System.Net;

namespace TestClientResponse;

public record UniStatusCode
{
    public UniStatusCode(HttpStatusCode statusCode) : this((int) statusCode) { }
    
    public UniStatusCode(int statusCode) : this(statusCode..statusCode) { }

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

    public Range Range { get; }
    public bool IsSingleValue => Range.Start.Value == Range.End.Value;
    
    public static implicit operator UniStatusCode (HttpStatusCode statusCode) => new (statusCode);
    public static implicit operator UniStatusCode (int statusCode) => new (statusCode);
    public static implicit operator UniStatusCode (Range statusCodes) => new (statusCodes);
    
    public bool IsMatch(HttpStatusCode statusCode) => IsMatch((int)statusCode);

    public bool IsMatch(int statusCode)
    {
        var startBoundarySatisfied = Range.Start.Value <= statusCode;
        var endBoundarySatisfied = statusCode <= Range.End.Value;

        return startBoundarySatisfied && endBoundarySatisfied;
    }

    public override string ToString() => IsSingleValue ? Range.Start.Value.ToString() : Range.ToString();
}