using TestResponses.Features;

namespace TestResponses.Empty;

public static class TestEmptyResponseExtensions 
{
    public static async Task AssertSucceeded(this Task<TestEmptyResponse> responseTask, UniStatusCode? withStatusCode = null)
    {
        var testResponse = await responseTask;
        await testResponse.Read();
        
        testResponse.AssertValid(withStatusCode?? 200..299);
    }
}