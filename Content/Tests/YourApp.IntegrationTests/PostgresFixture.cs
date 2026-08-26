using Testcontainers.PostgreSql;
using Xunit;

namespace YourApp.IntegrationTests;

/// <summary>
/// Spins up a real PostgreSQL 17 container once per test class via xUnit ICollectionFixture.
/// Tests that depend on this should be marked [Trait("Category", "Docker")].
/// If Docker is not available, set YOURAPP_TESTS_REQUIRE_DOCKER=0 (default) to skip
/// gracefully. Set to 1 to fail-fast in CI.
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public string ConnectionString { get; private set; } = string.Empty;
    public bool DockerAvailable { get; private set; }

    public async Task InitializeAsync()
    {
        try
        {
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:17-alpine")
                .WithDatabase("yourapp_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .WithCleanUp(true)
                .Build();
            await _container.StartAsync();
            ConnectionString = _container.GetConnectionString();
            DockerAvailable = true;
        }
        catch (Exception ex)
        {
            DockerAvailable = false;
            if (Environment.GetEnvironmentVariable("YOURAPP_TESTS_REQUIRE_DOCKER") == "1")
                throw new InvalidOperationException(
                    "Docker unavailable but YOURAPP_TESTS_REQUIRE_DOCKER=1. " +
                    "Install Docker Desktop or unset the env var.", ex);
        }
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync();
            _container = null;
        }
    }
}

[CollectionDefinition("postgres")]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture> { }