using Microsoft.EntityFrameworkCore;
using Presupuesto.Domain.Entities;
using Presupuesto.Domain.Enums;

namespace Presupuesto.Infrastructure.Data;

/// <summary>
/// Contexto de base de datos de la aplicación.
/// </summary>
public class PresupuestoDbContext : DbContext
{
    public PresupuestoDbContext(DbContextOptions<PresupuestoDbContext> options) : base(options)
    {
    }

    public DbSet<Gasto> Gastos => Set<Gasto>();
    public DbSet<PresupuestoMensual> PresupuestosMensuales => Set<PresupuestoMensual>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la entidad Gasto
        modelBuilder.Entity<Gasto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descripcion).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Monto).HasPrecision(18, 2);
            entity.Property(e => e.Categoria).HasConversion<int>();
        });

        // Configuración de la entidad PresupuestoMensual
        modelBuilder.Entity<PresupuestoMensual>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Monto).HasPrecision(18, 2);
            entity.Property(e => e.Concepto).HasMaxLength(200).IsRequired(false);
        });

        // Seed de datos iniciales
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var hoy = DateTime.Today;
        var mesActual = new DateTime(hoy.Year, hoy.Month, 1);
        var mesAnterior = mesActual.AddMonths(-1);

        modelBuilder.Entity<Gasto>().HasData(
            // Gastos del mes actual
            new Gasto { Id = 1, Fecha = mesActual.AddDays(1), Categoria = Categoria.Alimentacion, Descripcion = "Supermercado semanal", Monto = 150.50m },
            new Gasto { Id = 2, Fecha = mesActual.AddDays(3), Categoria = Categoria.Transporte, Descripcion = "Gasolina", Monto = 80.00m },
            new Gasto { Id = 3, Fecha = mesActual.AddDays(5), Categoria = Categoria.Servicios, Descripcion = "Factura de luz", Monto = 65.30m },
            new Gasto { Id = 4, Fecha = mesActual.AddDays(7), Categoria = Categoria.Entretenimiento, Descripcion = "Cine y cena", Monto = 45.00m },
            new Gasto { Id = 5, Fecha = mesActual.AddDays(10), Categoria = Categoria.Alimentacion, Descripcion = "Restaurante", Monto = 32.50m },
            new Gasto { Id = 6, Fecha = mesActual.AddDays(12), Categoria = Categoria.Servicios, Descripcion = "Internet", Monto = 55.00m },
            new Gasto { Id = 7, Fecha = mesActual.AddDays(15), Categoria = Categoria.Otros, Descripcion = "Medicinas", Monto = 28.75m },
            
            // Gastos del mes anterior
            new Gasto { Id = 8, Fecha = mesAnterior.AddDays(2), Categoria = Categoria.Alimentacion, Descripcion = "Compras mensuales", Monto = 200.00m },
            new Gasto { Id = 9, Fecha = mesAnterior.AddDays(5), Categoria = Categoria.Transporte, Descripcion = "Mantenimiento auto", Monto = 120.00m },
            new Gasto { Id = 10, Fecha = mesAnterior.AddDays(10), Categoria = Categoria.Entretenimiento, Descripcion = "Concierto", Monto = 75.00m },
            new Gasto { Id = 11, Fecha = mesAnterior.AddDays(15), Categoria = Categoria.Servicios, Descripcion = "Agua y gas", Monto = 48.50m },
            new Gasto { Id = 12, Fecha = mesAnterior.AddDays(20), Categoria = Categoria.Otros, Descripcion = "Regalos", Monto = 60.00m }
        );
    }
}
