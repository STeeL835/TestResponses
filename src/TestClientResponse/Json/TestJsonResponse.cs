using System.Text.Json;
using TestClientResponse.Text;

namespace TestClientResponse.Json;

public class TestJsonResponse<TDto>(HttpResponseMessage httpResponse) : TestTextResponse(httpResponse)
{
    private ResponseValue<TDto>? _json;

    public TDto? AsDto => _json.GetOrThrow();
    
    public T? As<T>() => JsonSerializer.Deserialize<T>(AsText, TestJsonResponseOptions.SerializerOptions);
    
    internal override bool CanHandleContentType()
    {
        if (HttpResponse.Content.Headers.ContentType?.MediaType is null) return false;
        if (HttpResponse.Content.Headers.ContentType.MediaType!.Contains("json")) return true;
        return false;
    }

    protected override async Task ReadResponse()
    {
        await ReadText();
        
        _json = await ResponseValue.Create(this, () =>  Task.FromResult(As<TDto>()));
    }

    protected override string GetInfoString() => TestJsonResponseFormatter.Format(this);
}