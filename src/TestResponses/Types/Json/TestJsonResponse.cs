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
        await base.ReadResponse();
        _json = ResponseValue.Create(this, As<TDto>)!;
    }

    internal override bool CanHandleContent() => ContentType is not null && ContentType.Contains("json");

    protected override void AssertResponseSchema() { _ = AsDto; } // if not read correctly, will throw

    protected override string GetInfoString() => TestJsonResponseFormatter.Format(this);
}