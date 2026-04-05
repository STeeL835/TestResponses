namespace TestResponses.Json;

/// <summary>
/// Contract for JSON serializers used by <see cref="TestJsonResponse" />.
/// </summary>
public interface ITestJsonResponseSerializer
{
    /// <summary>
    /// Deserializes json into a model. Result of deserialization and possible exceptions depend on serializer implementation
    /// </summary>
    public T? Deserialize<T>(string json);
    
    /// <summary>
    /// Tries to indent an input if it's JSON. If it's not, input value is unchanged
    /// </summary>
    public bool TryIndent(string possibleJson, out string possiblyIndentedJson);
}