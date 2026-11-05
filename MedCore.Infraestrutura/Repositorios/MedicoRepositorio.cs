using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces.IRepositorios;
using MedCore.Dominio;
using MedCore.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Infraestrutura.Repositorios;

public class MedicoRepositorio(MedCoreContexto _context)
    : IMedicoRepositorio
{
    public async Task Criar(Medico medico)
    {
        await _context.Medicos.AddAsync(medico);
    }

    public async Task<Medico?> BuscarPorCodigo(Guid codigo)
    {
        return await _context.Medicos
            .FirstOrDefaultAsync(m => m.Id == codigo);
    }

    public async Task<List<Medico>> Buscar(
        BuscarMedicoRequest filtros)
    {
        var query = _context.Medicos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtros.Nome))
            query = query.Where(m => m.Nome.Contains(filtros.Nome));

        if (!string.IsNullOrWhiteSpace(filtros.CRM))
            query = query.Where(m => m.CRM == filtros.CRM);

        if (!string.IsNullOrWhiteSpace(filtros.UF))
            query = query.Where(m => m.UF == filtros.UF);

        if (!string.IsNullOrWhiteSpace(filtros.Especialidade))
            query = query.Where(m =>
                m.Especialidade.Contains(filtros.Especialidade));

        return await query.ToListAsync();
    }

    public Task Atualizar(Medico medico)
    {
        _context.Medicos.Update(medico);
        return Task.CompletedTask;
    }

    public async Task<bool> ExisteCRM(string crm, string uf)
    {
        return await _context.Medicos.AnyAsync(m =>
            m.CRM == crm && m.UF == uf);
    }
}