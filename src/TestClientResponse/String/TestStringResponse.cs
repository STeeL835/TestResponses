using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace TestClientResponse.String;

public record TestStringResponse(HttpResponseMessage HttpResponse) : TestResponse(HttpResponse)
{
    private string? _asString;
    
    public string? AsString => GetReadValue(_asString);
    
    public override async Task Read() // TODO: maybe find a way to universalize this
    {
        if (IsRead) return; // TODO: Test it's not read
        await ReadString();
        IsRead = true; 
    }

    protected async Task<string> ReadString()
    {
        return _asString = await HttpResponse.Content.ReadAsStringAsync(); 
        // TODO: do not set _asString here
        // TODO: cancellationTokens?
    }

    [DoesNotReturn]
    protected override void ThrowAssertionException(string message)
    {
        throw new TestStringResponseAssertionException(this, message);
    }

    // TODO, TEST: Check the response type header?
    // TODO, TEST: What if response is a stream 
    // TODO, TEST: Response as string is always an empty string, is it? return null or empty string?
}
