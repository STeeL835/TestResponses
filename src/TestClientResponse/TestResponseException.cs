namespace TestClientResponse;

public class TestResponseException(string message, Exception? inner = null) : Exception(message, inner);