using System.Data;
using DataAccess.Repositories.Interfaces;
using DataAccess.Repositories;
using MudBlazor.Services;
using NumberManagement.Components;
using NumberManagement.Services;
using NumberManagement.Services.Interfaces;
using System.Data.SQLite;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddTransient<IComputeService, ComputeService>();
builder.Services.AddTransient<INumberRepository, NumberRepository>();
builder.Services.AddTransient<IDataDownloadService, DataDownloadService>();

var connectionString = builder.Configuration.GetConnectionString("DataConnection");
builder.Services.AddTransient<IDbConnection>(sp => new SQLiteConnection(connectionString));

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
