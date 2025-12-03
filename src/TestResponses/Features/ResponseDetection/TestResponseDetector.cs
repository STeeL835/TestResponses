using TestResponses.Unknown;

namespace TestResponses.Features;

internal static class TestResponseDetector
{
    public static async Task<TestResponse> ReadAsBestFitResponse(HttpResponseMessage httpResponse)
    {
        foreach (var testResponseType in TestResponseTypes.Global.List)
        {
            var testResponse = (TestResponse)Activator.CreateInstance(
                testResponseType.IsGenericTypeDefinition ? testResponseType.MakeGenericType(typeof(object)) : testResponseType,
                httpResponse)!;
                
            if (testResponse.CanHandleContentType())
            {
                await testResponse.Read();
                return testResponse;
            }
        }

        var unknownResponse = new TestUnknownResponse(httpResponse);
        await unknownResponse.Read();
        return unknownResponse;
    }
}