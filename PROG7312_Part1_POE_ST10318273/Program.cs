using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PROG7312_Part1_POE_ST10318273.Data;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Create path for JSON file storage in the Data directory
var dataPath = Path.Combine(builder.Environment.ContentRootPath, "Data", "issues.json");

// Initialize the linked list repository with the data file path
var repo = new LinkedListIssueRepository(dataPath);

// Load any existing data from disk into memory
await repo.LoadFromDiskAsync();

builder.Services.AddSingleton<IIssueRepository>(repo);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

// Configure default route pattern
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
