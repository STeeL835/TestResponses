using System.Text.Json;
using TestResponses.Features;
using TestResponses.Text;

namespace TestResponses.Json;

public class TestJsonResponse<TDto>(HttpResponseMessage httpResponse) : TestTextResponse(httpResponse)
{
    private ResponseValue<TDto>? _json;

    public TDto? AsDto => _json.GetOrThrow();
    
    public T? As<T>() => JsonSerializer.Deserialize<T>(AsText, TestJsonResponseOptions.SerializerOptions);

    protected override async Task ReadResponse()
    {
        await ReadText();
        
        _json = await ResponseValue.Create(this, () =>  Task.FromResult(As<TDto>()));
    }

    internal override bool CanHandleContentType()
    {
        if (HttpResponse.Content.Headers.ContentType?.MediaType is null) return false;
        if (HttpResponse.Content.Headers.ContentType.MediaType!.Contains("json")) return true;
        return false;
    }
    
    protected override void AssertResponseSchema() { _ = AsDto; } // if not read correctly, will throw

    protected override string GetInfoString() => TestJsonResponseFormatter.Format(this);
}