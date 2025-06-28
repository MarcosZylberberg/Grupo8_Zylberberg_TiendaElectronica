﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tienda_electronica.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEsDestacadoAProducto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsDestacado",
                table: "Productos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsDestacado",
                table: "Productos");
        }
    }
}
