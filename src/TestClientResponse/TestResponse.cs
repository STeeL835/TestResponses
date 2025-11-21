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

    public TestResponseStatusCode? ExpectedStatusCodes { get; set; }
    
    public void AssertExpectedStatusCode()
    {
        if (ExpectedStatusCodes is null) throw new TestResponseException("Expected status codes are not set"); 
        
        AssertStatusCode(ExpectedStatusCodes);
    }

    public void AssertStatusCode(TestResponseStatusCode expectedStatusCode)
    {
        if (!expectedStatusCode.IsMatch(StatusCode))
        {
            ThrowAssertionException(expectedStatusCode.IsSingleValue 
                ? $"Response status code is not {expectedStatusCode}"
                : $"Response status code is not in range {expectedStatusCode}");
        }
    }

    #endregion
    
    [DoesNotReturn]
    protected abstract void ThrowAssertionException(string message, Exception? innerException = null);
}