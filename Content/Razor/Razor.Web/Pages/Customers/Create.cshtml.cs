using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YourApp.Razor.Web.Pages.Customers;

public sealed class CreateModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = default!;

    public sealed class InputModel
    {
        [Required, StringLength(32)]
        public string Code { get; set; } = string.Empty;

        [Required, StringLength(128)]
        public string Name { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }
    }

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) return Page();
        // TODO: dispatch MediatR CreateCustomerCommand.
        return RedirectToPage("Index");
    }
}