using AwesomeAssertions;
using YourApp.Domain.Primitives;
using Xunit;

namespace YourApp.UnitTests.Aggregates;

/// <summary>
/// Sample Aggregate to prove the template's domain primitives work for real
/// domain logic. Replace this with your own aggregate when starting a new project.
/// </summary>
public class CustomerAggregateTests
{
    private sealed record CustomerId(Guid Value) : GuidId(Value);
    private sealed record Email(string Value);

    private sealed class Customer : AggregateRoot<CustomerId>
    {
        public string Name { get; private set; } = string.Empty;
        public Email? PrimaryEmail { get; private set; }
        public bool IsActive { get; private set; }

        public static Result<Customer> Register(string name, Email? email)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Customer>.Failure(DomainError.Validation("Name is required."));
            return Result<Customer>.Success(new Customer
            {
                Id = new CustomerId(Guid.NewGuid()),
                Name = name,
                PrimaryEmail = email,
                IsActive = true
            });
        }

        public Result<Unit> ChangeEmail(Email? newEmail)
        {
            if (string.IsNullOrWhiteSpace(newEmail?.Value))
                return Result.Failure(DomainError.Validation("Email cannot be empty."));
            PrimaryEmail = newEmail;
            return Result.Success();
        }

        public Result<Unit> Deactivate()
        {
            if (!IsActive) return Result.Success();
            IsActive = false;
            return Result.Success();
        }
    }

    [Fact]
    public void Register_succeeds_with_valid_name()
    {
        var result = Customer.Register("Acme Inc.", new Email("ops@acme.test"));
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Acme Inc.");
        result.Value.IsActive.Should().BeTrue();
        result.Value.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public void Register_fails_with_blank_name()
    {
        var result = Customer.Register("", new Email("ops@acme.test"));
        result.IsFailed.Should().BeTrue();
        result.Error.Category.Should().Be(ErrorCategory.Validation);
    }

    [Fact]
    public void ChangeEmail_updates_email()
    {
        var customer = Customer.Register("Acme", new Email("a@a.test")).Value;
        var result = customer.ChangeEmail(new Email("b@b.test"));
        result.IsSuccess.Should().BeTrue();
        customer.PrimaryEmail!.Value.Should().Be("b@b.test");
    }

    [Fact]
    public void Deactivate_is_idempotent()
    {
        var customer = Customer.Register("Acme", null).Value;
        customer.Deactivate().IsSuccess.Should().BeTrue();
        customer.IsActive.Should().BeFalse();
        customer.Deactivate().IsSuccess.Should().BeTrue();
        customer.IsActive.Should().BeFalse();
    }
}