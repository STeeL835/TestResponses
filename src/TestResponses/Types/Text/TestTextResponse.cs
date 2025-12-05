using TestResponses.Features;

namespace TestResponses.Text;

public class TestTextResponse(HttpResponseMessage httpResponse) : TestResponse(httpResponse)
{
    private ResponseValue<string>? _text;

    /// <summary>
    /// Response as a text, exactly as received.
    /// If response is actually a text, string can't be null (for example 204 No Content reads an empty string)
    /// If it's null, response is not text (may be a stream)
    /// </summary>
    public string AsText => _text.GetOrThrow()!;

    protected override async Task ReadResponse()
    {
        _text = await ResponseValue.Create(this, HttpResponse.Content.ReadAsStringAsync);
    }

    internal override bool CanHandleContentType()
    {
        return ContentType is not null && 
               (ContentType.StartsWith("text/") || ContentType.Contains("json") || ContentType.Contains("xml"));
    }

    protected override string GetInfoString() => TestTextResponseFormatter.Format(this);
}
