using System.Text.Json;
using System.Text.Json.Nodes;

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
                _ => TryFormatAsJson(response.AsText, response.JsonConfig.SerializerOptions)
            }}
            """;
    }

    public static string TryFormatAsJson(string json, JsonSerializerOptions? serializerOptions = null)
    {
        serializerOptions ??= TestJsonResponse.GlobalJsonConfig.SerializerOptions;
        try
        {
            var jObj = JsonNode.Parse(json, documentOptions: new JsonDocumentOptions()
            {
                AllowTrailingCommas = serializerOptions.AllowTrailingCommas,
                CommentHandling = serializerOptions.ReadCommentHandling,
            });

            if (jObj is null) return json;

            var indentedJson = jObj.ToJsonString(serializerOptions);
            return indentedJson;
        }
        catch (JsonException)
        {
            return json;
        }
    }
}