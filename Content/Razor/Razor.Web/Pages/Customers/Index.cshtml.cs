using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YourApp.Razor.Web.Pages.Customers;

public sealed class IndexModel : PageModel
{
    public List<CustomerRow> Customers { get; private set; } = [];

    public void OnGet()
    {
        // TODO: replace with MediatR query against the database.
        Customers =
        [
            new CustomerRow(Guid.NewGuid(), "C001", "Acme Inc.",  "ops@acme.test",     DateTime.UtcNow.AddDays(-30)),
            new CustomerRow(Guid.NewGuid(), "C002", "Wayne Co.",  "ceo@wayne.test",    DateTime.UtcNow.AddDays(-12)),
            new CustomerRow(Guid.NewGuid(), "C003", "Stark Ind.", "tony@stark.test",   DateTime.UtcNow.AddDays(-3)),
        ];
    }

    public sealed record CustomerRow(Guid Id, string Code, string Name, string Email, DateTime CreatedAt);
}