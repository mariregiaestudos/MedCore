
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

    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Medico> Medicos => Set<Medico>();
    public DbSet<Consulta> Consultas => Set<Consulta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MedCoreContexto).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
