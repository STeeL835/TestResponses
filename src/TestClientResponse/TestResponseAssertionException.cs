namespace TestClientResponse;

public abstract class TestResponseAssertionException<TResponse>(TResponse response, string message, Exception? inner = null)
    : TestResponseException(message, inner)
    where TResponse : TestResponse
{
    protected readonly TResponse Response = response;

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