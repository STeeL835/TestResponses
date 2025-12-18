using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestResponses.Json;

public class TestJsonResponseConfiguration
{
    public static TestJsonResponseConfiguration Default => new()
    {
        Serializer = new TestJsonResponseSystemTextSerializer(new (JsonSerializerDefaults.Web)
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
        })
    };

    public required ITestJsonResponseSerializer Serializer { get; init; }
}