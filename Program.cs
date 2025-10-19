using SushiInventorySystem.Components;
using MudBlazor.Services;

// === Added for backend integration ===
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using SushiInventorySystem.Data;
using SushiInventorySystem.Models;
using SushiInventorySystem.Services.Interfaces;
using SushiInventorySystem.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// ==========================
//  1️. Add services to container
// ==========================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// === Added for backend integration ===
// Load configuration (for connection string)
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Register EF Core DbContext (using SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<IBranchService, BranchService>();
builder.Services.AddScoped<IStockService, StockService>();

var app = builder.Build();

// ==========================
//  2️. Configure HTTP request pipeline
// ==========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// ==========================
//  3. Map Razor components
// ==========================
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ==========================
//  4️. Database initialization and seeding
// ==========================
// This ensures DB is created and initial seed data is inserted automatically on startup.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    SeedData.Initialize(context);
}

// ==========================
//  5️. Optional: Test logic during development
// ==========================
// You can remove this block after verifying backend integration.
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();

        Console.WriteLine("\n=== Search for 'Sushi' ===");
        var searchResults = await itemService.SearchItemsAsync("Sushi");
        foreach (var i in searchResults)
            Console.WriteLine($"{i.ItemName} ({i.Category}) - ${i.CostPerUnit}");

        Console.WriteLine("\n=== Average cost per category ===");
        var avg = await itemService.GetAverageCostByCategoryAsync();
        foreach (var kv in avg)
            Console.WriteLine($"{kv.Key}: ${kv.Value:F2}");
    }
}

app.Run();
