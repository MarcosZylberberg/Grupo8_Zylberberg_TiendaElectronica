using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tienda_electronica.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClientePedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Usuarios_ClienteIdUsuario",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_ClienteIdUsuario",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "ClienteIdUsuario",
                table: "Pedidos");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdUsuario",
                table: "Pedidos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_UsuarioIdUsuario",
                table: "Pedidos",
                column: "UsuarioIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Usuarios_UsuarioIdUsuario",
                table: "Pedidos",
                column: "UsuarioIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedidos_Usuarios_UsuarioIdUsuario",
                table: "Pedidos");

            migrationBuilder.DropIndex(
                name: "IX_Pedidos_UsuarioIdUsuario",
                table: "Pedidos");

            migrationBuilder.DropColumn(
                name: "UsuarioIdUsuario",
                table: "Pedidos");

            migrationBuilder.AddColumn<int>(
                name: "ClienteIdUsuario",
                table: "Pedidos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pedidos_ClienteIdUsuario",
                table: "Pedidos",
                column: "ClienteIdUsuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedidos_Usuarios_ClienteIdUsuario",
                table: "Pedidos",
                column: "ClienteIdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
