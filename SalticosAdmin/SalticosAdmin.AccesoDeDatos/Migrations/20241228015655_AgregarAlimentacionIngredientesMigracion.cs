using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAlimentacionIngredientesMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlimentacionIngredientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdIngrediente = table.Column<int>(type: "int", nullable: false),
                    IdAlimentacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlimentacionIngredientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlimentacionIngredientes_Alimentacion_IdAlimentacion",
                        column: x => x.IdAlimentacion,
                        principalTable: "Alimentacion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AlimentacionIngredientes_Ingredientes_IdIngrediente",
                        column: x => x.IdIngrediente,
                        principalTable: "Ingredientes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlimentacionIngredientes_IdAlimentacion",
                table: "AlimentacionIngredientes",
                column: "IdAlimentacion");

            migrationBuilder.CreateIndex(
                name: "IX_AlimentacionIngredientes_IdIngrediente",
                table: "AlimentacionIngredientes",
                column: "IdIngrediente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlimentacionIngredientes");
        }
    }
}
