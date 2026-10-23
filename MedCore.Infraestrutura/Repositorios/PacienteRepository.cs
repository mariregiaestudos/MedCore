using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces;
using MedCore.Dominio;
using MedCore.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Infraestrutura.Repositorios;

public class PacienteRepository(MedCoreContexto _contexto) : IPacienteRepositorio
{
    public async Task Criar(Paciente paciente)
    {
        await _contexto.Pacientes.AddAsync(paciente);
    }

    public async Task<Paciente?> BuscarPorCodigo(Guid codigo)
    {
        return await _contexto.Pacientes
            .FirstOrDefaultAsync(p => p.Id == codigo);
    }

    public async Task<List<Paciente>> Buscar(
        BuscarPacienteRequest filtros)
    {
        var consulta = _contexto.Pacientes.AsQueryable();
        if (filtros.Ativo.HasValue)
        {
            consulta = consulta.Where(
                p => p.Ativo == filtros.Ativo.Value);
        }
        if (!string.IsNullOrWhiteSpace(filtros.Nome))
            consulta = consulta.Where(
                p => p.Nome.Contains(filtros.Nome));

        if (!string.IsNullOrWhiteSpace(filtros.Email))
            consulta = consulta.Where(
                p => p.Email == filtros.Email);


        return await consulta.ToListAsync();
    }

    public Task Atualizar(Paciente paciente)
    {
        _contexto.Pacientes.Update(paciente);
        return Task.CompletedTask;
    }

    public async Task<bool> ExisteEmail(string email)
    {
        return await _contexto.Pacientes
            .AnyAsync(p => p.Email == email);
    }
}
