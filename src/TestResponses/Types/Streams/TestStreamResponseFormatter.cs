namespace TestResponses.Streams;

public static class TestStreamResponseFormatter
{
    public static string Format(TestStreamResponse response)
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