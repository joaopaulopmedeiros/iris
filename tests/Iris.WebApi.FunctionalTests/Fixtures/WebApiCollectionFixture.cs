namespace Iris.WebApi.FunctionalTests.Fixtures;

public static class WebApiCollectionFixture
{
    public const string Name = "web-api-collection";
}

[CollectionDefinition(WebApiCollectionFixture.Name)]
public sealed class WebApiCollectionDefinition
    : ICollectionFixture<WebApiTestFixture<Program>>
{
}