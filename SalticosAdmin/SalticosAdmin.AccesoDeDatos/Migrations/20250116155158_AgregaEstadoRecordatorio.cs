using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregaEstadoRecordatorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRecordatorio",
                table: "Eventos");

            migrationBuilder.AddColumn<bool>(
                name: "EstadoRecordatorio",
                table: "Eventos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstadoRecordatorio",
                table: "Eventos");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRecordatorio",
                table: "Eventos",
                type: "datetime2",
                nullable: true);
        }
    }
}
