namespace TestResponses.Text;

/// <summary>
/// Configuration for <see cref="TestTextResponse"/> instances.
/// </summary>
public class TestTextResponseConfiguration
{
    /// <summary>
    /// Default configuration instance. Can be used to reset configuration.
    /// </summary>
    public static TestTextResponseConfiguration Default { get; } = new();

    /// <summary>
    /// Formatter for <see cref="TestTextResponse"/> instances, defines response information used in exceptions.
    /// </summary>
    public ITestResponseFormatter<TestTextResponse> Formatter { get; init; } = new TestTextResponseFormatter();
}