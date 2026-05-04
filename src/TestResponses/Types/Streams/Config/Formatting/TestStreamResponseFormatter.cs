namespace TestResponses.Streams;

/// <summary>
/// Default formatter for stream responses.
/// </summary>
public class TestStreamResponseFormatter : ITestResponseFormatter<TestStreamResponse>
{
    public string Format(TestStreamResponse response)
    {
        return $"""
            {TestResponseFormatter.FormatStatusCodeInfo(response)}
            {FormatResponseAsStream(response)}
            """;
    }

    public static string FormatResponseAsStream(TestStreamResponse response)
    {
        return $"""
            Response: {response switch
            {
                { IsRead: false } => "*not read*",
                _ => $"{response.ContentType} stream {response.AsStream.Length} bytes long"
            }}
            """;
    }
}