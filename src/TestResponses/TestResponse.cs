using System.Diagnostics.CodeAnalysis;
using System.Net;
using TestResponses.Features;

namespace TestResponses;

public abstract class TestResponse(HttpResponseMessage httpResponse)
{
    public HttpResponseMessage HttpResponse { get; } = httpResponse;
    public HttpStatusCode StatusCode => HttpResponse.StatusCode;
    protected string? ContentType => HttpResponse.Content.Headers.ContentType?.MediaType;
    
    #region Read

    public bool IsRead { get; protected set; }

    private TestResponse? BestFitResponse { get; set; }
    
    public async Task Read()
    {
        if (IsRead) return; // TODO: Test it's not read twice (with custom testResponse) that throws exception if read twice
        
        // to be able to read http response twice (when response structure fits different class)
        await HttpResponse.Content.LoadIntoBufferAsync();
        
        await ReadResponse();
        
        if (!CanHandleContent()) 
            BestFitResponse = await TestResponseDetector.ReadAsBestFitResponse(HttpResponse);
        
        IsRead = true; 
    }
    
    // does not constrain the testResponse from trying to read response anyway, but helps to find one that can read the response if suddenly content is not what is expected.
    protected abstract Task ReadResponse();
    internal abstract bool CanHandleContent();
    
    #endregion

    #region Assertions

    public void AssertValid(UniStatusCode? withStatusCode = null)
    {
        if (!IsRead) ThrowAssertionException("Can't assert validity because response is not read");
        
        if (withStatusCode is not null) AssertStatusCode(withStatusCode);
        else if (ExpectedStatusCode is not null) AssertExpectedStatusCode();
        
        AssertExpectedContentType();
        AssertResponseSchema();
    }
    
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

    private void AssertExpectedContentType()
    {
        if (BestFitResponse is not null) 
            ThrowAssertionException($"{GetType().Name} didn't expect content-type '{HttpResponse.Content.Headers.ContentType}'");
    }

    protected virtual void AssertResponseSchema() { /* assume response has no schema by default */ }
    
    [DoesNotReturn]
    private void ThrowAssertionException(string message, Exception? innerException = null)
    {
        throw new TestResponseAssertionException($"{message}\n{ToString()}", innerException);
    }
    
    #endregion

    public override string ToString() => BestFitResponse?.ToString() ?? GetInfoString();
    protected virtual string GetInfoString() => TestResponseFormatter.Format(this);
}