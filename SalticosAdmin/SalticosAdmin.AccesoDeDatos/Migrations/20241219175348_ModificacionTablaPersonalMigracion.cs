using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class ModificacionTablaPersonalMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PadreId",
                table: "Personal",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personal_PadreId",
                table: "Personal",
                column: "PadreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personal_Personal_PadreId",
                table: "Personal",
                column: "PadreId",
                principalTable: "Personal",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personal_Personal_PadreId",
                table: "Personal");

            migrationBuilder.DropIndex(
                name: "IX_Personal_PadreId",
                table: "Personal");

            migrationBuilder.DropColumn(
                name: "PadreId",
                table: "Personal");
        }
    }
}
