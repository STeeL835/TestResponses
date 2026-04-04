namespace TestResponses.Empty;

/// <summary>
/// Represents an empty response, such as HTTP 204 No Content.
/// This type expects the response to contain no body and will fail if content is present.
/// </summary>
public class TestEmptyResponse(HttpResponseMessage httpResponse) : TestResponse(httpResponse)
{
    protected override Task ReadResponse() => Task.CompletedTask;

    internal override bool CanHandleContent() => ContentType is null;

    protected override string GetInfoString() => $"""
        {TestResponseFormatter.FormatStatusCodeInfo(this)}
        Response: *empty*
        """;
}