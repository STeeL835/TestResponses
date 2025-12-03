using TestResponses.Features;

namespace TestResponses.Json;

public static class TestJsonResponseExtensions 
{
    public static async Task<T?> AssertSucceeded<T>(this Task<TestJsonResponse<T>> responseTask, UniStatusCode? withStatusCode = null)
    {
        var testResponse = await responseTask;
        await testResponse.Read();
        
        testResponse.AssertValid(withStatusCode);

        return testResponse.AsDto;
    }
}