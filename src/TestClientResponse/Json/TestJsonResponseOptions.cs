using System.Text.Encodings.Web;
using System.Text.Json;

namespace TestClientResponse.Json;

public static class TestJsonResponseOptions
{
    public static readonly JsonSerializerOptions DefaultSerializerOptions = new (JsonSerializerDefaults.Web)
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // non-latin characters without encoding
        WriteIndented = true,
    };

    public static JsonSerializerOptions SerializerOptions = DefaultSerializerOptions;
}