using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace InfoClimaApi.Models
{
    public partial class InfoClimaContext : DbContext
    {

        public InfoClimaContext(DbContextOptions<InfoClimaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ciudades> Ciudades { get; set; }
        public virtual DbSet<Clima> Climas { get; set; }
        public virtual DbSet<Paise> Paises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Modern_Spanish_CI_AS");

            modelBuilder.Entity<Ciudades>(entity =>
            {
                entity.HasKey(e => e.IdCiudades);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.IdPaisesNavigation)
                    .WithMany(p => p.Ciudades)
                    .HasForeignKey(d => d.IdPaises)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ciudades_Paises");
            });

            modelBuilder.Entity<Clima>(entity =>
            {
                entity.HasKey(e => e.IdClima);

                entity.ToTable("Clima");

                entity.Property(e => e.Clima1)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("Clima");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.Termica)
                    .IsRequired()
                    .HasMaxLength(6);

                entity.HasOne(d => d.IdCiudadesNavigation)
                    .WithMany(p => p.Climas)
                    .HasForeignKey(d => d.IdCiudades)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Clima_Ciudades");
            });

            modelBuilder.Entity<Paise>(entity =>
            {
                entity.HasKey(e => e.IdPaises);

                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
