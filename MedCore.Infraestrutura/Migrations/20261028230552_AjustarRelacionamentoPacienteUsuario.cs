using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedCore.Infraestrutura.Migrations
{
    /// <inheritdoc />
    public partial class AjustarRelacionamentoPacienteUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pacientes_IdUsuario",
                table: "Pacientes");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_IdUsuario",
                table: "Pacientes",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pacientes_IdUsuario",
                table: "Pacientes");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_IdUsuario",
                table: "Pacientes",
                column: "IdUsuario",
                unique: true,
                filter: "[IdUsuario] IS NOT NULL");
        }
    }
}
