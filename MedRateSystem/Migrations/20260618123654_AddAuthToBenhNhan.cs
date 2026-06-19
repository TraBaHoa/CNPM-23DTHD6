using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedRateSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthToBenhNhan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MatKhau",
                table: "BenhNhan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TaiKhoan",
                table: "BenhNhan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VaiTro",
                table: "BenhNhan",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MatKhau",
                table: "BenhNhan");

            migrationBuilder.DropColumn(
                name: "TaiKhoan",
                table: "BenhNhan");

            migrationBuilder.DropColumn(
                name: "VaiTro",
                table: "BenhNhan");
        }
    }
}
