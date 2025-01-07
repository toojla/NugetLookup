using NugetLookup.Models;
using System.Text.Json;

namespace NugetLookup
{
    public class NugetDependencyIterator
    {
        public NugetDependencyResult GetNuGetDependencies(string? assemblyName, string? assemblyPath)
        {
            var result = ValidateAssemblyNameAndFile(assemblyName, assemblyPath);

            if (result.HasResult is false) return result;

            var document = GetJsonDocument(result);

            if (document is null)
            {
                return new NugetDependencyResult
                {
                    ErrorMessage = "Error reading the json file",
                    HasResult = false
                };
            }

            var dependencies = GetDependencies(document);

            return new NugetDependencyResult
            {
                HasResult = true,
                Dependencies = dependencies.OrderBy(x => x.Name).ToList()
            };
        }

        private static List<NugetDependencyItem> GetDependencies(JsonDocument document)
        {
            var dependencies = document.RootElement.GetProperty("targets")
                .EnumerateObject()
                .SelectMany(target => target.Value.EnumerateObject())
                .Select(dependency =>
                {
                    var dependencyValue = dependency.Value;
                    var tryGetProperty = dependencyValue.TryGetProperty("runtime", out var runtimeProperty);
                    if (!tryGetProperty) return new NugetDependencyItem(dependency.Name, "Unknown", "Unknown");

                    //var runtime = dependency.Value.GetProperty("runtime").EnumerateObject().FirstOrDefault();
                    var runtime = runtimeProperty.EnumerateObject().FirstOrDefault();

                    //if (runtime == null) return new NugetDependencyItem(dependency.Name, "Unknown", "Unknown");
                    var fileVersion = runtime.Value.TryGetProperty("fileVersion", out var fileVersionProperty)
                        ? fileVersionProperty.GetString()
                        : "Unknown";
                    var assemblyVersion =
                        runtime.Value.TryGetProperty("assemblyVersion", out var assemblyVersionProperty)
                            ? assemblyVersionProperty.GetString()
                            : "Unknown";
                    return new NugetDependencyItem(dependency.Name, assemblyVersion, fileVersion);
                }).Distinct().ToList();

            return dependencies;
        }

        private static JsonDocument? GetJsonDocument(NugetDependencyResult result)
        {
            try
            {
                var json = File.ReadAllText(result.FilePath);
                var document = JsonDocument.Parse(json);
                return document;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        private static NugetDependencyResult ValidateAssemblyNameAndFile(string? assemblyName, string? assemblyPath)
        {
            if (string.IsNullOrWhiteSpace(assemblyName) || string.IsNullOrWhiteSpace(assemblyPath))
            {
                return new NugetDependencyResult
                {
                    ErrorMessage = "Incorrect or missing assembly name",
                    HasResult = false
                };
            }

            var fileName = $"{assemblyName}.deps.json";
            var filePath = Path.Combine(assemblyPath, fileName);

            if (!File.Exists(filePath))
            {
                return new NugetDependencyResult
                {
                    ErrorMessage = $"The json file ({fileName}) is not found, {filePath}",
                    HasResult = false
                };
            }

            return new NugetDependencyResult
            {
                FilePath = filePath,
                HasResult = true
            };
        }
    }
}