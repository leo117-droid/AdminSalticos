using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFechaRecordatorioAEvento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRecordatorio",
                table: "Eventos",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRecordatorio",
                table: "Eventos");
        }
    }
}
