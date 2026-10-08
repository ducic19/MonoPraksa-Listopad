using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebApplication3.Model;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemId).HasName("member_pkey");

            entity.ToTable("member");

            entity.Property(e => e.MemId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("mem_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.SubsId).HasColumnName("subs_id");

            entity.HasOne(d => d.Subs).WithMany(p => p.Members)
                .HasForeignKey(d => d.SubsId)
                .HasConstraintName("fk_subs_id");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubsId).HasName("subscription_pkey");

            entity.ToTable("subscription");

            entity.Property(e => e.SubsId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("subs_id");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.Weekly).HasColumnName("weekly");

            entity.HasMany(d => d.Trainers).WithMany(p => p.Subs)
                .UsingEntity<Dictionary<string, object>>(
                    "TrainerSub",
                    r => r.HasOne<Trainer>().WithMany()
                        .HasForeignKey("TrainerId")
                        .HasConstraintName("fk_ts_t"),
                    l => l.HasOne<Subscription>().WithMany()
                        .HasForeignKey("SubsId")
                        .HasConstraintName("fk_ts_s"),
                    j =>
                    {
                        j.HasKey("SubsId", "TrainerId").HasName("trainer_subs_pkey");
                        j.ToTable("trainer_subs");
                        j.IndexerProperty<Guid>("SubsId").HasColumnName("subs_id");
                        j.IndexerProperty<Guid>("TrainerId").HasColumnName("trainer_id");
                    });
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.TrainerId).HasName("trainer_pkey");

            entity.ToTable("trainer");

            entity.Property(e => e.TrainerId)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("trainer_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
