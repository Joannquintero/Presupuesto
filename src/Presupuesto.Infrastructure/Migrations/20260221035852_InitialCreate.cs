using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasPresupuesto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasPresupuesto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PresupuestosMensuales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Anio = table.Column<int>(type: "INTEGER", nullable: false),
                    Mes = table.Column<int>(type: "INTEGER", nullable: false),
                    Monto = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Concepto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresupuestosMensuales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gastos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CategoriaPresupuestoId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubCategoria = table.Column<int>(type: "INTEGER", nullable: true),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Monto = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gastos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gastos_CategoriasPresupuesto_CategoriaPresupuestoId",
                        column: x => x.CategoriaPresupuestoId,
                        principalTable: "CategoriasPresupuesto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DistribucionesPresupuesto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PresupuestoMensualId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoriaPresupuestoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Porcentaje = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    Monto = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DistribucionesPresupuesto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DistribucionesPresupuesto_CategoriasPresupuesto_CategoriaPresupuestoId",
                        column: x => x.CategoriaPresupuestoId,
                        principalTable: "CategoriasPresupuesto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DistribucionesPresupuesto_PresupuestosMensuales_PresupuestoMensualId",
                        column: x => x.PresupuestoMensualId,
                        principalTable: "PresupuestosMensuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaldosPresupuesto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PresupuestoMensualId = table.Column<int>(type: "INTEGER", nullable: false),
                    Monto = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Concepto = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaldosPresupuesto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaldosPresupuesto_PresupuestosMensuales_PresupuestoMensualId",
                        column: x => x.PresupuestoMensualId,
                        principalTable: "PresupuestosMensuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CategoriasPresupuesto",
                columns: new[] { "Id", "Nombre" },
                values: new object[,]
                {
                    { 1, "Obligaciones" },
                    { 2, "Gustos personales" },
                    { 3, "Metas-Ahorro" },
                    { 4, "Imprevistos" },
                    { 5, "Otros" }
                });

            migrationBuilder.InsertData(
                table: "Gastos",
                columns: new[] { "Id", "CategoriaPresupuestoId", "Descripcion", "Fecha", "Monto", "SubCategoria" },
                values: new object[,]
                {
                    { 1, 1, "Supermercado semanal", new DateTime(2026, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 150.50m, 1 },
                    { 2, 1, "Gasolina", new DateTime(2026, 2, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), 80.00m, 2 },
                    { 3, 1, "Factura de luz", new DateTime(2026, 2, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 65.30m, 3 },
                    { 4, 2, "Cine y cena", new DateTime(2026, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 45.00m, null },
                    { 5, 1, "Restaurante", new DateTime(2026, 2, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 32.50m, 1 },
                    { 6, 1, "Internet", new DateTime(2026, 2, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), 55.00m, 3 },
                    { 7, 1, "Medicinas", new DateTime(2026, 2, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 28.75m, 5 },
                    { 8, 1, "Compras mensuales", new DateTime(2026, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 200.00m, 1 },
                    { 9, 1, "Mantenimiento auto", new DateTime(2026, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 120.00m, 2 },
                    { 10, 2, "Concierto", new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 75.00m, null },
                    { 11, 1, "Agua y gas", new DateTime(2026, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), 48.50m, 3 },
                    { 12, 5, "Regalos", new DateTime(2026, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), 60.00m, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionesPresupuesto_CategoriaPresupuestoId",
                table: "DistribucionesPresupuesto",
                column: "CategoriaPresupuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionesPresupuesto_PresupuestoMensualId",
                table: "DistribucionesPresupuesto",
                column: "PresupuestoMensualId");

            migrationBuilder.CreateIndex(
                name: "IX_Gastos_CategoriaPresupuestoId",
                table: "Gastos",
                column: "CategoriaPresupuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_SaldosPresupuesto_PresupuestoMensualId",
                table: "SaldosPresupuesto",
                column: "PresupuestoMensualId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistribucionesPresupuesto");

            migrationBuilder.DropTable(
                name: "Gastos");

            migrationBuilder.DropTable(
                name: "SaldosPresupuesto");

            migrationBuilder.DropTable(
                name: "CategoriasPresupuesto");

            migrationBuilder.DropTable(
                name: "PresupuestosMensuales");
        }
    }
}
