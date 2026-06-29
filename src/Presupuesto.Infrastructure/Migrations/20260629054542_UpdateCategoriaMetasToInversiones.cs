using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCategoriaMetasToInversiones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Inversiones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Metas y Ahorro");
        }
    }
}
