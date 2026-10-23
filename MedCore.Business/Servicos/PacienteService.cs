using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using MedCore.Dominio;

using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Servicos;

public class PacienteService(IUnityOfWork _uow) : IPacienteService
{

    public async Task<PacienteResponse> Criar(PacienteRequest request)
    {
        var email = request.Email.Trim();

        if (await _uow.Pacientes.ExisteEmail(email))
        {
            throw new InvalidOperationException(
                "Já existe um paciente cadastrado com esse e-mail.");
        }

        var paciente = new Paciente(request.Nome, email);

        await _uow.Pacientes.Criar(paciente);
        await _uow.Confirmar();

        return MapearParaResponse(paciente);
    }

    public async Task<PacienteResponse> BuscarPorCodigo(Guid codigo)
    {
        var paciente = await _uow.Pacientes.BuscarPorCodigo(codigo);

        if (paciente is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        return MapearParaResponse(paciente);
    }

    public async Task<List<PacienteResponse>> Buscar(
        BuscarPacienteRequest filtros)
    {
        var pacientes = await _uow.Pacientes.Buscar(filtros);

        return pacientes.Select(MapearParaResponse).ToList();
    }

    public async Task<PacienteResponse> Atualizar(
        Guid codigo,
        PacienteRequest request)
    {
        var paciente = await _uow.Pacientes.BuscarPorCodigo(codigo);

        if (paciente is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        var email = request.Email.Trim();

        if (!string.Equals(
                paciente.Email,
                email,
                StringComparison.OrdinalIgnoreCase)
            && await _uow.Pacientes.ExisteEmail(email))
        {
            throw new InvalidOperationException(
                "Já existe outro paciente cadastrado com esse e-mail.");
        }

        paciente.AtualizarDados(request.Nome, email);

        await _uow.Pacientes.Atualizar(paciente);
        await _uow.Confirmar();

        return MapearParaResponse(paciente);
    }

    public async Task Desativar(Guid codigo)
    {
        var paciente = await _uow.Pacientes.BuscarPorCodigo(codigo);

        if (paciente is null)
            throw new KeyNotFoundException("Paciente não encontrado.");

        paciente.Desativar();

        await _uow.Pacientes.Atualizar(paciente);
        await _uow.Confirmar();
    }

    private static PacienteResponse MapearParaResponse(Paciente paciente)
    {
        return new PacienteResponse
        {
            Id = paciente.Id,
            Nome = paciente.Nome,
            Email = paciente.Email
        };
    }
}
