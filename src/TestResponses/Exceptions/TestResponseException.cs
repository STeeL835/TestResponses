namespace TestResponses;

public class TestResponseException(string message, Exception? inner = null) : Exception(message, inner);