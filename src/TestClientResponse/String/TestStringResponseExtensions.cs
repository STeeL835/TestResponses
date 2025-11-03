namespace TestClientResponse.String;

public static class TestStringResponseExtensions
{
    public static async Task<TestStringResponse> AsTestStringResponse(this Task<HttpResponseMessage> responseTask)
    {
        var testResponse = new TestStringResponse(await responseTask);
        await testResponse.Read();
        return testResponse;
    }
}