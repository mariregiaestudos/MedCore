
using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces.IRepositorios;
using MedCore.Dominio;
using MedCore.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Infraestrutura.Repositorios;

public class PacienteRepository(
    MedCoreContexto _contexto) : IPacienteRepositorio
{
    public async Task Criar(Paciente paciente)
    {
        await _contexto.Pacientes.AddAsync(paciente);
    }

    public async Task<Paciente?> BuscarPorCodigo(Guid id)
    {
        return await _contexto.Pacientes
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Paciente?> BuscarPorUsuarioId(Guid usuarioId)
    {
        return await _contexto.Pacientes
            .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
    }

    public async Task<List<Paciente>> Buscar(
        BuscarPacienteRequest filtros)
    {
        var consulta = _contexto.Pacientes
            .AsNoTracking()
            .Include(p => p.Usuario)
            .AsQueryable();

        if (filtros.Ativo.HasValue)
        {
            consulta = consulta.Where(
                p => p.Ativo == filtros.Ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtros.Nome))
        {
            var nome = filtros.Nome.Trim();

            consulta = consulta.Where(
                p => p.Usuario.Nome.Contains(nome));
        }

        if (!string.IsNullOrWhiteSpace(filtros.Email))
        {
            var email = filtros.Email.Trim().ToLowerInvariant();

            consulta = consulta.Where(
                p => p.Usuario.Email == email);
        }

        return await consulta.ToListAsync();
    }

    public Task Atualizar(Paciente paciente)
    {
        _contexto.Pacientes.Update(paciente);

        return Task.CompletedTask;
    }
}
