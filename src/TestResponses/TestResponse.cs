using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace TestResponses;

/// <summary>
/// Base type for all test response wrappers.
/// It reads an <see cref="HttpResponseMessage" />, performs response validation, and provides
/// fallback best-fit detection for fail message when content does not match the expected response type.
/// </summary>
public abstract class TestResponse(HttpResponseMessage httpResponse)
{
    /// <summary>
    /// The underlying HTTP response message.
    /// </summary>
    public HttpResponseMessage HttpResponse { get; } = httpResponse;

    /// <summary>
    /// The HTTP status code returned by the response.
    /// </summary>
    public HttpStatusCode StatusCode => HttpResponse.StatusCode;

    /// <summary>
    /// The media type of the response content, or <c>null</c> if none is provided.
    /// </summary>
    public string? ContentType => HttpResponse.Content.Headers.ContentType?.MediaType;
    
    #region Read

    /// <summary>
    /// Indicates whether the response content has already been read.
    /// </summary>
    public bool IsRead { get; protected set; }

    private TestResponse? BestFitResponse { get; set; }
    
    /// <summary>
    /// Reads response content into the current TestResponse instance.
    /// This method is idempotent and may be called multiple times.
    /// </summary>
    public async Task Read()
    {
        if (IsRead) return;
        
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

    /// <summary>
    /// Validates the response content and status code.
    /// </summary>
    /// <param name="withStatusCode">Optional expected status code or range to assert against.</param>
    /// <exception cref="TestResponseAssertionException">Thrown if the response does not match the expected status code, content type, or schema.</exception>
    public void AssertValid(UniStatusCode? withStatusCode = null)
    {
        if (!IsRead) ThrowAssertionException("Can't assert validity because response is not read");
        
        if (withStatusCode is not null) AssertStatusCode(withStatusCode);
        else if (ExpectedStatusCode is not null) AssertExpectedStatusCode();
        
        AssertExpectedContentType();
        AssertResponseSchema();
    }
    
    /// <summary>
    /// Expected HTTP status code or range for this response.
    /// </summary>
    public UniStatusCode? ExpectedStatusCode { get; set; }

    /// <summary>
    /// Asserts that the response matches the configured expected status code.
    /// </summary>
    /// <exception cref="TestResponseException">Thrown if expected status codes are not set.</exception>
    /// <exception cref="TestResponseAssertionException">Thrown if the response status code does not match the expected value or range.</exception>
    public void AssertExpectedStatusCode()
    {
        if (ExpectedStatusCode is null) throw new TestResponseException("Expected status codes are not set"); 
        
        AssertStatusCode(ExpectedStatusCode);
    }

    /// <summary>
    /// Asserts that the response status code matches the provided value or range.
    /// </summary>
    /// <param name="expectedStatusCode">Expected status code or range.</param>
    /// <exception cref="TestResponseAssertionException">Thrown if the response status code does not match the expected value or range.</exception>
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

    /// <summary>
    /// Returns a user-friendly representation of the response.
    /// </summary>
    public override string ToString() => BestFitResponse?.ToString() ?? GetInfoString();

    protected virtual string GetInfoString() => TestResponseFormatter.Format(this);
}