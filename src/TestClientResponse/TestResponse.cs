using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace TestClientResponse;

public abstract record TestResponse(HttpResponseMessage HttpResponse)
{
    #region Read

    public bool IsRead { get; protected set; }

    public async Task Read()
    {
        if (IsRead) return; // TODO: Test it's not read twice
        await ReadResponse();
        IsRead = true; 
    }
    
    protected abstract Task ReadResponse();

    #endregion

    #region Get values

    protected T GetReadValue<T>(T value) 
    {
        if (!IsRead) throw new TestResponseException($"Response is not read. Call {nameof(Read)}() before accessing response content");
        return value;
    }
    
    protected T? GetReadValue<T>(ValueReadResult<T> value) 
    {
        if (!IsRead) 
            throw new TestResponseException($"Response is not read. Call {nameof(Read)}() before accessing response content");
        
        if (!value.IsReadSuccessfully) 
            ThrowAssertionException($"Response could not be read as {typeof(T).Name} (see inner exception)", value.ExceptionHappenedDuringRead);
        
        return value.Value;
    }

    #endregion

    #region Status code

    public HttpStatusCode StatusCode => HttpResponse.StatusCode;

    #region Expected status codes

    private Range? _expectedStatusCodes;
    public Range? ExpectedStatusCodes
    {
        get => _expectedStatusCodes;
        set 
        {
            if (value.HasValue) ValidateStatusCodeRange(value.Value);
            _expectedStatusCodes = value; 
        }
    } 
    
    public int ExpectedStatusCode
    {
        set => ExpectedStatusCodes = value..value;
    }

    
    public void ShouldHaveExpectedStatusCode()
    {
        if (!_expectedStatusCodes.HasValue) throw new TestResponseException("Expected status codes are not set"); 
        
        ShouldHaveStatusCode(_expectedStatusCodes.Value);
    }

    #endregion

    #region Status code assertions

    public void ShouldHaveStatusCode(HttpStatusCode expectedStatusCode) => ShouldHaveStatusCode((int)expectedStatusCode);

    public void ShouldHaveStatusCode(int expectedStatusCode)
    {
        if (expectedStatusCode is < 100 or > 599)
            throw new ArgumentException("Expected status code must be within range of HTTP status codes (100-599)", nameof(expectedStatusCode));

        if ((int)StatusCode != expectedStatusCode)
            ThrowAssertionException($"Response status code is not {expectedStatusCode}");
    }

    public void ShouldHaveStatusCode(Range expectedStatusCodes)
    {
        ValidateStatusCodeRange(expectedStatusCodes);

        var statusCodeInt = (int)StatusCode;
        var startBoundarySatisfied = expectedStatusCodes.Start.Value <= statusCodeInt;
        var endBoundarySatisfied = statusCodeInt <= expectedStatusCodes.End.Value;
        
        if (!(startBoundarySatisfied && endBoundarySatisfied))
            ThrowAssertionException($"Response status code is not in range {expectedStatusCodes}");
    }

    private static void ValidateStatusCodeRange(Range statusCodeRange)
    {
        if (statusCodeRange.Start.IsFromEnd || statusCodeRange.End.IsFromEnd)
            throw new ArgumentException("Expected status code can't use 'from end' index", nameof(statusCodeRange));
        
        if (statusCodeRange.Start.Value is < 100 or > 599 ||
            statusCodeRange.End.Value is < 100 or > 599)
            throw new ArgumentException("Expected status codes must be within range of HTTP status codes (100-599)", nameof(statusCodeRange));

        if (statusCodeRange.Start.Value > statusCodeRange.End.Value)
            throw new ArgumentException("Expected status codes range was inverted (start was bigger than end)", nameof(statusCodeRange));
    }

    #endregion

    #endregion
    
    [DoesNotReturn]
    protected abstract void ThrowAssertionException(string message, Exception? innerException = null);
}