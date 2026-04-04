namespace TestResponses.Unknown;

/// <summary>
/// Fallback response type for unknown or unsupported content types.
/// Always reports the response as unknown and does not attempt a typed read.
/// </summary>
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