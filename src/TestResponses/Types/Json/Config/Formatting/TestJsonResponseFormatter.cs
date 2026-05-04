namespace TestResponses.Json;

/// <summary>
/// Default formatter for JSON responses.
/// </summary>
public class TestJsonResponseFormatter : ITestResponseFormatter<TestJsonResponse>
{
    public string Format(TestJsonResponse response)
    {
        return $"""
            {TestResponseFormatter.FormatStatusCodeInfo(response)}
            {FormatResponseAsJson(response)}
            """;
    }

    public static string FormatResponseAsJson(TestJsonResponse response)
    {
        return $"""
            Response:
            {response switch
            {
                { IsRead: false } => "*not read*",
                { AsText.Length: 0 } => "*empty*",
                _ => TryFormatAsJson(response.AsText, response.JsonConfig.Serializer)
            }}
            """;
    }

    public static string TryFormatAsJson(string json, ITestJsonResponseSerializer? serializer = null)
    {
        serializer ??= TestJsonResponse.GlobalJsonConfig.Serializer;
        
        serializer.TryIndent(json, out var indentedJson);
        
        return indentedJson;
    }
}