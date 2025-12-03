using RichardSzalay.MockHttp;

namespace TestResponses.Tests.Utilities;

public static class TestHttpClient
{
    public static async Task<HttpResponseMessage> ReceiveResponse(Action<MockedRequest> setupResponse)
    {
        var mockHttp = new MockHttpMessageHandler();

        setupResponse(mockHttp.When("https://localhost/api/test"));
        
        var client = mockHttp.ToHttpClient();
        
        return await client.GetAsync("https://localhost/api/test");
    }
}