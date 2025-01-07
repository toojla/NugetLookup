using Microsoft.AspNetCore.Builder;
using NugetLookup.Models;

namespace NugetLookup;

public static class NugetLookupMiddlewareExtensions
{
    public static IApplicationBuilder UseNugetLookupMiddleware(
        this IApplicationBuilder builder,
        Action<NugetLookupOptions> configureOptions)
    {
        var options = new NugetLookupOptions();
        configureOptions(options);
        return builder.UseMiddleware<NugetLookupMiddleware>(options);
    }
}