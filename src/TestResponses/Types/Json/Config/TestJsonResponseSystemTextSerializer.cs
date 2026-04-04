using System.Text.Json;
using System.Text.Json.Nodes;

namespace TestResponses.Json;

public class TestJsonResponseSystemTextSerializer(JsonSerializerOptions options) : ITestJsonResponseSerializer
{
    public JsonSerializerOptions Options { get; } = options;

    public T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
    
    public bool TryIndent(string possibleJson, out string possiblyIndentedJson)
    {
        possiblyIndentedJson = possibleJson;
        try
        {
            var jObj = JsonNode.Parse(possibleJson, documentOptions: new JsonDocumentOptions()
            {
                AllowTrailingCommas = Options.AllowTrailingCommas,
                CommentHandling = Options.ReadCommentHandling,
            });

            if (jObj is null) return false;

            possiblyIndentedJson = jObj.ToJsonString(Options);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}