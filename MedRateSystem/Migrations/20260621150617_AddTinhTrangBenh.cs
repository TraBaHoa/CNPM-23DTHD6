using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedRateSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTinhTrangBenh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TinhTrangBenh",
                table: "PhieuKhaoSat",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TinhTrangBenh",
                table: "PhieuKhaoSat");
        }
    }
}
