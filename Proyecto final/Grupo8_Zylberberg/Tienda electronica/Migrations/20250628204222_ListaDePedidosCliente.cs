using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tienda_electronica.Migrations
{
    /// <inheritdoc />
    public partial class ListaDePedidosCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detalles_Productos_ProductoIdProducto",
                table: "Detalles");

            migrationBuilder.DropIndex(
                name: "IX_Detalles_ProductoIdProducto",
                table: "Detalles");

            migrationBuilder.DropColumn(
                name: "ProductoIdProducto",
                table: "Detalles");

            migrationBuilder.CreateIndex(
                name: "IX_Detalles_IdProducto",
                table: "Detalles",
                column: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_Detalles_Productos_IdProducto",
                table: "Detalles",
                column: "IdProducto",
                principalTable: "Productos",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detalles_Productos_IdProducto",
                table: "Detalles");

            migrationBuilder.DropIndex(
                name: "IX_Detalles_IdProducto",
                table: "Detalles");

            migrationBuilder.AddColumn<int>(
                name: "ProductoIdProducto",
                table: "Detalles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Detalles_ProductoIdProducto",
                table: "Detalles",
                column: "ProductoIdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_Detalles_Productos_ProductoIdProducto",
                table: "Detalles",
                column: "ProductoIdProducto",
                principalTable: "Productos",
                principalColumn: "IdProducto",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
