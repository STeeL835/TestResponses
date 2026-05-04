namespace TestResponses.Streams;

/// <summary>
/// Configuration for <see cref="TestStreamResponse"/> instances.
/// </summary>
public class TestStreamResponseConfiguration
{
    /// <summary>
    /// Default configuration instance. Can be used to reset configuration.
    /// </summary>
    public static TestStreamResponseConfiguration Default { get; } = new();

    /// <summary>
    /// Formatter for <see cref="TestStreamResponse"/> instances, defines response information used in exceptions.
    /// </summary>
    public ITestResponseFormatter<TestStreamResponse> Formatter { get; init; } = new TestStreamResponseFormatter();
}