using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Presupuesto.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateListaDeseoSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old ListasDeseos table if it exists (schema changed)
            migrationBuilder.Sql(@"
                IF OBJECT_ID(N'[ListasDeseos]', N'U') IS NOT NULL
                BEGIN
                    DROP TABLE [ListasDeseos];
                END
            ");

            migrationBuilder.CreateTable(
                name: "ListasDeseos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Concepto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Categoria = table.Column<int>(type: "int", nullable: false),
                    Prioridad = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasDeseos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListasDeseos");
        }
    }
}
