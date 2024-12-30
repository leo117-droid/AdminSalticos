using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEventoServicioAdicionalMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_EventoAlimentacion_Cantidad1",
                table: "EventoMobiliario");

            migrationBuilder.CreateTable(
                name: "EventoServicioAdicional",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdServicioAdicional = table.Column<int>(type: "int", nullable: false),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoServicioAdicional", x => x.Id);
                    table.CheckConstraint("CK_EventoServicioAdicional_Cantidad", "Cantidad > 0");
                    table.ForeignKey(
                        name: "FK_EventoServicioAdicional_Eventos_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Eventos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventoServicioAdicional_ServiciosAdicionales_IdServicioAdicional",
                        column: x => x.IdServicioAdicional,
                        principalTable: "ServiciosAdicionales",
                        principalColumn: "Id");
                });

            migrationBuilder.AddCheckConstraint(
                name: "CK_EventoMobiliario_Cantidad",
                table: "EventoMobiliario",
                sql: "Cantidad > 0");

            migrationBuilder.CreateIndex(
                name: "IX_EventoServicioAdicional_IdEvento",
                table: "EventoServicioAdicional",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_EventoServicioAdicional_IdServicioAdicional",
                table: "EventoServicioAdicional",
                column: "IdServicioAdicional");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoServicioAdicional");

            migrationBuilder.DropCheckConstraint(
                name: "CK_EventoMobiliario_Cantidad",
                table: "EventoMobiliario");

            migrationBuilder.AddCheckConstraint(
                name: "CK_EventoAlimentacion_Cantidad1",
                table: "EventoMobiliario",
                sql: "Cantidad > 0");
        }
    }
}
