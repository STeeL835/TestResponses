using System.Text.Json;
using System.Text.Json.Nodes;

namespace TestClientResponse.Json;

public static class TestJsonResponseFormatter
{
    public static string Format<T>(TestJsonResponse<T> response)
    {
        return $"""
            {TestResponseFormatter.FormatStatusCodeInfo(response)}
            {FormatResponseAsJson(response)}
            """;
    }

    public static string FormatResponseAsJson<T>(TestJsonResponse<T> response)
    {
        return $"""
            Response:
            {response switch
            {
                { IsRead: false } => "*not read*",
                { AsText.Length: 0 } => "*empty*",
                _ => TryFormatAsJson(response.AsText)
            }}
            """;
    }
    
    private static string TryFormatAsJson(string json)
    {
        try
        {
            var jObj = JsonNode.Parse(json, documentOptions: new JsonDocumentOptions()
            {
                AllowTrailingCommas = TestJsonResponseOptions.SerializerOptions.AllowTrailingCommas,
                CommentHandling = TestJsonResponseOptions.SerializerOptions.ReadCommentHandling,
            });

            if (jObj is null) return json;

            var indentedJson = jObj.ToJsonString(TestJsonResponseOptions.SerializerOptions);
            return indentedJson;
        }
        catch (JsonException)
        {
            return json;
        }
    }
}