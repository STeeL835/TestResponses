namespace TestResponses.Json;

public static class TestJsonResponseExtensions 
{
    public static async Task<T?> AssertSucceeded<T>(this Task<TestJsonResponse<T>> responseTask, UniStatusCode? withStatusCode = null) 
        => await responseTask.AssertSucceededAndReturn(r => r.AsDto, withStatusCode);
}