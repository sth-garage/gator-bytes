using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ResumeJobAnalysisTool.DAL.Models;

namespace ResumeJobAnalysisTool.DAL.Context;

public partial class HRContext : DbContext
{
    public HRContext()
    {
    }

    public HRContext(DbContextOptions<HRContext> options)
        : base(options)
    {
    }

    public virtual DbSet<DocumentUpload> DocumentUploads { get; set; }

    public virtual DbSet<JobPosting> JobPostings { get; set; }

    public virtual DbSet<JobSkill> JobSkills { get; set; }

    public virtual DbSet<MatchAnalysisResult> MatchAnalysisResults { get; set; }

    public virtual DbSet<Prompt> Prompts { get; set; }

    public virtual DbSet<PromptType> PromptTypes { get; set; }

    public virtual DbSet<Resume> Resumes { get; set; }

    public virtual DbSet<ResumeSkill> ResumeSkills { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=127.0.0.1;Initial Catalog=ResumeJobAnalysisTool;User Id=resumeJobAnalysisToolServiceLogin;Password=Testing777!!;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DocumentUpload>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Uploads");

            entity.Property(e => e.Base64Data).IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())", "DF_Uploads_CreatedOn")
                .HasColumnType("datetime");
            entity.Property(e => e.FileCreatedDate).HasColumnType("datetime");
            entity.Property(e => e.FileModifiedDate).HasColumnType("datetime");
            entity.Property(e => e.FileName)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("none", "DF_DocumentUploads_FileType");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_DocumentUploads_IsActive");
        });

        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.Property(e => e.CompanyName).IsUnicode(false);
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())", "DF_JobPostings_CreatedOn")
                .HasColumnType("datetime");
            entity.Property(e => e.Html)
                .IsUnicode(false)
                .HasColumnName("HTML");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_JobPostings_IsActive");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Position).IsUnicode(false);
            entity.Property(e => e.Summary).IsUnicode(false);

            entity.HasOne(d => d.DocumentUpload).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.DocumentUploadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JobPostings_DocumentUploads");
        });

        modelBuilder.Entity<JobSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_JobRequirement");

            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())", "DF_JobSkills_CreatedOn")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_JobSkills_IsActive");
            entity.Property(e => e.Justification).IsUnicode(false);

            entity.HasOne(d => d.JobPosting).WithMany(p => p.JobSkills)
                .HasForeignKey(d => d.JobPostingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JobSkills_JobPostings");

            entity.HasOne(d => d.Skill).WithMany(p => p.JobSkills)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_JobSkills_Skills");
        });

        modelBuilder.Entity<MatchAnalysisResult>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ChatAnalysisResults");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.GeneralMatchDetails).IsUnicode(false);
            entity.Property(e => e.GeneralMatchPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.GeneralMatchSummary).IsUnicode(false);
            entity.Property(e => e.Html)
                .IsUnicode(false)
                .HasColumnName("HTML");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_MatchAnalysisResults_IsActive");
            entity.Property(e => e.OverallMatchDetails).IsUnicode(false);
            entity.Property(e => e.OverallMatchPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.OverallMatchSummary).IsUnicode(false);
            entity.Property(e => e.SkillMatchDetails).IsUnicode(false);
            entity.Property(e => e.SkillMatchPercentage).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.SkillMatchSummary).IsUnicode(false);

            entity.HasOne(d => d.JobPosting).WithMany(p => p.MatchAnalysisResults)
                .HasForeignKey(d => d.JobPostingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MatchAnalysisResults_JobPostings");

            entity.HasOne(d => d.Resume).WithMany(p => p.MatchAnalysisResults)
                .HasForeignKey(d => d.ResumeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MatchAnalysisResults_Resumes");
        });

        modelBuilder.Entity<Prompt>(entity =>
        {
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())", "DF_Prompts_CreatedOn")
                .HasColumnType("datetime");
            entity.Property(e => e.PromptText).IsUnicode(false);
        });

        modelBuilder.Entity<PromptType>(entity =>
        {
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())", "DF_PromptTypes_CreatedOn")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_PromptTypes_IsActive");
            entity.Property(e => e.Type)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Resume>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Resume");

            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())", "DF_Resume_CreatedOn")
                .HasColumnType("datetime");
            entity.Property(e => e.Html)
                .IsUnicode(false)
                .HasColumnName("HTML");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Resumes_IsActive");
            entity.Property(e => e.Name).IsUnicode(false);
            entity.Property(e => e.Personality).IsUnicode(false);
            entity.Property(e => e.Summary).IsUnicode(false);

            entity.HasOne(d => d.DocumentUpload).WithMany(p => p.Resumes)
                .HasForeignKey(d => d.DocumentUploadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Resumes_DocumentUploads");
        });

        modelBuilder.Entity<ResumeSkill>(entity =>
        {
            entity.Property(e => e.CreatedOn)
                .HasDefaultValueSql("(getdate())", "DF_ResumeSkills_CreatedOn")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_ResumeSkills_IsActive");
            entity.Property(e => e.Justification).IsUnicode(false);

            entity.HasOne(d => d.Resume).WithMany(p => p.ResumeSkills)
                .HasForeignKey(d => d.ResumeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResumeSkills_Resumes");

            entity.HasOne(d => d.Skill).WithMany(p => p.ResumeSkills)
                .HasForeignKey(d => d.SkillId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResumeSkills_Skills");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Skills_IsActive");
            entity.Property(e => e.SkillName)
                .HasMaxLength(500)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
