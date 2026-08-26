using AwesomeAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace YourApp.IntegrationTests;

/// <summary>
/// Smoke tests for the API host. These do NOT require Docker — they boot the
/// API in-process via WebApplicationFactory and hit / and /api/health.
/// A real DB-backed test would belong in [Collection("postgres")] below.
/// </summary>
public class ApiSmokeTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ApiSmokeTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(_ => { /* tweak if needed */ });
    }

    [Fact]
    public async Task Root_returns_ok()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/");
        res.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task Health_returns_healthy()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/api/health");
        res.IsSuccessStatusCode.Should().BeTrue();
        var body = await res.Content.ReadAsStringAsync();
        body.Should().Contain("healthy");
    }

    [Fact]
    public async Task Health_live_returns_ok()
    {
        var client = _factory.CreateClient();
        var res = await client.GetAsync("/health/live");
        res.IsSuccessStatusCode.Should().BeTrue();
    }
}