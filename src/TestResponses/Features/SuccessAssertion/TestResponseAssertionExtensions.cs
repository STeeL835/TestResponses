using TestResponses.Empty;
using TestResponses.Streams;

namespace TestResponses.Features;

public static class TestResponseSuccessAssertionExtensions
{
    // TODO: code generation ([MainValue(nameof(AsText))] -> generates an extension that returns that value
    // - test result shouldn't have more that one main value
    // - attribute not inherited
    // - generic interfaces won't work - they will be inherited, ambiguous for compiler
    // TODO: move tests from string to own file
    // TODO: check in tests no expected, expected and overriden with argument 
    public static async Task<TResponseValue> AssertSucceededAndReturn<TTestResponse, TResponseValue>(
        this Task<TTestResponse> responseTask,
        Func<TTestResponse, TResponseValue> valueSelector, 
        UniStatusCode? withStatusCode = null)
        where TTestResponse : TestStreamResponse // Empty response has nothing to return, stream-based do
    {
        var testResponse = await responseTask;
        await testResponse.Read();
        
        testResponse.AssertValid(withStatusCode);

        return valueSelector.Invoke(testResponse);
    }
}