namespace NugetLookup.Models;

public record NugetDependencyResult(
    string FilePath = "",
    string ErrorMessage = "",
    bool HasResult = false,
    List<NugetDependencyItem>? Dependencies = null
);

public record NugetDependencyItem
(
    string Name = "",
    string? AssemblyVersion = "",
    string? FileVersion = ""
);