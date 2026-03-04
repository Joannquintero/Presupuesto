using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nombre",
                value: "Gastos Personales");

            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Metas y Ahorro");

            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nombre",
                value: "Fondo de Apoyo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 2,
                column: "Nombre",
                value: "Gustos personales");

            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Metas-Ahorro");

            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 4,
                column: "Nombre",
                value: "Imprevistos");
        }
    }
}
