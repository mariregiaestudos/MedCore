using MedCore.Dominio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Infraestrutura.Persistencia.Configuracoes;

public class ConsultaConfiguration : IEntityTypeConfiguration<Consulta>
{
    public void Configure(EntityTypeBuilder<Consulta> builder)
    {
        builder.ToTable("Consultas");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.DataHora)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(c => c.DuracaoMinutos)
            .IsRequired();

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasOne(c => c.Paciente)
            .WithMany()
            .HasForeignKey(c => c.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Medico)
            .WithMany()
            .HasForeignKey(c => c.MedicoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new
        {
            c.MedicoId,
            c.DataHora
        });

        builder.HasIndex(c => new
        {
            c.PacienteId,
            c.DataHora
        });
    }
}