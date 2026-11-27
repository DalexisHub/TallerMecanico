using Microsoft.EntityFrameworkCore;
using TallerMecanico.Entidades;

namespace TallerMecanico.Datos;

public class TallerMecanicoContext : DbContext
{
    public TallerMecanicoContext(DbContextOptions<TallerMecanicoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Vehiculo> Vehiculos { get; set; }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Database.IsNpgsql() ? "public" : "dbo");
        modelBuilder.Entity<Usuario>(entity => { entity.ToTable("Usuarios"); entity.HasKey(x => x.Id); entity.Property(x => x.Id).ValueGeneratedOnAdd(); entity.HasIndex(x => x.Correo, "UX_Usuarios_Correo").IsUnique(); entity.Property(x => x.Correo).HasMaxLength(254).IsRequired(); entity.Property(x => x.ClaveHash).HasMaxLength(100).IsRequired(); entity.Property(x => x.Rol).HasMaxLength(30).IsRequired(); });
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.Documento, "UX_Clientes_Documento").IsUnique();

            entity.Property(e => e.Documento).HasMaxLength(20);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(20);
        });

        modelBuilder.Entity<Vehiculo>(entity =>
        {
            entity.ToTable("Vehiculos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasIndex(e => e.ClienteId, "IX_Vehiculos_ClienteId");

            entity.HasIndex(e => e.Placa, "UX_Vehiculos_Placa").IsUnique();

            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Modelo).HasMaxLength(50);
            entity.Property(e => e.Placa).HasMaxLength(15);

            entity.HasOne(d => d.Cliente).WithMany(p => p.Vehiculos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Vehiculos_Clientes");
        });
    }
}
