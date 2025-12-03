using TestResponses.Features;

namespace TestResponses.Text;

public static class TestTextResponseExtensions
{
    // TODO: code generation for ShouldSucceed ([MainValue(nameof(AsText)] -> generates an extension that returns that value
    // - test result shouldn't have more that one main value
    // - attribute not inherited
    // - generic interfaces won't work - they will be inherited, ambiguous for compiler
    public static async Task<string> AssertSucceeded(this Task<TestTextResponse> responseTask, UniStatusCode? withStatusCode = null)
    {
        var testResponse = await responseTask;
        await testResponse.Read();
        
        testResponse.AssertValid(withStatusCode);

        return testResponse.AsText;
    }
    
}