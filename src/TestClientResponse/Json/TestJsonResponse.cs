using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TestClientResponse.String;

namespace TestClientResponse.Json;

public record TestJsonResponse<TDto>(HttpResponseMessage HttpResponse) : TestStringResponse(HttpResponse)
{
    private ValueReadResult<TDto>? _dtoReadResult;

    public bool IsDtoReadSuccessfully => _dtoReadResult?.IsReadSuccessfully ?? IsRead;
    public TDto? AsDto => GetReadValue(_dtoReadResult!);

    public override async Task Read()
    {
        if (IsRead) return; // TODO: Test it's not read again
        var stringResponse = await ReadString();
        _dtoReadResult = DeserializeDtoWithDelayedException(stringResponse);
        IsRead = true; 
    }

    private static ValueReadResult<TDto> DeserializeDtoWithDelayedException(string stringResponse)
    {
        try
        {
            // TODO: option to use different (-ly configured) serializer  
            var dto = JsonSerializer.Deserialize<TDto>(stringResponse, new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
            });
            return new ValueReadResult<TDto>(dto, null);
        }
        catch (JsonException ex)
        {
            return new ValueReadResult<TDto>(default, ex);
        }
    }

    [DoesNotReturn]
    protected override void ThrowAssertionException(string message, Exception? innerException = null)
    {
        throw new TestJsonResponseAssertionException(this, message, innerException);
    }
}