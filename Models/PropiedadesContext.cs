using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProyectoBasesII.Models
{
    public partial class PropiedadesContext : DbContext
    {
        public PropiedadesContext()
        {
        }

        public PropiedadesContext(DbContextOptions<PropiedadesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FotoPropiedad> FotoPropiedad { get; set; }
        public virtual DbSet<Pais> Pais { get; set; }
        public virtual DbSet<Propiedad> Propiedad { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=192.168.1.10\\SQLMASTER;Initial Catalog=Propiedades;Persist Security Info=True;User ID=propiedades;Password=propiedades");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FotoPropiedad>(entity =>
            {
                entity.HasKey(e => new { e.NumeroPlanoPropiedad, e.RutaFotoPropiedad });

                entity.ToTable("fotoPropiedad");

                entity.Property(e => e.NumeroPlanoPropiedad)
                    .HasColumnName("numeroPlanoPropiedad")
                    .HasColumnType("numeric(6, 0)");

                entity.Property(e => e.RutaFotoPropiedad)
                    .HasColumnName("rutaFotoPropiedad")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.NumeroPlanoPropiedadNavigation)
                    .WithMany(p => p.FotoPropiedad)
                    .HasForeignKey(d => d.NumeroPlanoPropiedad)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fotoPropiedad_fk_numeroPlanoPropiedad");
            });

            modelBuilder.Entity<Pais>(entity =>
            {
                entity.HasKey(e => e.IdPais);

                entity.ToTable("pais");

                entity.Property(e => e.IdPais)
                    .HasColumnName("idPais")
                    .HasColumnType("numeric(3, 0)");

                entity.Property(e => e.AreaKm)
                    .HasColumnName("areaKM")
                    .HasColumnType("numeric(12, 0)");

                entity.Property(e => e.NbrPais)
                    .IsRequired()
                    .HasColumnName("nbrPais")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.NivelRiesgo)
                    .IsRequired()
                    .HasColumnName("nivelRiesgo")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.PoblacionActual)
                    .HasColumnName("poblacionActual")
                    .HasColumnType("numeric(12, 0)");
            });

            modelBuilder.Entity<Propiedad>(entity =>
            {
                entity.HasKey(e => e.NumeroPlano);

                entity.ToTable("propiedad");

                entity.Property(e => e.NumeroPlano)
                    .HasColumnName("numeroPlano")
                    .HasColumnType("numeric(6, 0)");

                entity.Property(e => e.AnnoRegistro)
                    .HasColumnName("annoRegistro")
                    .HasColumnType("numeric(4, 0)");

                entity.Property(e => e.CodigoPais)
                    .HasColumnName("codigoPais")
                    .HasColumnType("numeric(3, 0)");

                entity.Property(e => e.CorreoElectronico)
                    .IsRequired()
                    .HasColumnName("correoElectronico")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.CostoDolares)
                    .HasColumnName("costoDolares")
                    .HasColumnType("numeric(10, 0)");

                entity.Property(e => e.FechaRegistro)
                    .HasColumnName("fechaRegistro")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.NbrContanto)
                    .IsRequired()
                    .HasColumnName("nbrContanto")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.Property(e => e.RutaImagenPlano)
                    .IsRequired()
                    .HasColumnName("rutaImagenPlano")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.RutaValoracion)
                    .IsRequired()
                    .HasColumnName("rutaValoracion")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.RutaVideo)
                    .HasColumnName("rutaVideo")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.TipoPropiedad)
                    .IsRequired()
                    .HasColumnName("tipoPropiedad")
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.HasOne(d => d.CodigoPaisNavigation)
                    .WithMany(p => p.Propiedad)
                    .HasForeignKey(d => d.CodigoPais)
                    .HasConstraintName("propiedad_fk_codigoPais");
            });
        }
    }
}
