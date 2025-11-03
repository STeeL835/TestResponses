namespace TestClientResponse.String;

public record TestStringResponse(HttpResponseMessage Response) : TestResponse(Response)
{
    private string? _asString;
    public string? AsString => GetReadValue(_asString);
    
    public override async Task Read()
    {
        await ReadString();
        IsRead = true; 
    }

    protected async Task ReadString()
    {
        _asString = await Response.Content.ReadAsStringAsync();
        // TODO: cancellationTokens?
    }
    
    // TODO, TEST: Check the response type header?
    // TODO, TEST: What if response is a stream 
    // TODO, TEST: Response as string is always an empty string, is it?
}