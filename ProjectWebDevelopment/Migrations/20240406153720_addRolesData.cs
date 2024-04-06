using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectWebDevelopment.Migrations
{
    /// <inheritdoc />
    public partial class addRolesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "98f2372e-5872-4278-a9e1-367c0c2d2db2", null, "Buyer", "BUYER" },
                    { "e8c8bf03-b598-44e5-8cec-286e57368b0e", null, "Seller", "SELLER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "98f2372e-5872-4278-a9e1-367c0c2d2db2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e8c8bf03-b598-44e5-8cec-286e57368b0e");
        }
    }
}
