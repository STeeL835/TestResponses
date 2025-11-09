using System.Diagnostics.CodeAnalysis;

namespace TestClientResponse.Text;

public record TestTextResponse(HttpResponseMessage HttpResponse) : TestResponse(HttpResponse)
{
    private string? _asText;
    
    /// <summary>
    /// Response as a text, exactly as received.
    /// If response is actually a text, string can't be null (for example 204 No Content reads an empty string)
    /// If it's null, response is not text (may be a stream)
    /// </summary>
    public string AsText => GetReadValue(_asText)!;

    protected override async Task ReadResponse()
    {
        await ReadText();
    }

    protected async Task<string> ReadText()
    {
        return _asText = await HttpResponse.Content.ReadAsStringAsync(); 
    }

    [DoesNotReturn]
    protected override void ThrowAssertionException(string message, Exception? innerException = null)
    {
        throw new TestTextResponseAssertionException(this, message, innerException);
    }

    // TODO, TEST: Check the response type header?
    // TODO, TEST: What if response is a stream 
}
