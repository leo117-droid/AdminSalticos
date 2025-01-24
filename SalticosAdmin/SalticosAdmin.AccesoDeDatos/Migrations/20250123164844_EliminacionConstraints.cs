using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class EliminacionConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_EventoServicioAdicional_Cantidad",
                table: "EventoServicioAdicional");

            migrationBuilder.DropCheckConstraint(
                name: "CK_EventoMobiliario_Cantidad",
                table: "EventoMobiliario");

            migrationBuilder.DropCheckConstraint(
                name: "CK_EventoAlimentacion_Cantidad",
                table: "EventoAlimentacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_EventoServicioAdicional_Cantidad",
                table: "EventoServicioAdicional",
                sql: "Cantidad > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_EventoMobiliario_Cantidad",
                table: "EventoMobiliario",
                sql: "Cantidad > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_EventoAlimentacion_Cantidad",
                table: "EventoAlimentacion",
                sql: "Cantidad > 0");
        }
    }
}
