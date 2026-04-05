namespace TestResponses;

/// <summary>
/// Contract for test response formatters. Defines response information used in exceptions
/// </summary>
public interface ITestResponseFormatter<TTestResponse> where TTestResponse : TestResponse
{
    /// <summary>
    /// Formats the response for display in assertion messages.
    /// </summary>
    /// <param name="response">The file response to format.</param>
    /// <returns>Formatted string representation of the response.</returns>
    string Format(TTestResponse response);
}