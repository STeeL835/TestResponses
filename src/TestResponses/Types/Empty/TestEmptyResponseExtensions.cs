using TestResponses.Features;

namespace TestResponses.Empty;

public static class TestEmptyResponseExtensions 
{
    public static async Task ShouldSucceed<T>(this Task<TestEmptyResponse> responseTask, UniStatusCode? withStatusCode = null)
    {
        var testResponse = await responseTask;
        await testResponse.Read();
        
        testResponse.AssertValid(withStatusCode);
    }
}