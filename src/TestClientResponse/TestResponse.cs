using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace TestClientResponse;

/* It goes like this
    - wrap http response in TestResponse
        - manually with ctor
        - or with ReadAs
    - Read response
      shouldn't fail here because negative cases are valid for us 
        - manually
        - or ReadAs already did it
    - Use it
        - assert it hasn't failed and has correct structure at least (status code and response structure assertion
        - or check status code manually
        - or check positive case 
            - response object assertions (structure assertion kicks in here)
        - or check negative case (like returning 404)
            - try to get negative case model for assertions (structure assertion kicks in here)
        - OR use HttpResponseMessage if TestResponse can't help
        
    So, when should we detect response structure to use correct type? 
 */

public abstract class TestResponse
{
    public TestResponse(HttpResponseMessage httpResponse) => HttpResponse = httpResponse;
    public HttpResponseMessage HttpResponse { get; }
    public HttpStatusCode StatusCode => HttpResponse.StatusCode;
    
    #region Read

    public bool IsRead { get; protected set; }

    private TestResponse? BestFitResponse { get; set; }
    
    public async Task Read()
    {
        if (IsRead) return; // TODO: Test it's not read twice (with custom testResponse) that throws exception if read twice
        
        // to be able to read http response twice (when response structure fits different class)
        await HttpResponse.Content.LoadIntoBufferAsync();
        
        await ReadResponse();
        
        if (!CanHandleContentType()) 
            BestFitResponse = await TestResponseDetector.ReadAsBestFitResponse(HttpResponse);
        
        IsRead = true; 
    }
    
    // does not constrain the testResponse from trying to read response anyway, but helps to find one that can read the response if suddenly content is not what is expected.
    internal abstract bool CanHandleContentType();
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

    #region Assertions

    public void AssertValid(UniStatusCode? withStatusCode = null)
    {
        if (!IsRead) ThrowAssertionException("Can't assert validity because response is not read");
        
        if (withStatusCode is not null) AssertStatusCode(withStatusCode);
        else if (ExpectedStatusCode is not null) AssertExpectedStatusCode();
        
        AssertExpectedContentType();
        //TODO: AssertResponseSchema - jsons for example can have deserialization problems
    }
    
    #region Status code

    public UniStatusCode? ExpectedStatusCode { get; set; }

    public void AssertExpectedStatusCode()
    {
        if (ExpectedStatusCode is null) throw new TestResponseException("Expected status codes are not set"); 
        
        AssertStatusCode(ExpectedStatusCode);
    }

    public void AssertStatusCode(UniStatusCode expectedStatusCode)
    {
        if (!expectedStatusCode.IsMatch(StatusCode))
        {
            ThrowAssertionException(expectedStatusCode.IsSingleValue 
                ? $"Response status code is not {expectedStatusCode}"
                : $"Response status code is not in range {expectedStatusCode}");
        }
    }

    #endregion
    
    private void AssertExpectedContentType()
    {
        if (BestFitResponse is not null) 
            ThrowAssertionException($"{GetType().Name} didn't expect content-type '{HttpResponse.Content.Headers.ContentType}'");
    }
    
    [DoesNotReturn]
    private void ThrowAssertionException(string message, Exception? innerException = null)
    {
        throw new TestResponseAssertionException($"{message}\n{ToString()}", innerException);
    }
    
    #endregion

    public override string ToString() => BestFitResponse?.ToString() ?? GetInfoString();
    protected virtual string GetInfoString() => TestResponseFormatter.Format(this);
}