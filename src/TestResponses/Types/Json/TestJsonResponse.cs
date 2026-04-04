using TestResponses.Text;

namespace TestResponses.Json;

/// <summary>
/// Response type for JSON payloads.
/// Supports JSON deserialization on demand with <see cref="As{T}"/> and JSON-specific formatting for response info.
/// </summary>
public class TestJsonResponse(HttpResponseMessage httpResponse) : TestTextResponse(httpResponse)
{
    /// <summary>
    /// Global configuration applied to all <see cref="TestJsonResponse"/> instances by default.
    /// </summary>
    public static TestJsonResponseConfiguration GlobalJsonConfig = TestJsonResponseConfiguration.Default;

    /// <summary>
    /// Instance-level configuration <see cref="TestJsonResponse"/>
    /// </summary>
    public TestJsonResponseConfiguration JsonConfig { get; init; } = GlobalJsonConfig;

    /// <summary>
    /// Deserializes the JSON response content into the requested type.
    /// </summary>
    /// <typeparam name="T">The target model type.</typeparam>
    /// <returns>The deserialized object.</returns>
    public T? As<T>() => JsonConfig.Serializer.Deserialize<T>(AsText);
    
    internal override bool CanHandleContent() => ContentType is not null && ContentType.Contains("json");
    
    protected override string GetInfoString() => TestJsonResponseFormatter.Format(this);
}