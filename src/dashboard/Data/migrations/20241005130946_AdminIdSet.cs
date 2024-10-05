using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dashboard.Data.migrations
{
    /// <inheritdoc />
    public partial class AdminIdSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "49229498-9df7-4026-84f3-cae91cce0e9a");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "533f6146-68ea-4332-8278-7b33cd95dc00", null, "admin", "ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "533f6146-68ea-4332-8278-7b33cd95dc00");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "49229498-9df7-4026-84f3-cae91cce0e9a", null, "admin", "ADMIN" });
        }
    }
}
