using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TestClientResponse.Text;

namespace TestClientResponse.Json;

public record TestJsonResponse<TDto>(HttpResponseMessage HttpResponse) : TestTextResponse(HttpResponse)
{
    private ValueReadResult<TDto>? _dtoReadResult;

    public bool IsDtoReadSuccessfully => _dtoReadResult?.IsReadSuccessfully ?? IsRead;
    public TDto? AsDto => GetReadValue(_dtoReadResult!);

    protected override async Task ReadResponse()
    {
        var responseText = await ReadText();
        _dtoReadResult = DeserializeDtoWithDelayedException(responseText);
    }

    private static ValueReadResult<TDto> DeserializeDtoWithDelayedException(string stringResponse)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<TDto>(stringResponse, TestJsonResponseOptions.SerializerOptions);
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