using Microsoft.EntityFrameworkCore;
using Presupuesto.Infrastructure;
using Presupuesto.Infrastructure.Data;
using Presupuesto.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configurar infraestructura (DbContext, servicios)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=presupuesto.db";
builder.Services.AddInfrastructure(connectionString);

var app = builder.Build();

// Aplicar migraciones y crear base de datos
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PresupuestoDbContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
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
