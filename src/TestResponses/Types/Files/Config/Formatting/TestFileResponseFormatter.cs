namespace TestResponses.Files;

/// <summary>
/// Default formatter for file responses.
/// </summary>
public class TestFileResponseFormatter : ITestResponseFormatter<TestFileResponse>
{
    public string Format(TestFileResponse response)
    {
        return $"""
            {TestResponseFormatter.FormatStatusCodeInfo(response)}
            {FormatResponseAsFile(response)}
            """;
    }

    public static string FormatResponseAsFile(TestFileResponse response)
    {
        return response switch
            {
                { IsRead: false } => "Response: *not read*",
                _ => $"""
                File name: {response.AsFile.Name}
                File size: {FormatFileSize(response.AsFile.SizeBytes)} ({response.AsFile.SizeBytes} bytes)
                """
            };
    }

    public static string FormatFileSize(long bytes)
    {
        string[] postfixes = ["B", "KB", "MB", "GB", "TB", "PB", "EB"];
        double size = bytes;
    
        int postfixIndex = 0;
        while (size >= 1000)
        {
            size /= 1000;
            postfixIndex++;
        }

        return $"{size:#0.##}{postfixes[postfixIndex]}";
    }
}