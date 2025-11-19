namespace TestClientResponse.Example.IntegrationTests;

[CollectionDefinition(nameof(AppContextCollection))]
public class AppContextCollection : ICollectionFixture<TestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}