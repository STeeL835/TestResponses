namespace TestClientResponse;

public abstract class TestResponseAssertionException<TResponse> : TestResponseException
    where TResponse : TestResponse
{
    protected readonly TResponse Response;

    public TestResponseAssertionException(TResponse response, string message) : base(message)
    {
        Response = response;
    }

    public TestResponseAssertionException(TResponse response, string message, Exception inner) : base(message, inner)
    {
        Response = response;
    }

    public override string Message => $"{BuildAssertMessage(base.Message)}";

    protected virtual string BuildAssertMessage(string message)
    {
        var result = $"""
            {message}
            Status code: {(int)Response.StatusCode} ({Response.HttpResponse.ReasonPhrase})
            """;
        
        return result;
    }
}