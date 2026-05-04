using TestResponses.Streams;

namespace TestResponses.Files;

/// <summary>
/// Response type for file downloads with attachment content disposition.
/// Exposes the downloaded file through convenient <see cref="AsFile" />.
/// </summary>
public class TestFileResponse(HttpResponseMessage httpResponse) : TestStreamResponse(httpResponse)
{
    /// <summary>
    /// Global configuration applied to all <see cref="TestFileResponse"/> instances by default.
    /// </summary>
    public static TestFileResponseConfiguration GlobalFileConfig = TestFileResponseConfiguration.Default;

    /// <summary>
    /// Instance-level configuration for <see cref="TestFileResponse"/>
    /// </summary>
    public TestFileResponseConfiguration FileConfig { get; init; } = GlobalFileConfig;

    
    private ResponseValue<ResponseFile>? _file;
    
    /// <summary>
    /// The downloaded file payload for attachment responses.
    /// </summary>
    public ResponseFile AsFile => _file.GetOrThrow()!;


    /// <summary>
    /// The content disposition property <c>filename*</c>, if it is set.
    /// To get a file name with a fallback to regular <c>filename</c>, use <see cref="AsFile"/> object (<see cref="ResponseFile.Name"/>).
    /// </summary>
    public string? FileNameStar { get; } = httpResponse.Content.Headers.ContentDisposition?.FileNameStar;
    
    /// <summary>
    /// The content disposition property <c>filename</c>, if it is set.
    /// To get a file name with preference to advanced <c>filename*</c>, use <see cref="AsFile"/> object (<see cref="ResponseFile.Name"/>).
    /// </summary>
    public string? FileName { get; } = httpResponse.Content.Headers.ContentDisposition?.FileName;

    
    protected override async Task ReadResponse()
    {
        await base.ReadResponse();

        _file = ResponseValue.Create(this, () => new ResponseFile(AsStream, FileNameStar ?? FileName));
    }
    
    internal override bool CanHandleContent() => ContentType is not null 
                                                 && HttpResponse.Content.Headers.ContentDisposition?.DispositionType is "attachment";

    protected override string GetInfoString() => FileConfig.Formatter.Format(this);
}