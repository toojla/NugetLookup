using NugetLookup;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

const string apiKey = "your-secure-api-key";
var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseNugetLookupMiddleware(options =>
{
    options.AssemblyName = assemblyName;
    options.AssemblyPath = location;
    options.ApiKey = apiKey;
    options.PathEquals = "nuget-lookup";
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();