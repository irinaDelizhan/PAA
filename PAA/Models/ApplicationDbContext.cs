using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PAA.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Participation> Participations { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<TransactionPaa> TransactionPaas { get; set; }

    public virtual DbSet<UserPaa> UserPaas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-D29N8OJ;Database=CompanyDB;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Participation>(entity =>
        {
            entity.HasKey(e => e.ParticipationId).HasName("PK__Particip__78B6F505A2CA16DE");

            entity.Property(e => e.ParticipationId).ValueGeneratedNever();

            entity.HasOne(d => d.Project).WithMany(p => p.Participations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Participa__proje__5070F446");

            entity.HasOne(d => d.User).WithMany(p => p.Participations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Participa__userI__4F7CD00D");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PK__Position__B07CF5AE6DE50ADB");

            entity.Property(e => e.PositionId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.ProjectId).HasName("PK__Project__11F14DA5A0163838");

            entity.Property(e => e.ProjectId).ValueGeneratedNever();

            entity.HasOne(d => d.Head).WithMany(p => p.Projects)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Project__headId__4CA06362");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("PK__Section__3F58FD52EF375BD5");

            entity.Property(e => e.SectionId).ValueGeneratedNever();
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.StateId).HasName("PK__State__A667B9E1F0862016");

            entity.Property(e => e.StateId).ValueGeneratedNever();

            entity.HasOne(d => d.Project).WithMany(p => p.States)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__State__projectId__534D60F1");

            entity.HasOne(d => d.User).WithMany(p => p.States)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__State__userId__5441852A");
        });

        modelBuilder.Entity<TransactionPaa>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__Transact__9B57CF726216B043");

            entity.Property(e => e.TransactionId).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.TransactionPaas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__userI__48CFD27E");
        });

        modelBuilder.Entity<UserPaa>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserPAA__CB9A1CFFB8273DFD");

            entity.Property(e => e.UserId).ValueGeneratedNever();

            entity.HasOne(d => d.Position).WithMany(p => p.UserPaas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPAA__positio__44FF419A");

            entity.HasOne(d => d.Section).WithMany(p => p.UserPaas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPAA__section__440B1D61");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
