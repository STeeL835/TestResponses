namespace TestResponses;

/// <summary>
/// Represents a failed assertion on a TestResponse, including response details.
/// </summary>
public class TestResponseAssertionException(TestResponse response, string message, Exception? inner = null) 
    : TestResponseException($"{message}\n\n{response}", inner);