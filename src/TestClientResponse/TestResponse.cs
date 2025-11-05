using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace TestClientResponse;

public abstract record TestResponse(HttpResponseMessage HttpResponse)
{
    #region Read

    public bool IsRead { get; protected set; }

    public abstract Task Read();

    protected T GetReadValue<T>(T value) 
    {
        if (IsRead) return value;
        throw new TestResponseException($"Response is not read. Call {nameof(Read)}() before accessing response content");
    }

    #endregion

    #region StatusCode

    public HttpStatusCode StatusCode => HttpResponse.StatusCode;

    public void ShouldHaveStatusCode(HttpStatusCode expectedStatusCode) => ShouldHaveStatusCode((int)expectedStatusCode);

    public void ShouldHaveStatusCode(int expectedStatusCode)
    {
        if (expectedStatusCode is < 100 or > 599)
            throw new ArgumentException("Expected status code must be within range of HTTP status codes (100-599)", nameof(expectedStatusCode));

        if ((int)StatusCode != expectedStatusCode)
            ThrowAssertionException($"Response status code is not {expectedStatusCode}"); // TODO: add default reason phrases maybe
    }

    public void ShouldHaveStatusCode(Range expectedStatusCodes)
    {
        if (expectedStatusCodes.Start.IsFromEnd || expectedStatusCodes.End.IsFromEnd)
            throw new ArgumentException("Expected status code can't use 'from end' index", nameof(expectedStatusCodes));
        
        if (expectedStatusCodes.Start.Value is < 100 or > 599 ||
            expectedStatusCodes.End.Value is < 100 or > 599)
            throw new ArgumentException("Expected status codes must be within range of HTTP status codes (100-599)", nameof(expectedStatusCodes));

        if (expectedStatusCodes.Start.Value > expectedStatusCodes.End.Value)
            throw new ArgumentException("Expected status codes range was inverted (start was bigger than end", nameof(expectedStatusCodes));
        
        var statusCodeInt = (int)StatusCode;
        var startBoundarySatisfied = expectedStatusCodes.Start.Value <= statusCodeInt;
        var endBoundarySatisfied = statusCodeInt <= expectedStatusCodes.End.Value;
        
        if (!(startBoundarySatisfied && endBoundarySatisfied))
            ThrowAssertionException($"Response status code is not in range {expectedStatusCodes}");
    }

    #endregion
    
    [DoesNotReturn]
    protected abstract void ThrowAssertionException(string message);
}