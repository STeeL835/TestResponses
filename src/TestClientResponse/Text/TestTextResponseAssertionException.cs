namespace TestClientResponse.Text;

public class TestTextResponseAssertionException(TestTextResponse response, string message, Exception? inner = null)
    : TestResponseAssertionException<TestTextResponse>(response, message, inner)
{
    private string? _assertMessage;

    protected override string BuildAssertMessage(string message)
    {
        _assertMessage ??= $"""
            {base.BuildAssertMessage(message)}
            Response: 
            {(Response.IsRead ? Response.AsText : "*not read*")}
            """;

        return _assertMessage;
    }
}