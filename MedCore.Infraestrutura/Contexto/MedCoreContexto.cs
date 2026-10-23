using MedCore.Dominio;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Infraestrutura.Contexto;

public class MedCoreContexto : DbContext
{
    public MedCoreContexto(
    DbContextOptions<MedCoreContexto> opcoes)
    : base(opcoes)
    {
    }

    public DbSet<Paciente> Pacientes { get; set; }
}
