using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedRateSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDaXuLyToPhieuCanhBao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DaXuLy",
                table: "PhieuCanhBaoADR",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

            migrationBuilder.AddColumn<string>(
                name: "NhanXet",
                table: "ChiTietKhaoSat",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaXuLy",
                table: "PhieuCanhBaoADR");

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

            migrationBuilder.DropColumn(
                name: "NhanXet",
                table: "ChiTietKhaoSat");
        }
    }
}
