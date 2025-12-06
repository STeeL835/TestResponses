namespace TestResponses.Unknown;

// TODO: there are stream and empty response types, maybe this no longer needed
public class TestUnknownResponse(HttpResponseMessage httpResponse) : TestResponse(httpResponse)
{
    internal override bool CanHandleContent() => true;

    protected override Task ReadResponse() => Task.CompletedTask;

    protected override string GetInfoString() => $"""
        {TestResponseFormatter.FormatStatusCodeInfo(this)}
        Content-Type: {HttpResponse.Content.Headers.ContentType}
        Response can't be shown because of unknown content-type
        """;
}