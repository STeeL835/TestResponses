namespace TestClientResponse;

/// <summary> A non-generic abstract class for convenience of catching </summary>
public abstract class TestResponseAssertionException(string message, Exception? inner = null) : TestResponseException(message, inner);