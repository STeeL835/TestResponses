using System.Text.Json;
using TestClientResponse.Text;

namespace TestClientResponse.Json;

public record TestJsonResponse<TDto>(HttpResponseMessage HttpResponse) : TestTextResponse(HttpResponse)
{
    private ValueReadResult<TDto>? _dtoReadResult;

    public bool IsDtoReadSuccessfully => _dtoReadResult?.IsReadSuccessfully ?? IsRead;
    public TDto? AsDto => GetReadValue(_dtoReadResult!);
    
    public T? As<T>() => JsonSerializer.Deserialize<T>(AsText, TestJsonResponseOptions.SerializerOptions);
    
    internal override bool CanHandleContentType()
    {
        if (HttpResponse.Content.Headers.ContentType?.MediaType is null) return false;
        if (HttpResponse.Content.Headers.ContentType.MediaType!.Contains("json")) return true;
        return false;
    }

    protected override async Task ReadResponse()
    {
        var responseText = await ReadText();
        _dtoReadResult = DeserializeDtoWithDelayedException(responseText);
    }

    private static ValueReadResult<TDto> DeserializeDtoWithDelayedException(string textResponse)
    {
        try
        {
            var dto = JsonSerializer.Deserialize<TDto>(textResponse, TestJsonResponseOptions.SerializerOptions);
            return new ValueReadResult<TDto>(dto, null);
        }
        catch (JsonException ex)
        {
            return new ValueReadResult<TDto>(default, ex);
        }
    }


    public override string ToString() => TestJsonResponseFormatter.Format(this);
}