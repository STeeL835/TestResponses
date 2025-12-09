namespace TestResponses;

public static class ResponseValue
{
    public static async Task<ResponseValue<T>> Create<T>(TestResponse testResponse, Func<Task<T>> reader)
    {
        var container = new ResponseValue<T>(testResponse);
        await container.Read(reader);
        return container;
    }
    
    public static ResponseValue<T> Create<T>(TestResponse testResponse, Func<T> reader)
    {
        var container = new ResponseValue<T>(testResponse);
        container.Read(() => Task.FromResult(reader())).Wait();
        return container;
    }
}