using AwesomeAssertions;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using YourApp.Infrastructure.Persistence;
using Xunit;

namespace YourApp.IntegrationTests;

/// <summary>
/// Real PostgreSQL tests. Run only when Docker is available.
/// All tests in this class are skipped via Skip.IfNot(...) so CI without Docker
/// still reports green.
/// </summary>
[Collection("postgres")]
public class PostgresIntegrationTests
{
    private readonly PostgresFixture _fixture;

    public PostgresIntegrationTests(PostgresFixture fixture) => _fixture = fixture;

    [Fact]
    public async Task Can_connect_to_postgres_and_query_version()
    {
        Skip.IfNot(_fixture.DockerAvailable, "Docker not available — skipping Postgres integration test.");

        await using var conn = new NpgsqlConnection(_fixture.ConnectionString);
        await conn.OpenAsync();
        var version = await conn.QuerySingleAsync<string>("SELECT version();");

        version.Should().StartWith("PostgreSQL");
    }

    [Fact]
    public async Task NpgsqlConnectionFactory_returns_open_connection()
    {
        Skip.IfNot(_fixture.DockerAvailable, "Docker not available — skipping Postgres integration test.");

        var services = new ServiceCollection();
        var dsBuilder = new NpgsqlDataSourceBuilder(_fixture.ConnectionString);
        dsBuilder.EnableDynamicJson();
        var ds = dsBuilder.Build();
        services.AddSingleton(ds);
        services.AddSingleton<NpgsqlConnectionFactory>();
        await using var sp = services.BuildServiceProvider();

        var factory = sp.GetRequiredService<NpgsqlConnectionFactory>();
        await using var conn = await factory.CreateOpenAsync();

        conn.State.Should().Be(System.Data.ConnectionState.Open);
    }
}

internal static class Skip
{
    public static void IfNot(bool condition, string reason)
    {
        Skip.If(!condition, reason);
    }

    public static void If(bool condition, string reason)
    {
        if (condition) throw new SkipException(reason);
    }

    public sealed class SkipException : Exception
    {
        public SkipException(string message) : base(message) { }
    }
}