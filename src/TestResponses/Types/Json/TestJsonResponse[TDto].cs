namespace TestResponses.Json;

public class TestJsonResponse<TDto>(HttpResponseMessage httpResponse) : TestJsonResponse(httpResponse)
{
    private ResponseValue<TDto>? _json;

    public TDto? AsDto => _json.GetOrThrow();

    protected override async Task ReadResponse()
    {
        await base.ReadResponse();
        _json = ResponseValue.Create(this, As<TDto>)!;
    }

    protected override void AssertResponseSchema() { _ = AsDto; } // if not read correctly, will throw
}