using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestResponses.Json;

public class TestJsonResponseConfiguration
{
    public static TestJsonResponseConfiguration Default => new()
    {
        SerializerOptions = new (JsonSerializerDefaults.Web)
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
            RespectNullableAnnotations = true,
            RespectRequiredConstructorParameters = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // non-latin characters without encoding
            WriteIndented = true,
        }
    };

    public required JsonSerializerOptions SerializerOptions { get; init; }
}