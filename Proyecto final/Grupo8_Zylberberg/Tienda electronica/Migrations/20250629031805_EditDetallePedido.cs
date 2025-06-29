using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tienda_electronica.Migrations
{
    /// <inheritdoc />
    public partial class EditDetallePedido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detalles_Pedidos_PedidoIdPedido",
                table: "Detalles");

            migrationBuilder.DropIndex(
                name: "IX_Detalles_PedidoIdPedido",
                table: "Detalles");

            migrationBuilder.DropColumn(
                name: "PedidoIdPedido",
                table: "Detalles");

            migrationBuilder.CreateIndex(
                name: "IX_Detalles_IdPedido",
                table: "Detalles",
                column: "IdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_Detalles_Pedidos_IdPedido",
                table: "Detalles",
                column: "IdPedido",
                principalTable: "Pedidos",
                principalColumn: "IdPedido",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detalles_Pedidos_IdPedido",
                table: "Detalles");

            migrationBuilder.DropIndex(
                name: "IX_Detalles_IdPedido",
                table: "Detalles");

            migrationBuilder.AddColumn<int>(
                name: "PedidoIdPedido",
                table: "Detalles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Detalles_PedidoIdPedido",
                table: "Detalles",
                column: "PedidoIdPedido");

            migrationBuilder.AddForeignKey(
                name: "FK_Detalles_Pedidos_PedidoIdPedido",
                table: "Detalles",
                column: "PedidoIdPedido",
                principalTable: "Pedidos",
                principalColumn: "IdPedido",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
