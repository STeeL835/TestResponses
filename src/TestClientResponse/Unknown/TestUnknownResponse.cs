namespace TestClientResponse.Unknown;

public record TestUnknownResponse(HttpResponseMessage HttpResponse) : TestResponse(HttpResponse)
{
    internal override bool CanHandleContentType() => true;

    protected override Task ReadResponse() => Task.CompletedTask;

    protected override string GetInfoString() => $"""
        {TestResponseFormatter.FormatStatusCodeInfo(this)}
        Content-Type: {HttpResponse.Content.Headers.ContentType}
        Response can't be shown because of unknown content-type
        """;
}