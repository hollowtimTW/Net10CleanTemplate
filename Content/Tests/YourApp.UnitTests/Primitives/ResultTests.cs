using AwesomeAssertions;
using YourApp.Domain.Primitives;
using Xunit;

namespace YourApp.UnitTests.Primitives;

public class ResultTests
{
    [Fact]
    public void Success_value_is_accessible()
    {
        Result<int> r = Result<int>.Success(42);
        r.IsSuccess.Should().BeTrue();
        r.IsFailed.Should().BeFalse();
        r.Value.Should().Be(42);
    }

    [Fact]
    public void Failure_is_accessible()
    {
        var err = DomainError.Validation("bad");
        Result<int> r = Result<int>.Failure(err);
        r.IsFailed.Should().BeTrue();
        r.Error.Should().Be(err);
    }

    [Fact]
    public void Reading_value_of_failed_result_throws()
    {
        Result<int> r = Result<int>.Failure(DomainError.NotFound("missing"));
        Action act = () => { var _ = r.Value; };
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Result_Unit_static_Success_works()
    {
        Result<Unit> r = Result.Success();
        r.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Implicit_conversion_from_T_wraps_in_Success()
    {
        Result<string> r = "hello";
        r.IsSuccess.Should().BeTrue();
        r.Value.Should().Be("hello");
    }
}