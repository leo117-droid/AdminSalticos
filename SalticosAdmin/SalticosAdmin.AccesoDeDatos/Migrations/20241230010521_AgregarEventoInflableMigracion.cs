using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SalticosAdmin.AccesoDeDatos.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEventoInflableMigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventoInflable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdInflable = table.Column<int>(type: "int", nullable: false),
                    IdEvento = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoInflable", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoInflable_Eventos_IdEvento",
                        column: x => x.IdEvento,
                        principalTable: "Eventos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventoInflable_Inflables_IdInflable",
                        column: x => x.IdInflable,
                        principalTable: "Inflables",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoInflable_IdEvento",
                table: "EventoInflable",
                column: "IdEvento");

            migrationBuilder.CreateIndex(
                name: "IX_EventoInflable_IdInflable",
                table: "EventoInflable",
                column: "IdInflable");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventoInflable");
        }
    }
}
