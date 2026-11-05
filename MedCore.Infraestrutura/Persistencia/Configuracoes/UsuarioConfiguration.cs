using MedCore.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedCore.Infraestrutura.Configuracoes;

public class UsuarioConfiguracao
    : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nome)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.CPF)
            .IsRequired()
            .HasMaxLength(14);

        builder.HasIndex(u => u.CPF)
            .IsUnique();

        builder.Property(u => u.DataNascimento)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(u => u.SenhaHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.Ativo)
            .IsRequired();
    }
}