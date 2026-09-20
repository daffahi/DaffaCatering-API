using System;
using System.Collections.Generic;
using DaffaCatering.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DaffaCatering.API.Data;

public partial class DaffaCateringContext : DbContext
{
    public DaffaCateringContext()
    {
    }

    public DaffaCateringContext(DbContextOptions<DaffaCateringContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DetailBahanBaku> DetailBahanBakus { get; set; }

    public virtual DbSet<DetailPembelian> DetailPembelians { get; set; }

    public virtual DbSet<DetailPenerimaan> DetailPenerimaans { get; set; }

    public virtual DbSet<DetailPenggunaan> DetailPenggunaans { get; set; }

    public virtual DbSet<DetailPenjualan> DetailPenjualans { get; set; }

    public virtual DbSet<DetailPenyesuaian> DetailPenyesuaians { get; set; }

    public virtual DbSet<DetailPesanan> DetailPesanans { get; set; }

    public virtual DbSet<DetailResep> DetailReseps { get; set; }

    public virtual DbSet<HeaderBahanBaku> HeaderBahanBakus { get; set; }

    public virtual DbSet<HeaderPembelian> HeaderPembelians { get; set; }

    public virtual DbSet<HeaderPenerimaan> HeaderPenerimaans { get; set; }

    public virtual DbSet<HeaderPenggunaan> HeaderPenggunaans { get; set; }

    public virtual DbSet<HeaderPenjualan> HeaderPenjualans { get; set; }

    public virtual DbSet<HeaderPenyesuaian> HeaderPenyesuaians { get; set; }

    public virtual DbSet<HeaderPesanan> HeaderPesanans { get; set; }

    public virtual DbSet<HeaderResep> HeaderReseps { get; set; }

    public virtual DbSet<KonversiSatuan> KonversiSatuans { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Pelanggan> Pelanggans { get; set; }

    public virtual DbSet<Pemasok> Pemasoks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SatuanBahanBaku> SatuanBahanBakus { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DetailBahanBaku>(entity =>
        {
            entity.HasKey(e => new { e.IdBahanBaku, e.TglKadaluwarsa }).HasName("PK_DetailBahanBaku_1");

            entity.ToTable("DetailBahanBaku", "BahanBaku");

            entity.Property(e => e.IdBahanBaku)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idBahanBaku");
            entity.Property(e => e.TglKadaluwarsa).HasColumnName("tglKadaluwarsa");
            entity.Property(e => e.IdSatuan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuan");
            entity.Property(e => e.SisaStok)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("sisaStok");
            entity.Property(e => e.StokAwal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("stokAwal");

            entity.HasOne(d => d.IdBahanBakuNavigation).WithMany(p => p.DetailBahanBakus)
                .HasForeignKey(d => d.IdBahanBaku)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailBahanBaku_HeaderBahanBaku");

            entity.HasOne(d => d.IdSatuanNavigation).WithMany(p => p.DetailBahanBakus)
                .HasForeignKey(d => d.IdSatuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailBahanBaku_Satuan");
        });

        modelBuilder.Entity<DetailPembelian>(entity =>
        {
            entity.HasKey(e => new { e.IdPembelian, e.IdBahanBaku });

            entity.ToTable("DetailPembelian", "Pembelian");

            entity.Property(e => e.IdPembelian)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPembelian");
            entity.Property(e => e.IdBahanBaku)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idBahanBaku");
            entity.Property(e => e.Harga)
                .HasColumnType("money")
                .HasColumnName("harga");
            entity.Property(e => e.IdSatuan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuan");
            entity.Property(e => e.Jumlah)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("jumlah");

            entity.HasOne(d => d.IdBahanBakuNavigation).WithMany(p => p.DetailPembelians)
                .HasForeignKey(d => d.IdBahanBaku)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPembelian_HeaderBahanBaku");

            entity.HasOne(d => d.IdPembelianNavigation).WithMany(p => p.DetailPembelians)
                .HasForeignKey(d => d.IdPembelian)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPembelian_HeaderPembelian");

            entity.HasOne(d => d.IdSatuanNavigation).WithMany(p => p.DetailPembelians)
                .HasForeignKey(d => d.IdSatuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPembelian_Satuan");
        });

        modelBuilder.Entity<DetailPenerimaan>(entity =>
        {
            entity.HasKey(e => new { e.IdPenerimaan, e.IdBahanBaku });

            entity.ToTable("DetailPenerimaan", "Pembelian");

            entity.Property(e => e.IdPenerimaan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPenerimaan");
            entity.Property(e => e.IdBahanBaku)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idBahanBaku");
            entity.Property(e => e.IdSatuan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuan");
            entity.Property(e => e.Jumlah)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("jumlah");
            entity.Property(e => e.StatusBahanBaku)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("statusBahanBaku");
            entity.Property(e => e.TglKadaluwarsa).HasColumnName("tglKadaluwarsa");

            entity.HasOne(d => d.IdBahanBakuNavigation).WithMany(p => p.DetailPenerimaans)
                .HasForeignKey(d => d.IdBahanBaku)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenerimaan_HeaderBahanBaku");

            entity.HasOne(d => d.IdPenerimaanNavigation).WithMany(p => p.DetailPenerimaans)
                .HasForeignKey(d => d.IdPenerimaan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenerimaan_HeaderPenerimaan");

            entity.HasOne(d => d.IdSatuanNavigation).WithMany(p => p.DetailPenerimaans)
                .HasForeignKey(d => d.IdSatuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenerimaan_Satuan");
        });

        modelBuilder.Entity<DetailPenggunaan>(entity =>
        {
            entity.HasKey(e => new { e.IdPenggunaan, e.IdBahanBaku, e.TglKadaluwarsa }).HasName("PK_DetailPenggunaan_1");

            entity.ToTable("DetailPenggunaan", "BahanBaku");

            entity.Property(e => e.IdPenggunaan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPenggunaan");
            entity.Property(e => e.IdBahanBaku)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idBahanBaku");
            entity.Property(e => e.TglKadaluwarsa).HasColumnName("tglKadaluwarsa");
            entity.Property(e => e.IdSatuan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuan");
            entity.Property(e => e.JumlahPenggunaan)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("jumlahPenggunaan");

            entity.HasOne(d => d.IdBahanBakuNavigation).WithMany(p => p.DetailPenggunaans)
                .HasForeignKey(d => d.IdBahanBaku)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenggunaan_HeaderBahanBaku");

            entity.HasOne(d => d.IdPenggunaanNavigation).WithMany(p => p.DetailPenggunaans)
                .HasForeignKey(d => d.IdPenggunaan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenggunaan_HeaderPenggunaan");

            entity.HasOne(d => d.IdSatuanNavigation).WithMany(p => p.DetailPenggunaans)
                .HasForeignKey(d => d.IdSatuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenggunaan_Satuan");
        });

        modelBuilder.Entity<DetailPenjualan>(entity =>
        {
            entity.HasKey(e => new { e.IdPenjualan, e.IdMenu }).HasName("PK_DetailPenjualan_1");

            entity.ToTable("DetailPenjualan", "Penjualan");

            entity.Property(e => e.IdPenjualan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPenjualan");
            entity.Property(e => e.IdMenu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idMenu");
            entity.Property(e => e.Harga)
                .HasColumnType("money")
                .HasColumnName("harga");
            entity.Property(e => e.IdSatuan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuan");
            entity.Property(e => e.Jumlah).HasColumnName("jumlah");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.DetailPenjualans)
                .HasForeignKey(d => d.IdMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenjualan_Menu");

            entity.HasOne(d => d.IdPenjualanNavigation).WithMany(p => p.DetailPenjualans)
                .HasForeignKey(d => d.IdPenjualan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenjualan_HeaderPenjualan");

            entity.HasOne(d => d.IdSatuanNavigation).WithMany(p => p.DetailPenjualans)
                .HasForeignKey(d => d.IdSatuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenjualan_Satuan");
        });

        modelBuilder.Entity<DetailPenyesuaian>(entity =>
        {
            entity.HasKey(e => new { e.IdPenyesuaian, e.IdBahanBaku, e.TglKadaluwarsa }).HasName("PK_DetailPenyesuaian_1");

            entity.ToTable("DetailPenyesuaian", "BahanBaku");

            entity.Property(e => e.IdPenyesuaian)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPenyesuaian");
            entity.Property(e => e.IdBahanBaku)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idBahanBaku");
            entity.Property(e => e.TglKadaluwarsa).HasColumnName("tglKadaluwarsa");
            entity.Property(e => e.IdSatuan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuan");
            entity.Property(e => e.Keterangan)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("keterangan");
            entity.Property(e => e.SelisihStok)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("selisihStok");
            entity.Property(e => e.StokFisik)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("stokFisik");

            entity.HasOne(d => d.IdBahanBakuNavigation).WithMany(p => p.DetailPenyesuaians)
                .HasForeignKey(d => d.IdBahanBaku)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenyesuaian_HeaderBahanBaku");

            entity.HasOne(d => d.IdPenyesuaianNavigation).WithMany(p => p.DetailPenyesuaians)
                .HasForeignKey(d => d.IdPenyesuaian)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenyesuaian_HeaderPenyesuaian");

            entity.HasOne(d => d.IdSatuanNavigation).WithMany(p => p.DetailPenyesuaians)
                .HasForeignKey(d => d.IdSatuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPenyesuaian_Satuan");
        });

        modelBuilder.Entity<DetailPesanan>(entity =>
        {
            entity.HasKey(e => new { e.IdPesanan, e.IdMenu });

            entity.ToTable("DetailPesanan", "Penjualan");

            entity.Property(e => e.IdPesanan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPesanan");
            entity.Property(e => e.IdMenu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idMenu");
            entity.Property(e => e.Harga)
                .HasColumnType("money")
                .HasColumnName("harga");
            entity.Property(e => e.IdSatuan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuan");
            entity.Property(e => e.Jumlah).HasColumnName("jumlah");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.DetailPesanans)
                .HasForeignKey(d => d.IdMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPesanan_Menu");

            entity.HasOne(d => d.IdPesananNavigation).WithMany(p => p.DetailPesanans)
                .HasForeignKey(d => d.IdPesanan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPesanan_HeaderPesanan");

            entity.HasOne(d => d.IdSatuanNavigation).WithMany(p => p.DetailPesanans)
                .HasForeignKey(d => d.IdSatuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailPesanan_Satuan");
        });

        modelBuilder.Entity<DetailResep>(entity =>
        {
            entity.HasKey(e => new { e.IdResep, e.IdBahanBaku }).HasName("PK_DetailResep_1");

            entity.ToTable("DetailResep", "Resep");

            entity.Property(e => e.IdResep)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idResep");
            entity.Property(e => e.IdBahanBaku)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idBahanBaku");
            entity.Property(e => e.IdSatuan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuan");
            entity.Property(e => e.Jumlah)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("jumlah");

            entity.HasOne(d => d.IdBahanBakuNavigation).WithMany(p => p.DetailReseps)
                .HasForeignKey(d => d.IdBahanBaku)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailResep_HeaderBahanBaku");

            entity.HasOne(d => d.IdResepNavigation).WithMany(p => p.DetailReseps)
                .HasForeignKey(d => d.IdResep)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailResep_HeaderResep");

            entity.HasOne(d => d.IdSatuanNavigation).WithMany(p => p.DetailReseps)
                .HasForeignKey(d => d.IdSatuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetailResep_Satuan");
        });

        modelBuilder.Entity<HeaderBahanBaku>(entity =>
        {
            entity.HasKey(e => e.IdBahanBaku);

            entity.ToTable("HeaderBahanBaku", "BahanBaku");

            entity.Property(e => e.IdBahanBaku)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idBahanBaku");
            entity.Property(e => e.Jenis).HasColumnName("jenis");
            entity.Property(e => e.NamaBahanBaku)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("namaBahanBaku");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<HeaderPembelian>(entity =>
        {
            entity.HasKey(e => e.IdPembelian);

            entity.ToTable("HeaderPembelian", "Pembelian");

            entity.Property(e => e.IdPembelian)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPembelian");
            entity.Property(e => e.IdPemasok)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPemasok");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TglPembelian).HasColumnName("tglPembelian");
            entity.Property(e => e.Total)
                .HasColumnType("money")
                .HasColumnName("total");

            entity.HasOne(d => d.IdPemasokNavigation).WithMany(p => p.HeaderPembelians)
                .HasForeignKey(d => d.IdPemasok)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HeaderPembelian_Pemasok");
        });

        modelBuilder.Entity<HeaderPenerimaan>(entity =>
        {
            entity.HasKey(e => e.IdPenerimaan);

            entity.ToTable("HeaderPenerimaan", "Pembelian");

            entity.Property(e => e.IdPenerimaan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPenerimaan");
            entity.Property(e => e.IdPemasok)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPemasok");
            entity.Property(e => e.IdPembelian)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPembelian");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TglPenerimaan).HasColumnName("tglPenerimaan");

            entity.HasOne(d => d.IdPemasokNavigation).WithMany(p => p.HeaderPenerimaans)
                .HasForeignKey(d => d.IdPemasok)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HeaderPenerimaan_Pemasok");

            entity.HasOne(d => d.IdPembelianNavigation).WithMany(p => p.HeaderPenerimaans)
                .HasForeignKey(d => d.IdPembelian)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HeaderPenerimaan_HeaderPembelian");
        });

        modelBuilder.Entity<HeaderPenggunaan>(entity =>
        {
            entity.HasKey(e => e.IdPenggunaan);

            entity.ToTable("HeaderPenggunaan", "BahanBaku");

            entity.Property(e => e.IdPenggunaan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPenggunaan");
            entity.Property(e => e.TglPenggunaan).HasColumnName("tglPenggunaan");
        });

        modelBuilder.Entity<HeaderPenjualan>(entity =>
        {
            entity.HasKey(e => e.IdPenjualan);

            entity.ToTable("HeaderPenjualan", "Penjualan");

            entity.Property(e => e.IdPenjualan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPenjualan");
            entity.Property(e => e.IdPelanggan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPelanggan");
            entity.Property(e => e.IdPesanan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPesanan");
            entity.Property(e => e.MetodePembayaran)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("metodePembayaran");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TglPenjualan).HasColumnName("tglPenjualan");
            entity.Property(e => e.Total)
                .HasColumnType("money")
                .HasColumnName("total");
            entity.Property(e => e.UangMuka)
                .HasColumnType("money")
                .HasColumnName("uangMuka");

            entity.HasOne(d => d.IdPelangganNavigation).WithMany(p => p.HeaderPenjualans)
                .HasForeignKey(d => d.IdPelanggan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HeaderPenjualan_Pelanggan");

            entity.HasOne(d => d.IdPesananNavigation).WithMany(p => p.HeaderPenjualans)
                .HasForeignKey(d => d.IdPesanan)
                .HasConstraintName("FK_HeaderPenjualan_HeaderPesanan");
        });

        modelBuilder.Entity<HeaderPenyesuaian>(entity =>
        {
            entity.HasKey(e => e.IdPenyesuaian);

            entity.ToTable("HeaderPenyesuaian", "BahanBaku");

            entity.Property(e => e.IdPenyesuaian)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPenyesuaian");
            entity.Property(e => e.TglPenyesuaian).HasColumnName("tglPenyesuaian");
        });

        modelBuilder.Entity<HeaderPesanan>(entity =>
        {
            entity.HasKey(e => e.IdPesanan);

            entity.ToTable("HeaderPesanan", "Penjualan");

            entity.Property(e => e.IdPesanan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPesanan");
            entity.Property(e => e.IdPelanggan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPelanggan");
            entity.Property(e => e.Keterangan)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("keterangan");
            entity.Property(e => e.MetodePembayaran)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("metodePembayaran");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TglPengambilan).HasColumnName("tglPengambilan");
            entity.Property(e => e.TglPesanan).HasColumnName("tglPesanan");
            entity.Property(e => e.Total)
                .HasColumnType("money")
                .HasColumnName("total");
            entity.Property(e => e.UangMuka)
                .HasColumnType("money")
                .HasColumnName("uangMuka");

            entity.HasOne(d => d.IdPelangganNavigation).WithMany(p => p.HeaderPesanans)
                .HasForeignKey(d => d.IdPelanggan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HeaderPesanan_Pelanggan");
        });

        modelBuilder.Entity<HeaderResep>(entity =>
        {
            entity.HasKey(e => e.IdResep);

            entity.ToTable("HeaderResep", "Resep");

            entity.Property(e => e.IdResep)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idResep");
            entity.Property(e => e.IdMenu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idMenu");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.IdMenuNavigation).WithMany(p => p.HeaderReseps)
                .HasForeignKey(d => d.IdMenu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HeaderResep_Menu");
        });

        modelBuilder.Entity<KonversiSatuan>(entity =>
        {
            entity.HasKey(e => e.IdKonversi);

            entity.ToTable("KonversiSatuan", "Satuan");

            entity.Property(e => e.IdKonversi)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idKonversi");
            entity.Property(e => e.IdSatuanBesar)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuanBesar");
            entity.Property(e => e.IdSatuanKecil)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuanKecil");
            entity.Property(e => e.NilaiKonversi)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("nilaiKonversi");

            entity.HasOne(d => d.IdSatuanBesarNavigation).WithMany(p => p.KonversiSatuanIdSatuanBesarNavigations)
                .HasForeignKey(d => d.IdSatuanBesar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KonversiSatuan_SatuanBesar");

            entity.HasOne(d => d.IdSatuanKecilNavigation).WithMany(p => p.KonversiSatuanIdSatuanKecilNavigations)
                .HasForeignKey(d => d.IdSatuanKecil)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KonversiSatuan_SatuanKecil");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.IdMenu);

            entity.ToTable("Menu", "Resep");

            entity.Property(e => e.IdMenu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idMenu");
            entity.Property(e => e.Harga)
                .HasColumnType("money")
                .HasColumnName("harga");
            entity.Property(e => e.IdSatuan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuan");
            entity.Property(e => e.Jumlah).HasColumnName("jumlah");
            entity.Property(e => e.NamaMenu)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("namaMenu");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.IdSatuanNavigation).WithMany(p => p.Menus)
                .HasForeignKey(d => d.IdSatuan)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Menu_Satuan");
        });

        modelBuilder.Entity<Pelanggan>(entity =>
        {
            entity.HasKey(e => e.IdPelanggan);

            entity.ToTable("Pelanggan", "Pelanggan");

            entity.Property(e => e.IdPelanggan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPelanggan");
            entity.Property(e => e.Alamat)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("alamat");
            entity.Property(e => e.NamaPelanggan)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("namaPelanggan");
            entity.Property(e => e.NoKontak)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("noKontak");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TglBergabung).HasColumnName("tglBergabung");
        });

        modelBuilder.Entity<Pemasok>(entity =>
        {
            entity.HasKey(e => e.IdPemasok);

            entity.ToTable("Pemasok", "Pemasok");

            entity.Property(e => e.IdPemasok)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idPemasok");
            entity.Property(e => e.Alamat)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("alamat");
            entity.Property(e => e.NamaPemasok)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("namaPemasok");
            entity.Property(e => e.NoKontak)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("noKontak");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TglBergabung).HasColumnName("tglBergabung");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRole);

            entity.ToTable("Role", "User");

            entity.Property(e => e.IdRole)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idRole");
            entity.Property(e => e.NamaRole)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("namaRole");
        });

        modelBuilder.Entity<SatuanBahanBaku>(entity =>
        {
            entity.HasKey(e => e.IdSatuan).HasName("PK_Satuan");

            entity.ToTable("SatuanBahanBaku", "Satuan");

            entity.Property(e => e.IdSatuan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idSatuan");
            entity.Property(e => e.NamaSatuan)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("namaSatuan");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("User", "User");

            entity.Property(e => e.IdUser)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idUser");
            entity.Property(e => e.IdRole)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("idRole");
            entity.Property(e => e.NamaUser)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("namaUser");
            entity.Property(e => e.Password)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
