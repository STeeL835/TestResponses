using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace TestClientResponse.String;

public record TestJsonResponse<TDto>(HttpResponseMessage HttpResponse) : TestStringResponse(HttpResponse)
{
    private TDto? _asDto;
    
    public TDto? AsDto => GetReadValue(_asDto);

    public override async Task Read()
    {
        if (IsRead) return; // TODO: Test it's not read
        var stringResponse = await ReadString();
        ReadDto(stringResponse);
        IsRead = true; 
    }

    protected TDto ReadDto(string stringResponse)
    {
        return _asDto = JsonSerializer.Deserialize<TDto>(stringResponse); // TODO: test is read
        
        // TODO: test is deserialized
        // TODO: test does not throw on read
        // TODO: test does throw on property access
        // TODO: test does throw on ShouldSucceed
        // TODO: test exception shows json
        // TODO: test exception shows string if json is incorrect
        //TODO: what if can't deserialize
        //TODO: exception can show string even though reading wasn't done completely
    }
    
    [DoesNotReturn]
    protected override void ThrowAssertionException(string message)
    {
        // TODO: Json exception
    }
}