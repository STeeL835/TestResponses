using System.Text.Json;
using System.Text.Json.Nodes;

namespace TestResponses.Json;

/// <summary>
/// System.Text.Json-based serializer used by <see cref="TestJsonResponse" />.
/// </summary>
public class TestJsonResponseSystemTextSerializer(JsonSerializerOptions options) : ITestJsonResponseSerializer
{
    /// <summary>
    /// Serializer options used by System.Text.Json.
    /// </summary>
    public JsonSerializerOptions Options { get; } = options;

    /// <summary>
    /// Deserializes JSON into the specified target type.
    /// </summary>
    /// <typeparam name="T">Target model type.</typeparam>
    /// <param name="json">JSON payload.</param>
    public T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
    
    /// <summary>
    /// Tries to indent JSON text for formatting.
    /// </summary>
    /// <param name="possibleJson">Input string to inspect.</param>
    /// <param name="possiblyIndentedJson">Indented JSON output when successful.</param>
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