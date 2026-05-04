namespace TestResponses.Text;

/// <summary>
/// Default formatter for text responses.
/// </summary>
public class TestTextResponseFormatter : ITestResponseFormatter<TestTextResponse>
{
    public string Format(TestTextResponse response)
    {
        return $"""
            {TestResponseFormatter.FormatStatusCodeInfo(response)}
            {FormatResponseAsText(response)}
            """;
    }

    public static string FormatResponseAsText(TestTextResponse response)
    {
        return $"""
            Response:
            {response switch
            {
                { IsRead: false } => "*not read*",
                { AsText.Length: 0 } => "*empty*",
                _ => response.AsText
            }}
            """;
    }
}