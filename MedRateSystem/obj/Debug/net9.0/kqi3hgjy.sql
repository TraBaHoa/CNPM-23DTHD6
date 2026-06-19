IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [BenhNhan] (
    [MaBenhNhan] varchar(20) NOT NULL,
    [HoTen] nvarchar(100) NOT NULL,
    [NgaySinh] date NOT NULL,
    [GioiTinh] nvarchar(10) NULL,
    [SoDienThoai] varchar(15) NULL,
    CONSTRAINT [PK__BenhNhan__22A8B330A7903BBE] PRIMARY KEY ([MaBenhNhan])
);

CREATE TABLE [Thuoc] (
    [MaThuoc] varchar(20) NOT NULL,
    [GiaTien] decimal(18,2) NULL,
    [PhanLoaiVen] nvarchar(50) NULL,
    [TenThuoc] nvarchar(100) NOT NULL,
    [NhaSanXuat] nvarchar(150) NULL,
    [DiemLikertTB] float NULL DEFAULT 5.0E0,
    [TyLeADR] float NULL DEFAULT 0.0E0,
    CONSTRAINT [PK__Thuoc__4BB1F620B5D55177] PRIMARY KEY ([MaThuoc])
);

CREATE TABLE [DonThuoc] (
    [MaDonThuoc] nvarchar(450) NOT NULL,
    [MaBenhNhan] varchar(20) NULL,
    [NgayKeDon] datetime2 NOT NULL,
    [BacSiKeDon] nvarchar(max) NULL,
    CONSTRAINT [PK_DonThuoc] PRIMARY KEY ([MaDonThuoc]),
    CONSTRAINT [FK_DonThuoc_BenhNhan] FOREIGN KEY ([MaBenhNhan]) REFERENCES [BenhNhan] ([MaBenhNhan])
);

CREATE TABLE [PhieuKhaoSat] (
    [MaPhieu] int NOT NULL IDENTITY,
    [MaBenhNhan] varchar(20) NOT NULL,
    [ThoiGianLamPhieu] datetime2 NULL,
    [GhiChuNhanXet] nvarchar(max) NULL,
    CONSTRAINT [PK_PhieuKhaoSat] PRIMARY KEY ([MaPhieu]),
    CONSTRAINT [FK_PhieuKhaoSat_BenhNhan] FOREIGN KEY ([MaBenhNhan]) REFERENCES [BenhNhan] ([MaBenhNhan]) ON DELETE CASCADE
);

CREATE TABLE [PhieuCanhBaoADR] (
    [MaCanhBao] int NOT NULL IDENTITY,
    [MaThuoc] varchar(20) NOT NULL,
    [NgayPhatHien] datetime NULL DEFAULT ((getdate())),
    [TyLeThucTe] float NOT NULL,
    [NoiDungCanhBao] nvarchar(500) NOT NULL,
    CONSTRAINT [PK__PhieuCan__73C23D934378EE10] PRIMARY KEY ([MaCanhBao]),
    CONSTRAINT [FK_CanhBao_Thuoc] FOREIGN KEY ([MaThuoc]) REFERENCES [Thuoc] ([MaThuoc])
);

CREATE TABLE [ChiTietDonThuoc] (
    [MaDonThuoc] nvarchar(450) NOT NULL,
    [MaThuoc] varchar(20) NOT NULL,
    [Id] int NOT NULL,
    [ThuocMaThuoc] varchar(20) NOT NULL,
    [SoLuong] int NULL,
    [CachDung] nvarchar(max) NULL,
    CONSTRAINT [PK_ChiTietDonThuoc] PRIMARY KEY ([MaDonThuoc], [MaThuoc]),
    CONSTRAINT [FK_ChiTietDonThuoc_DonThuoc_MaDonThuoc] FOREIGN KEY ([MaDonThuoc]) REFERENCES [DonThuoc] ([MaDonThuoc]) ON DELETE CASCADE,
    CONSTRAINT [FK_ChiTietDonThuoc_Thuoc_MaThuoc] FOREIGN KEY ([MaThuoc]) REFERENCES [Thuoc] ([MaThuoc]) ON DELETE CASCADE,
    CONSTRAINT [FK_ChiTietDonThuoc_Thuoc_ThuocMaThuoc] FOREIGN KEY ([ThuocMaThuoc]) REFERENCES [Thuoc] ([MaThuoc]) ON DELETE CASCADE
);

CREATE TABLE [ChiTietKhaoSat] (
    [MaPhieu] int NOT NULL,
    [MaThuoc] varchar(20) NOT NULL,
    [DiemLikert] int NOT NULL,
    [CoTacDungPhu] bit NULL DEFAULT CAST(0 AS bit),
    [MoTaTrieuChung] nvarchar(250) NULL,
    CONSTRAINT [PK__ChiTietK__32DBA082CEAC313E] PRIMARY KEY ([MaPhieu], [MaThuoc]),
    CONSTRAINT [FK_ChiTiet_Phieu] FOREIGN KEY ([MaPhieu]) REFERENCES [PhieuKhaoSat] ([MaPhieu]),
    CONSTRAINT [FK_ChiTiet_Thuoc] FOREIGN KEY ([MaThuoc]) REFERENCES [Thuoc] ([MaThuoc])
);

CREATE INDEX [IX_ChiTietDonThuoc_MaThuoc] ON [ChiTietDonThuoc] ([MaThuoc]);

CREATE INDEX [IX_ChiTietDonThuoc_ThuocMaThuoc] ON [ChiTietDonThuoc] ([ThuocMaThuoc]);

CREATE INDEX [IX_ChiTietKhaoSat_MaThuoc] ON [ChiTietKhaoSat] ([MaThuoc]);

CREATE INDEX [IX_DonThuoc_MaBenhNhan] ON [DonThuoc] ([MaBenhNhan]);

CREATE INDEX [IX_PhieuCanhBaoADR_MaThuoc] ON [PhieuCanhBaoADR] ([MaThuoc]);

CREATE INDEX [IX_PhieuKhaoSat_MaBenhNhan] ON [PhieuKhaoSat] ([MaBenhNhan]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260602104436_InitialSetup', N'9.0.5');

ALTER TABLE [PhieuCanhBaoADR] ADD [DaXuLy] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [ChiTietKhaoSat] ADD [DiemHieuQua] int NOT NULL DEFAULT 0;

ALTER TABLE [ChiTietKhaoSat] ADD [DiemTacDungPhu] int NOT NULL DEFAULT 0;

ALTER TABLE [ChiTietKhaoSat] ADD [DiemTienLoi] int NOT NULL DEFAULT 0;

ALTER TABLE [ChiTietKhaoSat] ADD [DiemTongThe] int NOT NULL DEFAULT 0;

ALTER TABLE [ChiTietKhaoSat] ADD [Id] int NOT NULL DEFAULT 0;

ALTER TABLE [ChiTietKhaoSat] ADD [NhanXet] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609141841_AddDaXuLyToPhieuCanhBao', N'9.0.5');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609141851_UpdateKhaoSat', N'9.0.5');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609150941_CapNhatChiTietKhaoSat', N'9.0.5');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609151408_CapNhatModelKhaoSat', N'9.0.5');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609151958_KhoiTaoLai', N'9.0.5');

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChiTietKhaoSat]') AND [c].[name] = N'DiemHieuQua');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [ChiTietKhaoSat] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [ChiTietKhaoSat] DROP COLUMN [DiemHieuQua];

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChiTietKhaoSat]') AND [c].[name] = N'DiemTacDungPhu');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ChiTietKhaoSat] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [ChiTietKhaoSat] DROP COLUMN [DiemTacDungPhu];

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChiTietKhaoSat]') AND [c].[name] = N'DiemTienLoi');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ChiTietKhaoSat] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [ChiTietKhaoSat] DROP COLUMN [DiemTienLoi];

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChiTietKhaoSat]') AND [c].[name] = N'DiemTongThe');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ChiTietKhaoSat] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [ChiTietKhaoSat] DROP COLUMN [DiemTongThe];

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ChiTietKhaoSat]') AND [c].[name] = N'Id');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [ChiTietKhaoSat] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [ChiTietKhaoSat] DROP COLUMN [Id];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609152549_ThemCotNhanXet', N'9.0.5');

ALTER TABLE [ChiTietKhaoSat] ADD [DiemHieuQua] int NOT NULL DEFAULT 0;

ALTER TABLE [ChiTietKhaoSat] ADD [DiemTacDungPhu] int NOT NULL DEFAULT 0;

ALTER TABLE [ChiTietKhaoSat] ADD [DiemTienLoi] int NOT NULL DEFAULT 0;

ALTER TABLE [ChiTietKhaoSat] ADD [DiemTongThe] int NOT NULL DEFAULT 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609153852_ThemTieuChiDanhGia', N'9.0.5');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609154101_CapNhatCauTrucKhaoSat', N'9.0.5');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609154702_ChiTietKhaoSat', N'9.0.5');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609163018_ThemCotDanhGia', N'9.0.5');

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609163403_UpdateKhaoSatGiaoDien', N'9.0.5');

COMMIT;
GO

