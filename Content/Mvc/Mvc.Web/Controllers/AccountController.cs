using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using YourApp.Mvc.Web.ViewModels;

namespace YourApp.Mvc.Web.Controllers;

public sealed class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
        => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        // TODO: wire to IUserDirectory / cookie sign-in
        await HttpContext.SignOutAsync();
        return LocalRedirect(vm.ReturnUrl ?? Url.Action(nameof(HomeController.Index), "Home")!);
    }
}