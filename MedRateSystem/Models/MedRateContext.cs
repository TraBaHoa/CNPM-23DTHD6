using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MedRateSystem.Models;

public partial class MedRateContext : DbContext
{
    public MedRateContext()
    {
    }

    public MedRateContext(DbContextOptions<MedRateContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DonThuoc> DonThuocs { get; set; }
    public virtual DbSet<ChiTietDonThuoc> ChiTietDonThuocs { get; set; }
    public virtual DbSet<BenhNhan> BenhNhans { get; set; }

    public virtual DbSet<ChiTietKhaoSat> ChiTietKhaoSats { get; set; }

    public virtual DbSet<PhieuCanhBaoAdr> PhieuCanhBaoAdrs { get; set; }

    public virtual DbSet<PhieuKhaoSat> PhieuKhaoSats { get; set; }

    public virtual DbSet<Thuoc> Thuocs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:MedRateDbConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DonThuoc>().ToTable("DonThuoc");
        modelBuilder.Entity<ChiTietDonThuoc>().ToTable("ChiTietDonThuoc");

        modelBuilder.Entity<DonThuoc>().HasKey(d => d.MaDonThuoc);
        modelBuilder.Entity<ChiTietDonThuoc>().HasKey(ct => new { ct.MaDonThuoc, ct.MaThuoc });

        modelBuilder.Entity<BenhNhan>(entity =>
        {
            entity.HasKey(e => e.MaBenhNhan).HasName("PK__BenhNhan__22A8B330A7903BBE");

            entity.ToTable("BenhNhan");

            entity.Property(e => e.MaBenhNhan)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ChiTietKhaoSat>(entity =>
        {
            entity.HasKey(e => new { e.MaPhieu, e.MaThuoc }).HasName("PK__ChiTietK__32DBA082CEAC313E");

            entity.ToTable("ChiTietKhaoSat");

            entity.Property(e => e.MaThuoc)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CoTacDungPhu).HasDefaultValue(false);
            entity.Property(e => e.MoTaTrieuChung).HasMaxLength(250);

            entity.HasOne(d => d.MaPhieuNavigation).WithMany(p => p.ChiTietKhaoSats)
                .HasForeignKey(d => d.MaPhieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTiet_Phieu");

            entity.HasOne(d => d.MaThuocNavigation).WithMany(p => p.ChiTietKhaoSats)
                .HasForeignKey(d => d.MaThuoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ChiTiet_Thuoc");
        });

        modelBuilder.Entity<PhieuCanhBaoAdr>(entity =>
        {
            entity.HasKey(e => e.MaCanhBao).HasName("PK__PhieuCan__73C23D934378EE10");

            entity.ToTable("PhieuCanhBaoADR");

            entity.Property(e => e.MaThuoc)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.NgayPhatHien)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDungCanhBao).HasMaxLength(500);

            entity.HasOne(d => d.MaThuocNavigation).WithMany(p => p.PhieuCanhBaoAdrs)
                .HasForeignKey(d => d.MaThuoc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CanhBao_Thuoc");
        });

        modelBuilder.Entity<DonThuoc>(entity =>
        {
            entity.ToTable("DonThuoc");
            entity.HasKey(d => d.MaDonThuoc);

            // Cấu hình rõ ràng khóa ngoại để tránh tự sinh cột rác
            entity.HasOne(d => d.MaBenhNhanNavigation)
                  .WithMany() // Nếu BenhNhan không có Collection DonThuocs, hãy để .WithMany()
                  .HasForeignKey(d => d.MaBenhNhan)
                  .HasConstraintName("FK_DonThuoc_BenhNhan");
        });

        modelBuilder.Entity<PhieuKhaoSat>(entity =>
        {
            entity.ToTable("PhieuKhaoSat");
            entity.HasKey(e => e.MaPhieu);

            entity.HasOne(d => d.MaBenhNhanNavigation)
                  .WithMany(p => p.PhieuKhaoSats)
                  .HasForeignKey(d => d.MaBenhNhan)
                  .HasConstraintName("FK_PhieuKhaoSat_BenhNhan");
        });

        modelBuilder.Entity<Thuoc>(entity =>
        {
            entity.HasKey(e => e.MaThuoc).HasName("PK__Thuoc__4BB1F620B5D55177");

            entity.ToTable("Thuoc");

            entity.Property(e => e.MaThuoc)
                .HasMaxLength(20)
                .IsUnicode(false);

            // THÊM ĐOẠN NÀY ĐỂ TRÁNH LỖI CỘT GIÁ TIỀN
            entity.Property(e => e.GiaTien).HasColumnName("GiaTien").HasColumnType("decimal(18,2)");

            // Đảm bảo PhanLoaiVen được ánh xạ
            entity.Property(e => e.PhanLoaiVen).HasColumnName("PhanLoaiVen").HasMaxLength(50);
            entity.Property(e => e.DiemLikertTb)
                .HasDefaultValue(5.0)
                .HasColumnName("DiemLikertTB");
            entity.Property(e => e.NhaSanXuat).HasMaxLength(150);
            entity.Property(e => e.TenThuoc).HasMaxLength(100);
            entity.Property(e => e.TyLeAdr)
                .HasDefaultValue(0.0)
                .HasColumnName("TyLeADR");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
