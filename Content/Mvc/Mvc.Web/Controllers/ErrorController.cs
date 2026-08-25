using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using YourApp.Mvc.Web.ViewModels;

namespace YourApp.Mvc.Web.Controllers;

public sealed class ErrorController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index() => View(new ErrorViewModel
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
    });
}