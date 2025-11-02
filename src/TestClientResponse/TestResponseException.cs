namespace TestClientResponse;

public class TestResponseException : Exception
{
    public TestResponseException(string message) : base(message) { }
    public TestResponseException(string message, Exception inner) : base(message, inner) { }
}