namespace TestClientResponse.String;

public static class TestStringResponseExtensions
{
    // TODO: maybe a Result T abstract property for universality.
    // But if there are more than one result type? (SuccessDto+ErrorDto, FileStream+FileName)
    // - fail cases are to check anyway, but for files.. file class?..
    // also, if inheritance is base -> string -> dto, how should dto override <string> in inheritance?
    // - an interface! we can have children implement smth like ICanSucceed<TResult>
    //   - but now dto class is ICanSucceed x2.. method either will require a type parameter or a custom extension anyway
    //     - can do an experiment later
    public static async Task<string?> ShouldSucceed(this Task<TestStringResponse> responseTask)
    {
        var testResponse = await responseTask;
        await testResponse.Read();
        
        testResponse.ShouldHaveStatusCode(200..299);

        return testResponse.AsString;
    }
    // TODO: Maybe TestResponse can hold expected status code so client can set it and ShouldSucceed can use it
    // TODO: ShouldSucceed(withStatus) x3
}