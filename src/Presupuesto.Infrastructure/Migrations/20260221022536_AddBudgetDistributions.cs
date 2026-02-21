using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetDistributions : Migration
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

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionesPresupuesto_CategoriaPresupuestoId",
                table: "DistribucionesPresupuesto",
                column: "CategoriaPresupuestoId");

            migrationBuilder.CreateIndex(
                name: "IX_DistribucionesPresupuesto_PresupuestoMensualId",
                table: "DistribucionesPresupuesto",
                column: "PresupuestoMensualId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DistribucionesPresupuesto");

            migrationBuilder.DropTable(
                name: "CategoriasPresupuesto");
        }
    }
}
