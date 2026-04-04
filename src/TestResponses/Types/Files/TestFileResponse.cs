using TestResponses.Streams;

namespace TestResponses.Files;

public class TestFileResponse(HttpResponseMessage httpResponse) : TestStreamResponse(httpResponse)
{
    private ResponseValue<ResponseFile>? _file;
    
    public ResponseFile AsFile => _file.GetOrThrow()!;
    
    public string? FileName { get; } =
        httpResponse.Content.Headers.ContentDisposition?.FileNameStar
           ?? httpResponse.Content.Headers.ContentDisposition?.FileName;

    protected override async Task ReadResponse()
    {
        await base.ReadResponse();

        _file = ResponseValue.Create(this, () => new ResponseFile(AsStream, FileName));
    }
    
    internal override bool CanHandleContent() => ContentType is not null 
                                                 && HttpResponse.Content.Headers.ContentDisposition?.DispositionType is "attachment";

    protected override string GetInfoString() => TestFileResponseFormatter.Format(this);
}