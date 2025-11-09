using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using TestClientResponse.Text;

namespace TestClientResponse.Json;

public class TestJsonResponseAssertionException(TestTextResponse response, string message, Exception? inner = null)
    : TestResponseAssertionException<TestTextResponse>(response, message, inner)
{
    private string? _assertMessage;

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
                AllowTrailingCommas = TestJsonResponseOptions.SerializerOptions.AllowTrailingCommas,
                CommentHandling = TestJsonResponseOptions.SerializerOptions.ReadCommentHandling,
            });

            if (jObj is null) return json;

            var indentedJson = jObj.ToJsonString(TestJsonResponseOptions.SerializerOptions);
            return indentedJson;
        }
        catch (JsonException)
        {
            return json;
        }
    }
}