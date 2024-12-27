using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCapacitacionPersonalMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CapacitacionesPersonal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPersonal = table.Column<int>(type: "int", nullable: false),
                    IdCapacitacion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapacitacionesPersonal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CapacitacionesPersonal_Capacitaciones_IdCapacitacion",
                        column: x => x.IdCapacitacion,
                        principalTable: "Capacitaciones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CapacitacionesPersonal_Personal_IdPersonal",
                        column: x => x.IdPersonal,
                        principalTable: "Personal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CapacitacionesPersonal_IdCapacitacion",
                table: "CapacitacionesPersonal",
                column: "IdCapacitacion");

            migrationBuilder.CreateIndex(
                name: "IX_CapacitacionesPersonal_IdPersonal",
                table: "CapacitacionesPersonal",
                column: "IdPersonal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapacitacionesPersonal");
        }
    }
}
