using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YourApp.Razor.Web.Pages.Customers;

public sealed class DetailsModel : PageModel
{
    [BindProperty(SupportsGet = true)] public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }

    public void OnGet()
    {
        // TODO load from DB
        Code = "C001";
        Name = "Acme Inc.";
        Email = "ops@acme.test";
        CreatedAt = DateTime.UtcNow.AddDays(-30);
    }
}