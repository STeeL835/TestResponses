namespace TestResponses;

/// <summary>
/// Response formatter that uses a delegate to format response info.
/// Useful if formatting is too simple for a dedicated class.  
/// </summary>
/// <param name="format">A delegate that will format provided response.</param>
/// <typeparam name="TResponse">A<see cref="TestResponse"/> type</typeparam>
public class TestResponseDelegateFormatter<TResponse>(Func<TResponse, string> format) 
    : ITestResponseFormatter<TResponse> 
    where TResponse : TestResponse
{
    /// <summary>
    /// Formats the response using the provided delegate.
    /// </summary>
    public string Format(TResponse response) => format(response);
}