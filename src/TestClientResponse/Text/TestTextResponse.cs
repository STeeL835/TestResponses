namespace TestClientResponse.Text;

public class TestTextResponse(HttpResponseMessage httpResponse) : TestResponse(httpResponse)
{
    private string? _asText;
    
    /// <summary>
    /// Response as a text, exactly as received.
    /// If response is actually a text, string can't be null (for example 204 No Content reads an empty string)
    /// If it's null, response is not text (may be a stream)
    /// </summary>
    public string AsText => GetReadValue(_asText)!;

    internal override bool CanHandleContentType()
    {
        var contentMediaType = HttpResponse.Content.Headers.ContentType?.MediaType; // TODO: maybe a dedicated property?
        
        if (contentMediaType is null) return false;
        if (contentMediaType.StartsWith("text/")) return true;
        if (contentMediaType.Contains("json")) return true;
        if (contentMediaType.Contains("xml")) return true;
        return false;
    }

    protected override async Task ReadResponse()
    {
        await ReadText();
    }

    protected async Task<string> ReadText()
    {
        return _asText = await HttpResponse.Content.ReadAsStringAsync(); 
    }

    
    protected override string GetInfoString() => TestTextResponseFormatter.Format(this);
    
    // TODO, TEST: Check the response type header?
    // TODO, TEST: What if response is a stream 
}
