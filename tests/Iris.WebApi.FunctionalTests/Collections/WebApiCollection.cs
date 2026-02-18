using Iris.WebApi.FunctionalTests.Fixtures;

namespace Iris.WebApi.FunctionalTests.Collections;

public static class WebApiCollection
{
    public const string Name = "web-api-collection";
}

[CollectionDefinition(WebApiCollection.Name)]
public sealed class WebApiCollectionDefinition
    : ICollectionFixture<WebApiTestFixture<Program>>
{
}