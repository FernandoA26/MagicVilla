using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVilla_API.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarBd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImageUrl", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "Detalle de la villa ...", new DateTime(2024, 8, 20, 19, 54, 0, 843, DateTimeKind.Local).AddTicks(9971), new DateTime(2024, 8, 20, 19, 54, 0, 843, DateTimeKind.Local).AddTicks(9983), "", 50, "Villa Real", 5, 200.0 },
                    { 2, "", "Detalle de la villa ...", new DateTime(2024, 8, 20, 19, 54, 0, 843, DateTimeKind.Local).AddTicks(9988), new DateTime(2024, 8, 20, 19, 54, 0, 843, DateTimeKind.Local).AddTicks(9989), "", 40, "Premium Vista a la piscina", 4, 150.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
