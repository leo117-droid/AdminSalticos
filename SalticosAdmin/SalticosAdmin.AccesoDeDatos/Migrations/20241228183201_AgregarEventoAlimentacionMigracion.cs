using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEventoAlimentacionMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventoAlimentacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAlimentacion = table.Column<int>(type: "int", nullable: false),
                    IdEvento = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoAlimentacion", x => x.Id);
                    table.CheckConstraint("CK_EventoAlimentacion_Cantidad", "Cantidad > 0");
                    table.ForeignKey(
                        name: "FK_EventoAlimentacion_Alimentacion_IdAlimentacion",
                        column: x => x.IdAlimentacion,
                        principalTable: "Alimentacion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventoAlimentacion_Eventos_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Eventos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoAlimentacion_IdAlimentacion",
                table: "EventoAlimentacion",
                column: "IdAlimentacion");

            migrationBuilder.CreateIndex(
                name: "IX_EventoAlimentacion_IdEvento",
                table: "EventoAlimentacion",
                column: "IdEvento");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoAlimentacion");
        }
    }
}
