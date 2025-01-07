using Microsoft.AspNetCore.Http;
using NugetLookup.Models;
using System.Text.Json;

namespace NugetLookup;

public class NugetLookupMiddleware(
    RequestDelegate next,
    NugetLookupOptions options)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (string.IsNullOrWhiteSpace(options.PathEquals))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Forbidden");
            return;
        }

        if (context.Request.Path.Equals($"/{options.PathEquals}"))
        {
            if (!context.Request.Query.TryGetValue("apiKey", out var extractedApiKey) || extractedApiKey != options.ApiKey)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Forbidden");
                return;
            }

            var nugetDependencyIterator = new NugetDependencyIterator();
            var result = nugetDependencyIterator.GetNuGetDependencies(options.AssemblyName, options.AssemblyPath);

            if (!result.HasResult)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(result.ErrorMessage);
                return;
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(result.Dependencies));
        }
        else
        {
            await next(context);
        }
    }
}