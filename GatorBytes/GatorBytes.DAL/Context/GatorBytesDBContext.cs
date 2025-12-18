using System;
using System.Collections.Generic;
using GatorBytes.DAL.EFModels;
using Microsoft.EntityFrameworkCore;

namespace GatorBytes.DAL.Context;

public partial class GatorBytesDBContext : DbContext
{
    public GatorBytesDBContext()
    {
    }

    public GatorBytesDBContext(DbContextOptions<GatorBytesDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DocumentUpload> DocumentUploads { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=GatorBytes;User Id=gatorBytesServiceLogin;Password=Testing777!!;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentUpload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Uploads");

            entity.Property(e => e.Base64Data).IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())", "DF_Uploads_CreatedOn")
                .HasColumnType("datetime");
            entity.Property(e => e.FileName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_DocumentUploads_IsActive");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
