using AwesomeAssertions;
using YourApp.Domain.Primitives;
using Xunit;

namespace YourApp.UnitTests.Primitives;

public class GuardTests
{
    [Fact]
    public void NotNull_throws_on_null()
    {
        string? value = null;
        Action act = () => Guard.NotNull(value, "x");
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void NotNull_passes_through_for_value()
    {
        var v = Guard.NotNull("hello", "x");
        v.Should().Be("hello");
    }

    [Fact]
    public void NotNullOrWhiteSpace_throws_on_empty()
    {
        Action act = () => Guard.NotNullOrWhiteSpace("  ", "x");
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Positive_throws_on_zero()
    {
        Action act = () => Guard.Positive(0, "x");
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Positive_throws_on_negative()
    {
        Action act = () => Guard.Positive(-1, "x");
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Positive_passes_for_one()
    {
        Guard.Positive(1, "x").Should().Be(1);
    }

    [Fact]
    public void NotEmpty_throws_on_empty_collection()
    {
        Action act = () => Guard.NotEmpty(Array.Empty<int>(), "x");
        act.Should().Throw<ArgumentException>();
    }
}