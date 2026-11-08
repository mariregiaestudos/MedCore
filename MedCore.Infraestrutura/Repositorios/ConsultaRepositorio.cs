using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces.IRepositorios;
using MedCore.Dominio;
using MedCore.Dominio.Enums;
using MedCore.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Infraestrutura.Repositorios;

public class ConsultaRepositorio(MedCoreContexto _context)
    : IConsultaRepositorio
{
    public async Task<bool> ExisteConflitoAsync(
        Guid medicoId,
        Guid pacienteId,
        DateTime dataHora,
        DateTime dataHoraFim,
        Guid? consultaIdIgnorada = null)
    {
        return await _context.Consultas.AnyAsync(c =>
            c.Id != consultaIdIgnorada &&
            c.Status != StatusConsulta.Cancelada &&
            (c.MedicoId == medicoId || c.PacienteId == pacienteId) &&
            dataHora < c.DataHoraFim &&
            dataHoraFim > c.DataHora);
    }

    public async Task Criar(Consulta consulta)
    {
        await _context.Consultas.AddAsync(consulta);
    }

    public async Task<Consulta?> BuscarPorCodigo(Guid codigo)
    {
        return await _context.Consultas
            .FirstOrDefaultAsync(c => c.Id == codigo);
    }

    public async Task<List<Consulta>> Buscar(BuscarConsultaRequest filtros)
    {
        var query = _context.Consultas.AsQueryable();

        if (filtros.PacienteId.HasValue)
        {
            query = query.Where(c => c.PacienteId == filtros.PacienteId.Value);
        }

        if (filtros.MedicoId.HasValue)
        {
            query = query.Where(c => c.MedicoId == filtros.MedicoId.Value);
        }

        if (filtros.DataInicio.HasValue)
        {
            query = query.Where(c => c.DataHora >= filtros.DataInicio.Value);
        }

        if (filtros.DataFim.HasValue)
        {
            query = query.Where(c => c.DataHora <= filtros.DataFim.Value);
        }

        if (filtros.Status.HasValue)
        {
            query = query.Where(c => c.Status == filtros.Status.Value);
        }

        return await query.ToListAsync();
    }

    public Task Atualizar(Consulta consulta)
    {
        _context.Consultas.Update(consulta);
        return Task.CompletedTask;
    }
}