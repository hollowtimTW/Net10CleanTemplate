using Microsoft.AspNetCore.Mvc;

namespace YourApp.Mvc.Web.Controllers;

public sealed class HomeController : Controller
{
    public IActionResult Index() => View();
}