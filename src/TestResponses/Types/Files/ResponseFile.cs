namespace TestResponses.Files;

/// <summary>
/// Represents a downloaded file payload, including the content stream and file name.
/// </summary>
public class ResponseFile(Stream stream, string? name)
{
    /// <summary>
    /// Stream containing downloaded file data.
    /// </summary>
    public Stream Stream { get; } = stream;

    /// <summary>
    /// The file name supplied by the server, if available.
    /// </summary>
    public string? Name { get; } = name;

    /// <summary>
    /// The file size in bytes.
    /// </summary>
    public long SizeBytes => Stream.Length;

    public override string ToString() => $"{Name}, {TestFileResponseFormatter.FormatFileSize(SizeBytes)}";
}