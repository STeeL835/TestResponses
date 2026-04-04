namespace TestResponses.Streams;

/// <summary>
/// Base type for responses that expose raw stream content.
/// Provides the <see cref="AsStream" /> property and stream-oriented response formatting.
/// </summary>
public class TestStreamResponse(HttpResponseMessage httpResponse) : TestResponse(httpResponse)
{
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

    protected override string GetInfoString() => TestStreamResponseFormatter.Format(this);
}