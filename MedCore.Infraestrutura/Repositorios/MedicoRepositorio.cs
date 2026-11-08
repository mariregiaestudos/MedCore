
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
            .Include(m => m.Usuario)
            .FirstOrDefaultAsync(m => m.Id == codigo);
    }

    public async Task<Medico?> BuscarPorUsuarioId(Guid usuarioId)
    {
        return await _context.Medicos
            .Include(m => m.Usuario)
            .FirstOrDefaultAsync(m => m.UsuarioId == usuarioId);
    }

    public async Task<List<Medico>> Buscar(
        BuscarMedicoRequest filtros)
    {
        var query = _context.Medicos
            .AsNoTracking()
            .Include(m => m.Usuario)
            .AsQueryable();

        if (filtros.Ativo.HasValue)
        {
            query = query.Where(m => m.Ativo == filtros.Ativo.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtros.Nome))
        {
            var nome = filtros.Nome.Trim();

            query = query.Where(m =>
                m.Usuario.Nome.Contains(nome));
        }

        if (!string.IsNullOrWhiteSpace(filtros.CRM))
        {
            var crm = filtros.CRM.Trim();

            query = query.Where(m =>
                m.CRM == crm);
        }

        if (!string.IsNullOrWhiteSpace(filtros.UF))
        {
            var uf = filtros.UF.Trim().ToUpperInvariant();

            query = query.Where(m =>
                m.UF == uf);
        }

        if (!string.IsNullOrWhiteSpace(filtros.Especialidade))
        {
            var especialidade = filtros.Especialidade.Trim();

            query = query.Where(m =>
                m.Especialidade.Contains(especialidade));
        }

        return await query.ToListAsync();
    }

    public Task Atualizar(Medico medico)
    {
        _context.Medicos.Update(medico);

        return Task.CompletedTask;
    }

    public async Task<bool> ExisteCRM(string crm, string uf)
    {
        crm = crm.Trim();
        uf = uf.Trim().ToUpperInvariant();

        return await _context.Medicos.AnyAsync(m =>
            m.CRM == crm && m.UF == uf);
    }
}
