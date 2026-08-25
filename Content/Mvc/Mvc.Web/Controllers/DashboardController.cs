using Microsoft.AspNetCore.Mvc;

namespace YourApp.Mvc.Web.Controllers;

public sealed class DashboardController : Controller
{
    public IActionResult Index()
    {
        var vm = new YourApp.Mvc.Web.ViewModels.DashboardViewModel
        {
            RevenueJson = "[120000, 145000, 132000, 168000, 156000, 182000]",
            CategoriesJson = "[['Hardware', 35],['Services', 25],['Support', 15],['Other', 25]]"
        };
        return View(vm);
    }
}