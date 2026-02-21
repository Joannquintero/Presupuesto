using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSaldoWithCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoriaPresupuestoId",
                table: "SaldosPresupuesto",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SaldosPresupuesto_CategoriaPresupuestoId",
                table: "SaldosPresupuesto",
                column: "CategoriaPresupuestoId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaldosPresupuesto_CategoriasPresupuesto_CategoriaPresupuestoId",
                table: "SaldosPresupuesto",
                column: "CategoriaPresupuestoId",
                principalTable: "CategoriasPresupuesto",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaldosPresupuesto_CategoriasPresupuesto_CategoriaPresupuestoId",
                table: "SaldosPresupuesto");

            migrationBuilder.DropIndex(
                name: "IX_SaldosPresupuesto_CategoriaPresupuestoId",
                table: "SaldosPresupuesto");

            migrationBuilder.DropColumn(
                name: "CategoriaPresupuestoId",
                table: "SaldosPresupuesto");
        }
    }
}
