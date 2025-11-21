namespace TestClientResponse.Json;

public static class TestJsonResponseExtensions 
{
    public static async Task<T?> ShouldSucceed<T>(this Task<TestJsonResponse<T>> responseTask)
    {
        var testResponse = await responseTask;
        await testResponse.Read();
        
        testResponse.AssertStatusCode(200..299);

        return testResponse.AsDto;
    }
}