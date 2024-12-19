using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCapacitacionMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Capacitaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "date", nullable: false),
                    Tema = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Duracion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capacitaciones", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Capacitaciones");
        }
    }
}
