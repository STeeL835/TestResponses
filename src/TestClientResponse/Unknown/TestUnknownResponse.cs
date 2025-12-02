namespace TestClientResponse.Unknown;

public class TestUnknownResponse(HttpResponseMessage httpResponse) : TestResponse(httpResponse)
{
    internal override bool CanHandleContentType() => true;

    protected override Task ReadResponse() => Task.CompletedTask;

    protected override string GetInfoString() => $"""
        {TestResponseFormatter.FormatStatusCodeInfo(this)}
        Content-Type: {HttpResponse.Content.Headers.ContentType}
        Response can't be shown because of unknown content-type
        """;
}