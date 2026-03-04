using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEsSistemaToCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsSistema",
                table: "CategoriasPresupuesto",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 1,
                column: "EsSistema",
                value: true);

            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 2,
                column: "EsSistema",
                value: false);

            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 3,
                column: "EsSistema",
                value: false);

            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 4,
                column: "EsSistema",
                value: false);

            migrationBuilder.UpdateData(
                table: "CategoriasPresupuesto",
                keyColumn: "Id",
                keyValue: 5,
                column: "EsSistema",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsSistema",
                table: "CategoriasPresupuesto");
        }
    }
}
