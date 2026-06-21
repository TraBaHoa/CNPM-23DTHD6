using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedRateSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BacSi",
                columns: table => new
                {
                    MaBacSi = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaiKhoan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacSi", x => x.MaBacSi);
                });

            migrationBuilder.CreateTable(
                name: "BenhNhan",
                columns: table => new
                {
                    MaBenhNhan = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: false),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SoDienThoai = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    TaiKhoan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BenhNhan__22A8B330A7903BBE", x => x.MaBenhNhan);
                });

            migrationBuilder.CreateTable(
                name: "Thuoc",
                columns: table => new
                {
                    MaThuoc = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    GiaTien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PhanLoaiVen = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenThuoc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NhaSanXuat = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DiemLikertTB = table.Column<double>(type: "float", nullable: true, defaultValue: 5.0),
                    TyLeADR = table.Column<double>(type: "float", nullable: true, defaultValue: 0.0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Thuoc__4BB1F620B5D55177", x => x.MaThuoc);
                });

            migrationBuilder.CreateTable(
                name: "DonThuoc",
                columns: table => new
                {
                    MaDonThuoc = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaBenhNhan = table.Column<string>(type: "varchar(20)", nullable: true),
                    NgayKeDon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BacSiKeDon = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonThuoc", x => x.MaDonThuoc);
                    table.ForeignKey(
                        name: "FK_DonThuoc_BenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan");
                });

            migrationBuilder.CreateTable(
                name: "PhieuKhaoSat",
                columns: table => new
                {
                    MaPhieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBenhNhan = table.Column<string>(type: "varchar(20)", nullable: false),
                    ThoiGianLamPhieu = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChuNhanXet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuKhaoSat", x => x.MaPhieu);
                    table.ForeignKey(
                        name: "FK_PhieuKhaoSat_BenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhieuCanhBaoADR",
                columns: table => new
                {
                    MaCanhBao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaThuoc = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    NgayPhatHien = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    TyLeThucTe = table.Column<double>(type: "float", nullable: false),
                    NoiDungCanhBao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DaXuLy = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PhieuCan__73C23D934378EE10", x => x.MaCanhBao);
                    table.ForeignKey(
                        name: "FK_CanhBao_Thuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonThuoc",
                columns: table => new
                {
                    MaDonThuoc = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MaThuoc = table.Column<string>(type: "varchar(20)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: true),
                    CachDung = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonThuoc", x => new { x.MaDonThuoc, x.MaThuoc });
                    table.ForeignKey(
                        name: "FK_ChiTietDonThuoc_DonThuoc_MaDonThuoc",
                        column: x => x.MaDonThuoc,
                        principalTable: "DonThuoc",
                        principalColumn: "MaDonThuoc",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonThuoc_Thuoc_MaThuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietKhaoSat",
                columns: table => new
                {
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    MaThuoc = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    DiemLikert = table.Column<int>(type: "int", nullable: false),
                    CoTacDungPhu = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    MoTaTrieuChung = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DiemTongThe = table.Column<int>(type: "int", nullable: false),
                    DiemHieuQua = table.Column<int>(type: "int", nullable: false),
                    DiemTacDungPhu = table.Column<int>(type: "int", nullable: false),
                    DiemTienLoi = table.Column<int>(type: "int", nullable: false),
                    NhanXet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChiTietK__32DBA082CEAC313E", x => new { x.MaPhieu, x.MaThuoc });
                    table.ForeignKey(
                        name: "FK_ChiTiet_Phieu",
                        column: x => x.MaPhieu,
                        principalTable: "PhieuKhaoSat",
                        principalColumn: "MaPhieu");
                    table.ForeignKey(
                        name: "FK_ChiTiet_Thuoc",
                        column: x => x.MaThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "MaThuoc");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonThuoc_MaThuoc",
                table: "ChiTietDonThuoc",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietKhaoSat_MaThuoc",
                table: "ChiTietKhaoSat",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_DonThuoc_MaBenhNhan",
                table: "DonThuoc",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuCanhBaoADR_MaThuoc",
                table: "PhieuCanhBaoADR",
                column: "MaThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuKhaoSat_MaBenhNhan",
                table: "PhieuKhaoSat",
                column: "MaBenhNhan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BacSi");

            migrationBuilder.DropTable(
                name: "ChiTietDonThuoc");

            migrationBuilder.DropTable(
                name: "ChiTietKhaoSat");

            migrationBuilder.DropTable(
                name: "PhieuCanhBaoADR");

            migrationBuilder.DropTable(
                name: "DonThuoc");

            migrationBuilder.DropTable(
                name: "PhieuKhaoSat");

            migrationBuilder.DropTable(
                name: "Thuoc");

            migrationBuilder.DropTable(
                name: "BenhNhan");
        }
    }
}
