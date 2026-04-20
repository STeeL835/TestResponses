using System.Diagnostics.CodeAnalysis;
using TestResponses.Json;
using TestResponses.Streams;
using TestResponses.Text;

namespace TestResponses.Tests.Utilities;

class NonIdempotentReadResponse(HttpResponseMessage httpMessage) : TestStreamResponse(httpMessage)
{
    protected override async Task ReadResponse()
    {
        if (IsRead) throw new Exception("Reading twice!");
        await base.ReadResponse();
    }
}

#pragma warning disable CS9113 // Parameter is unread. (It's needed to test the instantiation)
class InvalidCtorResponse(HttpResponseMessage httpMessage, string text) : TestTextResponse(httpMessage);
#pragma warning restore CS9113 // Parameter is unread.

class JsonPatchResponse(HttpResponseMessage httpMessage) : TestJsonResponse<object>(httpMessage);

class MarkdownResponse(HttpResponseMessage httpMessage) : TestTextResponse(httpMessage);

abstract class HeaderContentResponse(HttpResponseMessage httpMessage, string headerName) : TestResponse(httpMessage)
{
    private ResponseValue<string?>? _value;
    public string Value => _value.GetOrThrow()!;

    internal override bool CanHandleContent() => HttpResponse.Headers.Contains(headerName);
    protected override async Task ReadResponse()
    {
        _value = await ResponseValue.Create(this, () => 
            Task.FromResult((string?)string.Join("\n", HttpResponse.Headers.GetValues(headerName))));
    }
}

class CookieContentResponse(HttpResponseMessage httpMessage) : HeaderContentResponse(httpMessage, "Cookie");

