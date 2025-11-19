using Microsoft.AspNetCore.Mvc.Testing;

namespace TestClientResponse.Example.IntegrationTests;

public class TestFixture : IAsyncLifetime
{
    internal WebApplicationFactory<Program> Factory { get; set; }
    public HttpClient Client { get; set; }
    
    public Task InitializeAsync()
    {
        Factory = new WebApplicationFactory<Program>();
        Client = Factory.CreateClient();
        
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;
}