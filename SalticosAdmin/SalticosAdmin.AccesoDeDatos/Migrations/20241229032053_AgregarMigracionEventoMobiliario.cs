using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMigracionEventoMobiliario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventoMobiliario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdMobiliario = table.Column<int>(type: "int", nullable: false),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoMobiliario", x => x.Id);
                    table.CheckConstraint("CK_EventoAlimentacion_Cantidad1", "Cantidad > 0");
                    table.ForeignKey(
                        name: "FK_EventoMobiliario_Eventos_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Eventos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventoMobiliario_Mobilarios_IdMobiliario",
                        column: x => x.IdMobiliario,
                        principalTable: "Mobilarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoMobiliario_IdEvento",
                table: "EventoMobiliario",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_EventoMobiliario_IdMobiliario",
                table: "EventoMobiliario",
                column: "IdMobiliario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoMobiliario");
        }
    }
}
