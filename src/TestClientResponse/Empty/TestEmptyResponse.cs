namespace TestClientResponse.Empty;

public record TestEmptyResponse(HttpResponseMessage HttpResponse) : TestResponse(HttpResponse)
{
    internal override bool CanHandleContentType() => HttpResponse.Content.Headers.ContentType is null;

    protected override Task ReadResponse() => Task.CompletedTask;

    protected override string GetInfoString() => $"""
        {TestResponseFormatter.FormatStatusCodeInfo(this)}
        Response: *empty*
        """;
}
