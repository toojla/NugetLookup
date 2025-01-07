# NugetLookup
A simple middleware to list your nuget dependencies on a given page in json format

## Usage
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
	app.UseNugetLookup("/nuget");
}
```