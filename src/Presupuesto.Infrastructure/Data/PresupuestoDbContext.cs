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
    public DbSet<SaldoPresupuesto> SaldosPresupuesto => Set<SaldoPresupuesto>();
    public DbSet<CategoriaPresupuesto> CategoriasPresupuesto => Set<CategoriaPresupuesto>();
    public DbSet<DistribucionPresupuesto> DistribucionesPresupuesto => Set<DistribucionPresupuesto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la entidad CategoriaPresupuesto
        modelBuilder.Entity<CategoriaPresupuesto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
        });

        // Configuración de la entidad DistribucionPresupuesto
        modelBuilder.Entity<DistribucionPresupuesto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Porcentaje).HasPrecision(5, 2);
            entity.Property(e => e.Monto).HasPrecision(18, 2);

            entity.HasOne(e => e.PresupuestoMensual)
                  .WithMany(p => p.Distribuciones)
                  .HasForeignKey(e => e.PresupuestoMensualId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CategoriaPresupuesto)
                  .WithMany()
                  .HasForeignKey(e => e.CategoriaPresupuestoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de la entidad Gasto
        modelBuilder.Entity<Gasto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Descripcion).HasMaxLength(200).IsRequired(false);
            entity.Property(e => e.Monto).HasPrecision(18, 2);
            entity.Property(e => e.SubCategoria).HasConversion<int>().IsRequired(false);
            
            entity.HasOne(e => e.CategoriaPresupuesto)
                  .WithMany()
                  .HasForeignKey(e => e.CategoriaPresupuestoId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de la entidad PresupuestoMensual
        modelBuilder.Entity<PresupuestoMensual>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Monto).HasPrecision(18, 2);
            entity.Property(e => e.Concepto).HasMaxLength(200).IsRequired(false);
        });

        // Configuración de la entidad SaldoPresupuesto
        modelBuilder.Entity<SaldoPresupuesto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Monto).HasPrecision(18, 2);
            entity.Property(e => e.Concepto).HasMaxLength(200).IsRequired(false);
            entity.Property(e => e.Tipo).HasConversion<int>();
            entity.HasOne(e => e.PresupuestoMensual)
                  .WithMany(p => p.Saldos)
                  .HasForeignKey(e => e.PresupuestoMensualId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed de datos iniciales
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categorías Presupuesto
        modelBuilder.Entity<CategoriaPresupuesto>().HasData(
            new CategoriaPresupuesto { Id = 1, Nombre = "Obligaciones", EsSistema = true },
            new CategoriaPresupuesto { Id = 2, Nombre = "Gastos Personales", EsSistema = false },
            new CategoriaPresupuesto { Id = 3, Nombre = "Metas y Ahorro", EsSistema = false },
            new CategoriaPresupuesto { Id = 4, Nombre = "Fondo de Apoyo", EsSistema = false },
            new CategoriaPresupuesto { Id = 5, Nombre = "Otros", EsSistema = false }
        );

        var hoy = DateTime.Today;
        var mesActual = new DateTime(hoy.Year, hoy.Month, 1);
        var mesAnterior = mesActual.AddMonths(-1);

        modelBuilder.Entity<Gasto>().HasData(
            // Gastos del mes actual - Usando CategoriaPresupuestoId 1 (Obligaciones)
            new Gasto { Id = 1, Fecha = mesActual.AddDays(1), CategoriaPresupuestoId = 1, SubCategoria = Categoria.Alimentacion, Descripcion = "Supermercado semanal", Monto = 150.50m },
            new Gasto { Id = 2, Fecha = mesActual.AddDays(3), CategoriaPresupuestoId = 1, SubCategoria = Categoria.Transporte, Descripcion = "Gasolina", Monto = 80.00m },
            new Gasto { Id = 3, Fecha = mesActual.AddDays(5), CategoriaPresupuestoId = 1, SubCategoria = Categoria.Servicios, Descripcion = "Factura de luz", Monto = 65.30m },
            new Gasto { Id = 4, Fecha = mesActual.AddDays(7), CategoriaPresupuestoId = 2, SubCategoria = null, Descripcion = "Cine y cena", Monto = 45.00m },
            new Gasto { Id = 5, Fecha = mesActual.AddDays(10), CategoriaPresupuestoId = 1, SubCategoria = Categoria.Alimentacion, Descripcion = "Restaurante", Monto = 32.50m },
            new Gasto { Id = 6, Fecha = mesActual.AddDays(12), CategoriaPresupuestoId = 1, SubCategoria = Categoria.Servicios, Descripcion = "Internet", Monto = 55.00m },
            new Gasto { Id = 7, Fecha = mesActual.AddDays(15), CategoriaPresupuestoId = 1, SubCategoria = Categoria.Otros, Descripcion = "Medicinas", Monto = 28.75m },
            
            // Gastos del mes anterior
            new Gasto { Id = 8, Fecha = mesAnterior.AddDays(2), CategoriaPresupuestoId = 1, SubCategoria = Categoria.Alimentacion, Descripcion = "Compras mensuales", Monto = 200.00m },
            new Gasto { Id = 9, Fecha = mesAnterior.AddDays(5), CategoriaPresupuestoId = 1, SubCategoria = Categoria.Transporte, Descripcion = "Mantenimiento auto", Monto = 120.00m },
            new Gasto { Id = 10, Fecha = mesAnterior.AddDays(10), CategoriaPresupuestoId = 2, SubCategoria = null, Descripcion = "Concierto", Monto = 75.00m },
            new Gasto { Id = 11, Fecha = mesAnterior.AddDays(15), CategoriaPresupuestoId = 1, SubCategoria = Categoria.Servicios, Descripcion = "Agua y gas", Monto = 48.50m },
            new Gasto { Id = 12, Fecha = mesAnterior.AddDays(20), CategoriaPresupuestoId = 5, SubCategoria = null, Descripcion = "Regalos", Monto = 60.00m }
        );
    }
}
