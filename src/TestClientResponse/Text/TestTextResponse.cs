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

    
    public override string ToString() => TestTextResponseFormatter.Format(this);
    
    // TODO, TEST: Check the response type header?
    // TODO, TEST: What if response is a stream 
}
