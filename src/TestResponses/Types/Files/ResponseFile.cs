namespace TestResponses.Files;

public class ResponseFile(Stream stream, string name)
{
    public Stream Stream { get; } = stream;
    public string Name { get; } = name;
    public long SizeBytes => Stream.Length;

    public override string ToString() => $"{Name}, {TestFileResponseFormatter.FormatFileSize(SizeBytes)}";
}