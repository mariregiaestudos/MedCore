using MedCore.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedCore.Infraestrutura.Persistencia.Configuracoes;

public class PacienteConfiguracao
    : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("Pacientes");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.UsuarioId)
            .IsRequired();

        builder.HasIndex(p => p.UsuarioId)
            .IsUnique();

        builder.HasOne(p => p.Usuario)
            .WithOne(u => u.Paciente)
            .HasForeignKey<Paciente>(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(p => p.Ativo)
            .IsRequired();
    }
}