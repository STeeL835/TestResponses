namespace TestClientResponse.String;

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

        return result;
    }
}