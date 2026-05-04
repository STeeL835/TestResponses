using TestResponses.Streams;

namespace TestResponses.Text;

/// <summary>
/// Response type for text-based content, including plain text, XML, and JSON.
/// Exposes the response content as a string through <see cref="AsText" />.
/// </summary>
public class TestTextResponse(HttpResponseMessage httpResponse) : TestStreamResponse(httpResponse)
{
    /// <summary>
    /// Global configuration applied to all <see cref="TestTextResponse"/> instances by default.
    /// </summary>
    public static TestTextResponseConfiguration GlobalTextConfig = TestTextResponseConfiguration.Default;

    /// <summary>
    /// Instance-level configuration for <see cref="TestTextResponse"/>
    /// </summary>
    public TestTextResponseConfiguration TextConfig { get; init; } = GlobalTextConfig;

    
    private ResponseValue<string>? _text;

    /// <summary>
    /// Response as a text, exactly as received.
    /// If response is actually a text, string can't be null (for example 204 No Content reads an empty string)
    /// If it's null, response is not text (may be a stream)
    /// </summary>
    public string AsText => _text.GetOrThrow()!;

    
    protected override async Task ReadResponse()
    {
        await base.ReadResponse();
        _text = await ResponseValue.Create(this, HttpResponse.Content.ReadAsStringAsync);
    }

    internal override bool CanHandleContent()
    {
        return ContentType is not null && 
               (ContentType.StartsWith("text/") || ContentType.Contains("json") || ContentType.Contains("xml"));
    }

    protected override string GetInfoString() => TextConfig.Formatter.Format(this);
}
