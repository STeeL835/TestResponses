namespace TestResponses.Streams;

public class TestStreamResponse(HttpResponseMessage httpResponse) : TestResponse(httpResponse)
{
    private ResponseValue<Stream>? _stream;

    public Stream AsStream => _stream.GetOrThrow()!;

    protected override async Task ReadResponse()
    {
        _stream = await ResponseValue.Create(this, HttpResponse.Content.ReadAsStreamAsync);
    }

    internal override bool CanHandleContent() => ContentType is not null && HttpResponse.Content is StreamContent;

    protected override string GetInfoString() => TestStreamResponseFormatter.Format(this);
}