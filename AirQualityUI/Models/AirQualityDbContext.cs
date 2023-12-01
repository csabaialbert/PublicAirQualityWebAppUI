using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AirQualityUI.Models;

public partial class AirQualityDbContext : DbContext
{
    //A Microsoft Entity Framework Core által automatikusan generált adatbázist kezelő osztály.
    //Leírja az adatbázis adattábláit és kapcsolatait.
    public AirQualityDbContext()
    {
    }

    public AirQualityDbContext(DbContextOptions<AirQualityDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Module> Modules { get; set; }

    public virtual DbSet<SensorValue> SensorValues { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserModuleConnection> UserModuleConnections { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("airquali_admin");

        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Modules__3214EC072EC4C572");

            entity.ToTable("Modules", "dbo");

            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ModuleName)
                .HasMaxLength(25)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SensorValue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Id");

            entity.ToTable("SensorValues", "dbo");

            entity.Property(e => e.Humidity).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq135Aceton).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq135Alcohol).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq135Co)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq135CO");
            entity.Property(e => e.Mq135Co2)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq135CO2");
            entity.Property(e => e.Mq135Nh4)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq135NH4");
            entity.Property(e => e.Mq135Raw).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq135Tolueno).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq2Alcohol).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq2Ch4)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq2CH4");
            entity.Property(e => e.Mq2Co)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq2CO");
            entity.Property(e => e.Mq2H2).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq2Lpg)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq2LPG");
            entity.Property(e => e.Mq2Propane).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq2Raw).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq2Smoke).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq3Alcohol).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq3Benzine).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq3Ch4)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq3CH4");
            entity.Property(e => e.Mq3Co)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq3CO");
            entity.Property(e => e.Mq3Exane).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq3Lpg)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq3LPG");
            entity.Property(e => e.Mq3Raw).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq4Alcohol).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq4Ch4)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq4CH4");
            entity.Property(e => e.Mq4Co)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq4CO");
            entity.Property(e => e.Mq4H2).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq4Lpg)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq4LPG");
            entity.Property(e => e.Mq4Raw).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq4Smoke).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq6Alcohol).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq6Ch4)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq6CH4");
            entity.Property(e => e.Mq6Co)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq6CO");
            entity.Property(e => e.Mq6H2).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq6Lpg)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq6LPG");
            entity.Property(e => e.Mq6Raw).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq7Alcohol).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq7Ch4)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq7CH4");
            entity.Property(e => e.Mq7Co)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq7CO");
            entity.Property(e => e.Mq7H2).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq7Lpg)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq7LPG");
            entity.Property(e => e.Mq7Raw).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq8Alcohol).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq8Ch4)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq8CH4");
            entity.Property(e => e.Mq8Co)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq8CO");
            entity.Property(e => e.Mq8H2).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq8Lpg)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq8LPG");
            entity.Property(e => e.Mq8Raw).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Mq9Ch4)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq9CH4");
            entity.Property(e => e.Mq9Co)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq9CO");
            entity.Property(e => e.Mq9Lpg)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("Mq9LPG");
            entity.Property(e => e.Mq9Raw).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.ReadDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Temperature).HasColumnType("decimal(18, 4)");

            entity.HasOne(d => d.Module).WithMany(p => p.SensorValues)
                .HasForeignKey(d => d.ModuleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SensorValues_Modules");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07798ABE92");

            entity.ToTable("Users", "dbo");

            entity.Property(e => e.CreateDate)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserRole)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<UserModuleConnection>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserModu__3214EC0731B59BB8");

            entity.ToTable("UserModuleConnection", "dbo");

            entity.HasOne(d => d.Module).WithMany(p => p.UserModuleConnections)
                .HasForeignKey(d => d.ModuleId)
                .HasConstraintName("FK_UserModuleConnection_Modules");

            entity.HasOne(d => d.User).WithMany(p => p.UserModuleConnections)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_UserModuleConnection_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
