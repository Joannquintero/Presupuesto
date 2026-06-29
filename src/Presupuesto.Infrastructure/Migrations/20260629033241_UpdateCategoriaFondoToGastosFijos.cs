using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoriaFondoToGastosFijos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nombre",
                value: "Gastos Fijos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nombre",
                value: "Fondo de Apoyo");
        }
    }
}
