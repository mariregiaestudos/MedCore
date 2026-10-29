
using MedCore.Dominio;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Infraestrutura.Contexto;

public class MedCoreContexto : DbContext
{
    public MedCoreContexto(
        DbContextOptions<MedCoreContexto> opcoes)
        : base(opcoes)
    {
    }

    public DbSet<Paciente> Pacientes { get; set; } = null!;
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Email)
                .HasMaxLength(256)
                .IsRequired();

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.SenhaHash)
                .IsRequired();

            entity.Property(u => u.Ativo)
                .IsRequired();
        });

        modelBuilder.Entity<Paciente>()
            .HasOne(p => p.Usuario)
            .WithMany()
            .HasForeignKey(p => p.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
