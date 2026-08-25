using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace YourApp.Mvc.Web.ViewModels;

public sealed record CustomerIndexItem(Guid Id, string Code, string Name, string? Email, DateTime CreatedAt);

public sealed class CustomerIndexViewModel
{
    public List<CustomerIndexItem> Items { get; set; } = [];
}

public sealed class CustomerCreateViewModel
{
    [Required, StringLength(32)]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(128)]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }
}

public sealed class CustomerEditViewModel
{
    [HiddenInput]
    public Guid Id { get; set; }

    [Required, StringLength(32)]
    public string Code { get; set; } = string.Empty;

    [Required, StringLength(128)]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }
}

public sealed class CustomerDetailsViewModel
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; }
}