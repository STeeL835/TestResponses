using TestClientResponse.Json;
using TestClientResponse.Text;

namespace TestClientResponse.Tests.Utilities;

class InvalidCtorResponse(HttpResponseMessage httpMessage, string text) : TestTextResponse(httpMessage);

class JsonPatchResponse(HttpResponseMessage httpMessage) : TestJsonResponse<object>(httpMessage);

class MarkdownResponse(HttpResponseMessage httpMessage) : TestTextResponse(httpMessage);

abstract class HeaderContentResponse(HttpResponseMessage httpMessage, string headerName) : TestResponse(httpMessage)
{
    private string _value;
    public string Value => GetReadValue(_value);

    internal override bool CanHandleContentType() => HttpResponse.Headers.Contains(headerName);
    protected override Task ReadResponse()
    {
        if (CanHandleContentType())
            _value = string.Join("\n", HttpResponse.Headers.GetValues(headerName));
        return Task.CompletedTask;
    }
}

class CookieContentResponse(HttpResponseMessage httpMessage) : HeaderContentResponse(httpMessage, "Cookie");

