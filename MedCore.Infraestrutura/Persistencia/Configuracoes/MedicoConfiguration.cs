using MedCore.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedCore.Infraestrutura.Persistencia.Configuracoes;

public class MedicoConfiguracao
    : IEntityTypeConfiguration<Medico>
{
    public void Configure(EntityTypeBuilder<Medico> builder)
    {
        builder.ToTable("Medicos");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.UsuarioId)
            .IsRequired();

        builder.HasIndex(m => m.UsuarioId)
            .IsUnique();

        builder.HasOne(m => m.Usuario)
            .WithOne(u => u.Medico)
            .HasForeignKey<Medico>(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(m => m.CRM)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.UF)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(m => m.Especialidade)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(m => new { m.CRM, m.UF })
            .IsUnique();

        builder.Property(m => m.Ativo)
            .IsRequired();
    }
}