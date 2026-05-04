namespace TestResponses.Empty;

/// <summary>
/// Default formatter for empty responses.
/// </summary>
public class TestEmptyResponseFormatter : ITestResponseFormatter<TestEmptyResponse>
{
    public string Format(TestEmptyResponse response)
    {
        return $"""
            {TestResponseFormatter.FormatStatusCodeInfo(response)}
            Response: *empty*
            """;
    }
}