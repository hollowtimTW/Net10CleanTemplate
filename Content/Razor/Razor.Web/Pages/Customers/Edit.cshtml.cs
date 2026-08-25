using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YourApp.Razor.Web.Pages.Customers;

public sealed class EditModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty, Required, StringLength(32)]
    public string Code { get; set; } = string.Empty;

    [BindProperty, Required, StringLength(128)]
    public string Name { get; set; } = string.Empty;

    [BindProperty, EmailAddress]
    public string? Email { get; set; }

    public void OnGet() { /* TODO load */ }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        // TODO dispatch UpdateCustomerCommand
        return RedirectToPage("Index");
    }
}