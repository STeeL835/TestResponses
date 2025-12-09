using TestResponses.Streams;

namespace TestResponses.Files;

// TODO: test response
public class TestFileResponse(HttpResponseMessage httpResponse) : TestStreamResponse(httpResponse)
{
    private ResponseValue<ResponseFile>? _file;
    
    public ResponseFile AsFile => _file.GetOrThrow()!;

    protected override async Task ReadResponse()
    {
        await base.ReadResponse();

        _file = ResponseValue.Create(this, () => new ResponseFile(
            AsStream,
            GetFileNameOrNull() ?? throw new TestResponseException("Response does not contain file name")));
    }
    
    internal override bool CanHandleContent() => ContentType is not null && GetFileNameOrNull() is not null;

    private string? GetFileNameOrNull()
    {
        return HttpResponse.Content.Headers.ContentDisposition?.FileNameStar
               ?? HttpResponse.Content.Headers.ContentDisposition?.FileName;
    }

    protected override string GetInfoString() => TestFileResponseFormatter.Format(this);
}