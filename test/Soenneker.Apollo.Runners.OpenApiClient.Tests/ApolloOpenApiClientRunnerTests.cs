using Soenneker.Tests.HostedUnit;

namespace Soenneker.Apollo.Runners.OpenApiClient.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class ApolloOpenApiClientRunnerTests : HostedUnitTest
{
    public ApolloOpenApiClientRunnerTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
