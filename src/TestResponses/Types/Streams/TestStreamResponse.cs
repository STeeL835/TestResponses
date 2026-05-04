namespace TestResponses.Streams;

/// <summary>
/// Base type for responses that expose raw stream content.
/// Provides the <see cref="AsStream" /> property and stream-oriented response formatting.
/// </summary>
public class TestStreamResponse(HttpResponseMessage httpResponse) : TestResponse(httpResponse)
{
    /// <summary>
    /// Global configuration applied to all <see cref="TestStreamResponse"/> instances by default.
    /// </summary>
    public static TestStreamResponseConfiguration GlobalStreamConfig = TestStreamResponseConfiguration.Default;

    /// <summary>
    /// Instance-level configuration for <see cref="TestStreamResponse"/>
    /// </summary>
    public TestStreamResponseConfiguration StreamConfig { get; init; } = GlobalStreamConfig;

    
    private ResponseValue<Stream>? _stream;

    /// <summary>
    /// The response content exposed as a <see cref="Stream" />.
    /// </summary>
    public Stream AsStream => _stream.GetOrThrow()!;

    
    protected override async Task ReadResponse()
    {
        _stream = await ResponseValue.Create(this, HttpResponse.Content.ReadAsStreamAsync);
    }

    internal override bool CanHandleContent() => ContentType is not null && HttpResponse.Content is StreamContent;

    protected override string GetInfoString() => StreamConfig.Formatter.Format(this);
}