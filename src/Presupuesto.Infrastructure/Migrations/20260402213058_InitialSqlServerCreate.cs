using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlServerCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoriasPresupuesto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EsSistema = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasPresupuesto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PresupuestosMensuales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresupuestosMensuales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gastos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoriaPresupuestoId = table.Column<int>(type: "int", nullable: false),
                    SubCategoria = table.Column<int>(type: "int", nullable: true),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PresupuestoMensualId = table.Column<int>(type: "int", nullable: false),
                    CategoriaPresupuestoId = table.Column<int>(type: "int", nullable: false),
                    Porcentaje = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Bloqueada = table.Column<bool>(type: "bit", nullable: false)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PresupuestoMensualId = table.Column<int>(type: "int", nullable: false),
                    CategoriaPresupuestoId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaldosPresupuesto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaldosPresupuesto_CategoriasPresupuesto_CategoriaPresupuestoId",
                        column: x => x.CategoriaPresupuestoId,
                        principalTable: "CategoriasPresupuesto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaldosPresupuesto_PresupuestosMensuales_PresupuestoMensualId",
                        column: x => x.PresupuestoMensualId,
                        principalTable: "PresupuestosMensuales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CategoriasPresupuesto",
                columns: new[] { "Id", "EsSistema", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "Obligaciones" },
                    { 2, false, "Gastos Personales" },
                    { 3, false, "Metas y Ahorro" },
                    { 4, false, "Fondo de Apoyo" },
                    { 5, false, "Otros" }
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
                name: "IX_SaldosPresupuesto_CategoriaPresupuestoId",
                table: "SaldosPresupuesto",
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
