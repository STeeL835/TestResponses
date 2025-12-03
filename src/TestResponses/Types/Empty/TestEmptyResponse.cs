namespace TestResponses.Empty;

public class TestEmptyResponse(HttpResponseMessage httpResponse) : TestResponse(httpResponse)
{
    protected override Task ReadResponse() => Task.CompletedTask;

    internal override bool CanHandleContentType() => HttpResponse.Content.Headers.ContentType is null;

    protected override string GetInfoString() => $"""
        {TestResponseFormatter.FormatStatusCodeInfo(this)}
        Response: *empty*
        """;
}