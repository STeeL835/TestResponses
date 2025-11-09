using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using TestClientResponse.Text;

namespace TestClientResponse.Json;

public class TestJsonResponseAssertionException : TestResponseAssertionException<TestTextResponse>
{
    private string? _assertMessage;

    public TestJsonResponseAssertionException(TestTextResponse response, string message, Exception? inner = null) : base(response, message, inner) { }

    protected override string BuildAssertMessage(string message)
    {
        _assertMessage ??= $"""
            {base.BuildAssertMessage(message)}
            Response:
            {(Response.IsRead ? TryFormatAsJson(Response.AsText) : "*not read*")}
            """;

        return _assertMessage;
    }

    protected string TryFormatAsJson(string json)
    {
        try
        {
            var jObj = JsonNode.Parse(json, documentOptions: new JsonDocumentOptions()
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
            });

            if (jObj is null) return json;

            var indentedJson = jObj.ToJsonString(new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // non-latin characters without encoding
                WriteIndented = true,
            });
            return indentedJson;
        }
        catch (JsonException)
        {
            return json;
        }
    }
}