namespace TestClientResponse;

public static class TestResponseFormatter
{
    public static string Format(TestResponse response)
    {
        var result = FormatStatusCodeInfo(response);
        
        return result;
    }
    public static string FormatStatusCodeInfo(TestResponse response) =>
        $"Status code: {(int)response.StatusCode} ({response.HttpResponse.ReasonPhrase})";

}