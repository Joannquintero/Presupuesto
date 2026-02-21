using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presupuesto.Application.Services;
using Presupuesto.Infrastructure.Data;

namespace Presupuesto.Infrastructure;

/// <summary>
/// Extensiones para configurar los servicios de infraestructura.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        // Configurar DbContext con SQLite
        services.AddDbContext<PresupuestoDbContext>(options =>
            options.UseSqlite(connectionString));

        // Registrar DbContext base para el servicio
        services.AddScoped<DbContext>(provider => provider.GetRequiredService<PresupuestoDbContext>());

        // Registrar servicios
        services.AddScoped<IGastoService, GastoService>();
        services.AddScoped<IPresupuestoMensualService, PresupuestoMensualService>();
        services.AddScoped<ISaldoPresupuestoService, SaldoPresupuestoService>();

        return services;
    }
}
