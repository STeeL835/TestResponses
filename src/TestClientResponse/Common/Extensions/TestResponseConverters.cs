namespace TestClientResponse;

public static class TestResponseConverters
{
    public static async Task<TTestResponse> ReadAs<TTestResponse>(this Task<HttpResponseMessage> responseTask) where TTestResponse : TestResponse
    {
        return await ReadAs<TTestResponse>(await responseTask);
    }
    
    public static async Task<TTestResponse> ReadAs<TTestResponse>(this HttpResponseMessage httpResponse) where TTestResponse : TestResponse
    {
        // TODO: source generation for instantiation
        try
        {
            var testResponse = Activator.CreateInstance(typeof(TTestResponse), httpResponse) as TTestResponse;
            await testResponse!.Read();
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