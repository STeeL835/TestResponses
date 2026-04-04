namespace TestResponses.Json;

/// <summary>
/// Strongly typed JSON response that deserializes JSON into a DTO of type <typeparamref name="TDto" />.
/// Provides the strongly typed <see cref="AsDto" /> property and schema validation via deserialization.
/// </summary>
public class TestJsonResponse<TDto>(HttpResponseMessage httpResponse) : TestJsonResponse(httpResponse)
{
    private ResponseValue<TDto>? _json;

    /// <summary>
    /// Strongly typed DTO deserialized from JSON response content.
    /// </summary>
    public TDto? AsDto => _json.GetOrThrow();

    protected override async Task ReadResponse()
    {
        await base.ReadResponse();
        _json = ResponseValue.Create(this, As<TDto>)!;
    }

    protected override void AssertResponseSchema() { _ = AsDto; } // if not read correctly, will throw
}