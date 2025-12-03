namespace TestResponses.Empty;

public class TestEmptyResponse(HttpResponseMessage httpResponse) : TestResponse(httpResponse)
{
    internal override bool CanHandleContentType() => HttpResponse.Content.Headers.ContentType is null;

    protected override Task ReadResponse() => Task.CompletedTask;

    protected override string GetInfoString() => $"""
        {TestResponseFormatter.FormatStatusCodeInfo(this)}
        Response: *empty*
        """;
}
