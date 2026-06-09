using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedRateSystem.Migrations
{
    /// <inheritdoc />
    public partial class ThemCotNhanXet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiemHieuQua",
                table: "ChiTietKhaoSat");

            migrationBuilder.DropColumn(
                name: "DiemTacDungPhu",
                table: "ChiTietKhaoSat");

            migrationBuilder.DropColumn(
                name: "DiemTienLoi",
                table: "ChiTietKhaoSat");

            migrationBuilder.DropColumn(
                name: "DiemTongThe",
                table: "ChiTietKhaoSat");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ChiTietKhaoSat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiemHieuQua",
                table: "ChiTietKhaoSat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiemTacDungPhu",
                table: "ChiTietKhaoSat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiemTienLoi",
                table: "ChiTietKhaoSat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DiemTongThe",
                table: "ChiTietKhaoSat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ChiTietKhaoSat",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
