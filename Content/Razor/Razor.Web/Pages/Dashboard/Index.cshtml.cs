using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YourApp.Razor.Web.Pages.Dashboard;

public sealed class IndexModel : PageModel
{
    public string RevenueJson { get; private set; } = "[]";
    public string CategoriesJson { get; private set; } = "[]";

    public void OnGet()
    {
        // TODO: load from your MediatR query or API
        RevenueJson = "[120000, 145000, 132000, 168000, 156000, 182000]";
        CategoriesJson = "[['Hardware', 35],['Services', 25],['Support', 15],['Other', 25]]";
    }
}