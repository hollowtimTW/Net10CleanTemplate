using Microsoft.AspNetCore.Mvc;
using YourApp.Mvc.Web.ViewModels;

namespace YourApp.Mvc.Web.Controllers;

public sealed class CustomersController : Controller
{
    public IActionResult Index()
    {
        var vm = new CustomerIndexViewModel
        {
            Items =
            [
                new(Guid.NewGuid(), "C001", "Acme Inc.", "ops@acme.test", DateTime.UtcNow.AddDays(-30)),
                new(Guid.NewGuid(), "C002", "Wayne Co.", "ceo@wayne.test", DateTime.UtcNow.AddDays(-12)),
                new(Guid.NewGuid(), "C003", "Stark Ind.", "tony@stark.test", DateTime.UtcNow.AddDays(-3)),
            ]
        };
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create() => View(new CustomerCreateViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CustomerCreateViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        // TODO: dispatch MediatR CreateCustomerCommand
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(Guid id)
    {
        // TODO: load
        var vm = new CustomerEditViewModel { Id = id, Code = "C001", Name = "Acme Inc.", Email = "ops@acme.test" };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(CustomerEditViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);
        // TODO: dispatch UpdateCustomerCommand
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Details(Guid id)
    {
        var vm = new CustomerDetailsViewModel
        {
            Id = id,
            Code = "C001",
            Name = "Acme Inc.",
            Email = "ops@acme.test",
            CreatedAt = DateTime.UtcNow.AddDays(-30)
        };
        return View(vm);
    }
}