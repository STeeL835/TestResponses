using TestResponses.Features;

namespace TestResponses.Empty;

public static class TestEmptyResponseExtensions 
{
    public static async Task ShouldSucceed(this Task<TestEmptyResponse> responseTask, UniStatusCode? withStatusCode = null)
    {
        var testResponse = await responseTask;
        await testResponse.Read();
        
        testResponse.AssertValid(withStatusCode);
    }
}