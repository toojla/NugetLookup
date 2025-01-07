namespace NugetLookup.Models;

public class NugetLookupOptions
{
    public string? ApiKey { get; set; }
    public string? PathEquals { get; set; }
    public string? AssemblyName { get; set; }
    public string? AssemblyPath { get; set; }
}