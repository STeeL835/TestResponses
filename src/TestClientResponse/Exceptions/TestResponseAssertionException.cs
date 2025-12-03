namespace TestClientResponse;

public class TestResponseAssertionException(string message, Exception? inner = null) : TestResponseException(message, inner);