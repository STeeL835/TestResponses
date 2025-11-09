using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TestClientResponse.String;

namespace TestClientResponse.Json;

public record TestJsonResponse<TDto>(HttpResponseMessage HttpResponse) : TestStringResponse(HttpResponse)
{
    private DelayedValueWithException<TDto>? _delayedDto;

    public bool IsDtoReadSuccessfully => _delayedDto?.IsReadSuccessfully ?? IsRead;
    public TDto? AsDto => GetReadValue(_delayedDto!);

    public override async Task Read()
    {
        if (IsRead) return; // TODO: Test it's not read
        var stringResponse = await ReadString();
        _delayedDto = DeserializeDtoWithDelayedException(stringResponse);
        IsRead = true; 
    }

    private static DelayedValueWithException<TDto> DeserializeDtoWithDelayedException(string stringResponse)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<TDto>(stringResponse, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
            });
            return new DelayedValueWithException<TDto>(dto, null);
        }
        catch (JsonException ex)
        {
            return new DelayedValueWithException<TDto>(default, ex);
        }
    }

    [DoesNotReturn]
    protected override void ThrowAssertionException(string message, Exception? innerException = null)
    {
        throw new TestJsonResponseAssertionException(this, message, innerException);
    }
}