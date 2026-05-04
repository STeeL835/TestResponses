using Microsoft.AspNetCore.Mvc.Testing;

namespace TestResponses.Example.IntegrationTests;

public class TestFixture : IAsyncLifetime
{
    internal WebApplicationFactory<Program> AppFactory { get; set; } = null!;

    public HttpClient HttpClient { get; set; } = null!;

    public IServiceProvider AppServices { get; set; } = null!;
    
    
    public Task InitializeAsync()
    {
        AppFactory = new WebApplicationFactory<Program>();
        HttpClient = AppFactory.CreateClient();
        AppServices =  AppFactory.Services;
        
        return Task.CompletedTask;
    }

    public Task DisposeAsync() => Task.CompletedTask;
}