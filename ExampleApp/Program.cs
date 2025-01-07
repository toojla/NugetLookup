// See https://aka.ms/new-console-template for more information
using System.Reflection;
using NugetLookup;

Console.WriteLine("Hello, World!");

var iterator = new NugetDependencyIterator();

// get name of assembly ConsoleApp
var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

var result = iterator.GetNuGetDependencies(assemblyName, location);

if (!result.HasResult)
{
    Console.WriteLine(result.ErrorMessage);
    return;
}

Console.WriteLine("Dependencies");
foreach (var dependency in result.Dependencies)
{
    Console.WriteLine($"Name: {dependency.Name}, AssemblyVersion: {dependency.AssemblyVersion}, FileVersion: {dependency.FileVersion}");
}