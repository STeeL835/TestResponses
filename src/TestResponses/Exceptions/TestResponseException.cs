namespace TestResponses;

/// <summary>
/// Represents an error that occurred while processing a TestResponse.
/// </summary>
public class TestResponseException(string message, Exception? inner = null) : Exception(message, inner);