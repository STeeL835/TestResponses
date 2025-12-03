namespace TestResponses.Features;

public static class TestResponseReadExtensions
{
    public static async Task<TTestResponse> ReadAs<TTestResponse>(
        this Task<HttpResponseMessage> responseTask, 
        UniStatusCode? expectedStatusCode = null) 
        where TTestResponse : TestResponse
    {
        return await ReadAs<TTestResponse>(await responseTask, expectedStatusCode);
    }
    
    public static async Task<TTestResponse> ReadAs<TTestResponse>(
        this HttpResponseMessage httpResponse, 
        UniStatusCode? expectedStatusCode = null) 
        where TTestResponse : TestResponse
    {
        try
        {
            var testResponse = Activator.CreateInstance(typeof(TTestResponse), httpResponse) as TTestResponse;
            testResponse!.ExpectedStatusCode = expectedStatusCode;
            
            await testResponse.Read();
            return testResponse;
        }
        catch (MissingMethodException ex)
        {
            throw new TestResponseException($"Could not find a constructor to create a test response instance." +
                                            $" To make this method work, make sure {typeof(TTestResponse).Name}" +
                                            $" has a constructor that takes {nameof(HttpResponseMessage)} as its only parameter," +
                                            $" or make your own extension method that would instantiate a test response" +
                                            $" and call {nameof(TestResponse.Read)} method on it", ex);
        }
    }
}