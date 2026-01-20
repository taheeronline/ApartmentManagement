using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Application.Services;
using ApartmentManagement.Infrastructure.Persistence;
using ApartmentManagement.Infrastructure.Repositories;
using ApartmentManagement.UI.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddDbContext<ApartmentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<iApartmentRepository, ApartmentRepository>();
builder.Services.AddScoped<iFlatRepository, FlatRepository>();
builder.Services.AddScoped<iResidentRepository, ResidentRepository>();

builder.Services.AddScoped<ApartmentService>();
builder.Services.AddScoped<FlatService>();
builder.Services.AddScoped<ResidentService>();

// Add logging for application services and repositories
builder.Services.AddLogging();

// Re-register services with ILogger-enabled constructors
builder.Services.AddScoped<ApartmentService, ApartmentService>();
builder.Services.AddScoped<FlatService, FlatService>();
builder.Services.AddScoped<ResidentService, ResidentService>();

builder.Services.AddScoped<ApartmentRepository>();
builder.Services.AddScoped<FlatRepository>();
builder.Services.AddScoped<ResidentRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
