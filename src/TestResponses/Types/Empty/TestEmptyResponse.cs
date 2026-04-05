namespace TestResponses.Empty;

/// <summary>
/// Represents an empty response, such as HTTP 204 No Content.
/// This type expects the response to contain no body and will fail if content is present.
/// </summary>
public class TestEmptyResponse(HttpResponseMessage httpResponse) : TestResponse(httpResponse)
{
    /// <summary>
    /// Global configuration applied to all <see cref="TestEmptyResponse"/> instances by default.
    /// </summary>
    public static TestEmptyResponseConfiguration GlobalEmptyConfig = TestEmptyResponseConfiguration.Default;

    /// <summary>
    /// Instance-level configuration for <see cref="TestEmptyResponse"/>
    /// </summary>
    public TestEmptyResponseConfiguration EmptyConfig { get; init; } = GlobalEmptyConfig;

    
    protected override Task ReadResponse() => Task.CompletedTask;

    internal override bool CanHandleContent() => ContentType is null;

    protected override string GetInfoString() => EmptyConfig.Formatter.Format(this);
}