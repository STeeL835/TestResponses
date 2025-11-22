namespace TestClientResponse.Empty;

public record TestEmptyResponse(HttpResponseMessage HttpResponse) : TestResponse(HttpResponse)
{
    protected override Task ReadResponse() => Task.CompletedTask;

    public override string ToString() => $"""
        {TestResponseFormatter.FormatStatusCodeInfo(this)}
        Response: *empty*
        """;
    
    // TODO, TEST: What if response is a stream or text 
    // TODO, TEST: Check the response type header?
}
