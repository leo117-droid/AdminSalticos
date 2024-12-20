using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarInflableMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inflables",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Dimensiones = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Precio = table.Column<double>(type: "float", nullable: false),
                    PrecioHoraAdicional = table.Column<double>(type: "float", nullable: false),
                    CategoriaTamannoId = table.Column<int>(type: "int", nullable: false),
                    CategoriaEdadId = table.Column<int>(type: "int", nullable: false),
                    PadreId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inflables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inflables_CategoriaTammano_CategoriaTamannoId",
                        column: x => x.CategoriaTamannoId,
                        principalTable: "CategoriaTammano",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Inflables_CategoriasEdades_CategoriaEdadId",
                        column: x => x.CategoriaEdadId,
                        principalTable: "CategoriasEdades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Inflables_Inflables_PadreId",
                        column: x => x.PadreId,
                        principalTable: "Inflables",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inflables_CategoriaEdadId",
                table: "Inflables",
                column: "CategoriaEdadId");

            migrationBuilder.CreateIndex(
                name: "IX_Inflables_CategoriaTamannoId",
                table: "Inflables",
                column: "CategoriaTamannoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inflables_PadreId",
                table: "Inflables",
                column: "PadreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inflables");
        }
    }
}
