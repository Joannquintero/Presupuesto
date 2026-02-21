using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSaldoPresupuesto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_SaldosPresupuesto_PresupuestoMensualId",
                table: "SaldosPresupuesto",
                column: "PresupuestoMensualId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaldosPresupuesto");
        }
    }
}
