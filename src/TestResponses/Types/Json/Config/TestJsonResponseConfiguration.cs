using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestResponses.Json;

/// <summary>
/// Configuration for <see cref="TestJsonResponse"/> instances.
/// </summary>
public class TestJsonResponseConfiguration
{
    /// <summary>
    /// Default configuration instance. Can be used to reset configuration.
    /// </summary>
    public static TestJsonResponseConfiguration Default { get; } = new();
    
    
    /// <summary>
    /// Formatter for <see cref="TestJsonResponse"/> instances, defines response information used in exceptions.
    /// </summary>
    public ITestResponseFormatter<TestJsonResponse> Formatter { get; init; } = new TestJsonResponseFormatter();
    
    /// <summary>
    /// Serializer for <see cref="TestJsonResponse"/> instances.
    /// </summary>
    public ITestJsonResponseSerializer Serializer { get; init; } = new TestJsonResponseSystemTextSerializer(new (JsonSerializerDefaults.Web)
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
#if NET9_0_OR_GREATER
        RespectNullableAnnotations = true, // to fail if non-nullable reference type values weren't found in json
        RespectRequiredConstructorParameters = true, // to fail if non-nullable ctor parameters were not found in json
#endif
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // to show non-latin characters unencoded
        WriteIndented = true,
    });
}
