namespace TestClientResponse.String;

public class TestStringResponseAssertionException(TestStringResponse response, string message, Exception? inner = null)
    : TestResponseAssertionException<TestStringResponse>(response, message, inner)
{
    private string? _assertMessage;

    protected override string BuildAssertMessage(string message)
    {
        _assertMessage ??= $"""
            {base.BuildAssertMessage(message)}
            Response: 
            {(Response.IsRead ? Response.AsString : "*not read*")}
            """;

        return _assertMessage;
    }
}