using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGasSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar todos los datos para dejar la BD limpia (solo conservando Categorías)
            migrationBuilder.Sql("DELETE FROM Gastos;");
            migrationBuilder.Sql("DELETE FROM DistribucionesPresupuesto;");
            migrationBuilder.Sql("DELETE FROM SaldosPresupuesto;");
            migrationBuilder.Sql("DELETE FROM PresupuestosMensuales;");

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Gastos",
                keyColumn: "Id",
                keyValue: 12);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Gastos",
                columns: new[] { "Id", "CategoriaPresupuestoId", "Descripcion", "Fecha", "Monto", "SubCategoria" },
                values: new object[,]
                {
                    { 1, 1, "Supermercado semanal", new DateTime(2026, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 150.50m, 1 },
                    { 2, 1, "Gasolina", new DateTime(2026, 3, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 80.00m, 2 },
                    { 3, 1, "Factura de luz", new DateTime(2026, 3, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 65.30m, 3 },
                    { 4, 2, "Cine y cena", new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 45.00m, null },
                    { 5, 1, "Restaurante", new DateTime(2026, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 32.50m, 1 },
                    { 6, 1, "Internet", new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 55.00m, 3 },
                    { 7, 1, "Medicinas", new DateTime(2026, 3, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 28.75m, 5 },
                    { 8, 1, "Compras mensuales", new DateTime(2026, 2, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 200.00m, 1 },
                    { 9, 1, "Mantenimiento auto", new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 120.00m, 2 },
                    { 10, 2, "Concierto", new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 75.00m, null },
                    { 11, 1, "Agua y gas", new DateTime(2026, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 48.50m, 3 },
                    { 12, 5, "Regalos", new DateTime(2026, 2, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 60.00m, null }
                });
        }
    }
}
