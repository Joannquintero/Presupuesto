using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoriaObligacionesToGastosBasicos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nombre",
                value: "Gastos Básicos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nombre",
                value: "Obligaciones");
        }
    }
}
