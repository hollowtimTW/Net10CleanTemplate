using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace YourApp.Identity;

public static class IdentityExtensions
{
    /// <summary>
    /// Wires up Cookie + JWT + Windows auth. Cookie for web, JWT for API/mobile,
    /// Negotiate for AD SSO. Tweak as needed.
    /// </summary>
    public static IServiceCollection AddYourAppIdentity(
        this IServiceCollection services,
        Action<IdentityOptions>? configure = null)
    {
        var options = new IdentityOptions();
        configure?.Invoke(options);

        var authBuilder = services.AddAuthentication(opts =>
        {
            opts.DefaultScheme = "Multi";
            opts.DefaultChallengeScheme = "Multi";
        });

        authBuilder.AddPolicyScheme("Multi", "Multi-scheme selector", opt =>
        {
            opt.ForwardDefaultSelector = ctx =>
            {
                // If Authorization header is "Bearer ...", use JWT
                var auth = ctx.Request.Headers.Authorization.ToString();
                if (auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    return JwtBearerDefaults.AuthenticationScheme;
                // If request is for /api/*, prefer JWT
                if (ctx.Request.Path.StartsWithSegments("/api"))
                    return JwtBearerDefaults.AuthenticationScheme;
                // Else cookie (web)
                return CookieAuthenticationDefaults.AuthenticationScheme;
            };
        });

        authBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt =>
        {
            opt.Cookie.Name = ".YourApp.Auth";
            opt.Cookie.HttpOnly = true;
            opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            opt.Cookie.SameSite = SameSiteMode.Lax;
            opt.SlidingExpiration = true;
            opt.ExpireTimeSpan = TimeSpan.FromHours(8);
        });

        if (options.EnableJwt)
        {
            authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
            {
                opt.RequireHttpsMetadata = true;
                opt.SaveToken = true;
                opt.TokenValidationParameters = options.JwtValidationParameters;
            });
        }

        if (options.EnableWindows)
        {
            authBuilder.AddNegotiate();
        }

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("Authenticated", p => p.RequireAuthenticatedUser());
        });

        services.AddHttpContextAccessor();
        return services;
    }
}

public sealed class IdentityOptions
{
    public bool EnableJwt { get; set; } = true;
    public bool EnableWindows { get; set; } = true;
    public Microsoft.IdentityModel.Tokens.TokenValidationParameters JwtValidationParameters { get; set; } = new();
}