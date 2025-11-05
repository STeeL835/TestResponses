using TestClientResponse.String;

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

public class TestStringResponseAssertionException : TestResponseAssertionException<TestStringResponse>
{
    public TestStringResponseAssertionException(TestStringResponse response, string message) : base(response, message) { }

    public TestStringResponseAssertionException(TestStringResponse response, string message, Exception inner) : base(response, message, inner) { }

    protected override string BuildAssertMessage(string message)
    {
        var result = $"""
            {base.BuildAssertMessage(message)}
            Response: 
            {(Response.IsRead ? Response.AsString : "*not read*")}
            """;
        // TODO, TEST: read, not read 

        return result;
    }
}