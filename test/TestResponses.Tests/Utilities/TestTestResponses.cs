using TestResponses.Features;
using TestResponses.Json;
using TestResponses.Text;

namespace TestResponses.Tests.Utilities;

class InvalidCtorResponse(HttpResponseMessage httpMessage, string text) : TestTextResponse(httpMessage);

class JsonPatchResponse(HttpResponseMessage httpMessage) : TestJsonResponse<object>(httpMessage);

class MarkdownResponse(HttpResponseMessage httpMessage) : TestTextResponse(httpMessage);

abstract class HeaderContentResponse(HttpResponseMessage httpMessage, string headerName) : TestResponse(httpMessage)
{
    private ResponseValue<string>? _value;
    public string Value => _value.GetOrThrow()!;

    internal override bool CanHandleContentType() => HttpResponse.Headers.Contains(headerName);
    protected override async Task ReadResponse()
    {
        _value = await ResponseValue.Create(this, () => 
            Task.FromResult((string?)string.Join("\n", HttpResponse.Headers.GetValues(headerName))));
    }
}

class CookieContentResponse(HttpResponseMessage httpMessage) : HeaderContentResponse(httpMessage, "Cookie");

