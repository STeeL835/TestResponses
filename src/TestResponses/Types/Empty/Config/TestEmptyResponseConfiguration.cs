namespace TestResponses.Empty;

/// <summary>
/// Configuration for <see cref="TestEmptyResponse"/> instances.
/// </summary>
public class TestEmptyResponseConfiguration
{
    /// <summary>
    /// Default configuration instance. Can be used to reset configuration.
    /// </summary>
    public static TestEmptyResponseConfiguration Default { get; } = new();

    /// <summary>
    /// Formatter for <see cref="TestEmptyResponse"/> instances, defines response information used in exceptions.
    /// </summary>
    public ITestResponseFormatter<TestEmptyResponse> Formatter { get; init; } = new TestEmptyResponseFormatter();
}