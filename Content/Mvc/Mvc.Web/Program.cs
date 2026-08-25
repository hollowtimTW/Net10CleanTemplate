using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YourApp.AuditLog;
using YourApp.FileStorage;
using YourApp.Identity;
using YourApp.Notification;
using YourApp.Shared.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddYourAppHosting("YourApp.Mvc.Web");
builder.Services.AddYourAppIdentity(opt => { opt.EnableJwt = false; opt.EnableWindows = false; });
builder.Services.AddYourAppAuditLog();
builder.Services.AddYourAppNotification();
builder.Services.AddLocalFileStorage(@"C:\YourApp\uploads");
builder.Services.AddControllersWithViews();

var app = builder.Build();
app.UseYourAppDefaults();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
await app.RunAsync();

public partial class Program { }