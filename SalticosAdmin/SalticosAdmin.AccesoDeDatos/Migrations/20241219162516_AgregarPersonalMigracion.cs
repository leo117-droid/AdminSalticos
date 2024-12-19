using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPersonalMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Personal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cedula = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "date", nullable: false),
                    FechaEntrada = table.Column<DateTime>(type: "date", nullable: false),
                    RolPersonalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Personal_RolPersonal_RolPersonalId",
                        column: x => x.RolPersonalId,
                        principalTable: "RolPersonal",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personal_RolPersonalId",
                table: "Personal",
                column: "RolPersonalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Personal");
        }
    }
}
