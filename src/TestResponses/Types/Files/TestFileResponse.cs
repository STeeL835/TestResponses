using TestResponses.Streams;

namespace TestResponses.Files;

/// <summary>
/// Response type for file downloads with attachment content disposition.
/// Exposes the downloaded file through convenient <see cref="AsFile" />.
/// </summary>
public class TestFileResponse(HttpResponseMessage httpResponse) : TestStreamResponse(httpResponse)
{
    private ResponseValue<ResponseFile>? _file;
    
    /// <summary>
    /// The downloaded file payload for attachment responses.
    /// </summary>
    public ResponseFile AsFile => _file.GetOrThrow()!;
    
    /// <summary>
    /// The file name from the response content disposition, if available.
    /// </summary>
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