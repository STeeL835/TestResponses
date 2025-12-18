namespace TestResponses.Json;

public static class TestJsonResponseFormatter
{
    public static string Format(TestJsonResponse response)
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