namespace TestClientResponse;

public record TestStringResponse(HttpResponseMessage Response) : TestResponse(Response)
{
    public string AsString { get; private set; } // TODO, TEST: exception if response is not read
    
    // TODO, TEST: Check the response type header?
    // TODO, TEST: What if response is a stream 

    public override async Task Read()
    {
        await ReadString();
    }

    protected async Task ReadString()
    {
        AsString = await Response.Content.ReadAsStringAsync();
        // TODO: cancellationTokens?
    }
}