namespace TestResponses.Files;

/// <summary>
/// Configuration for <see cref="TestFileResponse"/> instances.
/// </summary>
public class TestFileResponseConfiguration
{
    /// <summary>
    /// Default configuration instance. Can be used to reset configuration.
    /// </summary>
    public static TestFileResponseConfiguration Default { get; } = new();

    /// <summary>
    /// Formatter for <see cref="TestFileResponse"/> instances, defines response information used in exceptions.
    /// </summary>
    public ITestResponseFormatter<TestFileResponse> Formatter { get; init; } = new TestFileResponseFormatter();
}