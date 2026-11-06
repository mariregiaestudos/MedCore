using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using MedCore.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Servicos;

public class ConsultaService(
    IUnityOfWork _uow,
    IColetorErros _coletorErros) : IConsultaService
{
    public async Task<ConsultaResponse?> Agendar(ConsultaRequest request)
    {
        var paciente = await _uow.Pacientes.BuscarPorCodigo(
            request.PacienteId);
        if (!paciente.Ativo)
        {
            _coletorErros.AdicionarErro(
                "Não é possível agendar uma consulta para um paciente inativo.");

            return null;
        }
        if (paciente is null)
        {
            _coletorErros.AdicionarErro("Paciente não encontrado.");
            return null;
        }

        var medico = await _uow.Medicos.BuscarPorCodigo(
            request.MedicoId);

        if (medico is null)
        {
            _coletorErros.AdicionarErro("Médico não encontrado.");
            return null;
        }
        if (!medico.Ativo)
        {
            _coletorErros.AdicionarErro(
                "Não é possível agendar uma consulta com um médico inativo.");

            return null;
        }

        if (request.DataHora <= DateTime.Now)
        {
            _coletorErros.AdicionarErro(
                "A data da consulta deve ser futura.");
            return null;
        }

        if (request.DuracaoMinutos <= 0)
        {
            _coletorErros.AdicionarErro(
                "A duração deve ser maior que zero.");
            return null;
        }

        var dataHoraFim = request.DataHora.AddMinutes(
            request.DuracaoMinutos);

        var existeConflito = await _uow.Consultas.ExisteConflitoAsync(
            request.MedicoId,
            request.PacienteId,
            request.DataHora,
            dataHoraFim);

        if (existeConflito)
        {
            _coletorErros.AdicionarErro(
                "Já existe uma consulta nesse horário para o médico ou paciente.");
            return null;
        }

        var consulta = new Consulta(
            request.PacienteId,
            request.MedicoId,
            request.DataHora,
            request.DuracaoMinutos);

        await _uow.Consultas.Criar(consulta);
        await _uow.Confirmar();

        return MapearParaResponse(consulta);
    }

    public async Task<ConsultaResponse?> BuscarPorCodigo(Guid codigo)
    {
        var consulta = await _uow.Consultas.BuscarPorCodigo(codigo);

        if (consulta is null)
        {
            _coletorErros.AdicionarErro("Consulta não encontrada.");
            return null;
        }

        return MapearParaResponse(consulta);
    }

    public async Task<List<ConsultaResponse>> Buscar(
        BuscarConsultaRequest filtros)
    {
        var consultas = await _uow.Consultas.Buscar(filtros);

        return consultas
            .Select(MapearParaResponse)
            .ToList();  
    }

    public async Task<ConsultaResponse?> Reagendar(
        Guid codigo,
        ReagendarConsultaRequest request)
    {
        var consulta = await _uow.Consultas.BuscarPorCodigo(codigo);
        if (!consulta.Paciente.Ativo)
        {
            _coletorErros.AdicionarErro(
                "Não é possível reagendar uma consulta para um paciente inativo.");

            return null;
        }

        if (!consulta.Medico.Ativo)
        {
            _coletorErros.AdicionarErro(
                "Não é possível reagendar uma consulta com um médico inativo.");

            return null;
        }

        if (consulta is null)
        {
            _coletorErros.AdicionarErro("Consulta não encontrada.");
            return null;
        }

        if (request.DataHora <= DateTime.Now)
        {
            _coletorErros.AdicionarErro(
                "A nova data da consulta deve ser futura.");
            return null;
        }

        if (request.DuracaoMinutos <= 0)
        {
            _coletorErros.AdicionarErro(
                "A duração deve ser maior que zero.");
            return null;
        }

        var dataHoraFim = request.DataHora.AddMinutes(
            request.DuracaoMinutos);

        var existeConflito = await _uow.Consultas.ExisteConflitoAsync(
            consulta.MedicoId,
            consulta.PacienteId,
            request.DataHora,
            dataHoraFim,
            consulta.Id);

        if (existeConflito)
        {
            _coletorErros.AdicionarErro(
                "Já existe uma consulta nesse horário para o médico ou paciente.");
            return null;
        }

        try
        {
            consulta.Reagendar(
                request.DataHora,
                request.DuracaoMinutos);
        }
        catch (InvalidOperationException ex)
        {
            _coletorErros.AdicionarErro(ex.Message);
            return null;
        }

        await _uow.Consultas.Atualizar(consulta);
        await _uow.Confirmar();

        return MapearParaResponse(consulta);
    }

    public async Task<bool> Confirmar(Guid codigo)
    {
        var consulta = await _uow.Consultas.BuscarPorCodigo(codigo);

        if (consulta is null)
        {
            _coletorErros.AdicionarErro("Consulta não encontrada.");
            return false;
        }

        try
        {
            consulta.Confirmar();
        }
        catch (InvalidOperationException ex)
        {
            _coletorErros.AdicionarErro(ex.Message);
            return false;
        }

        await _uow.Consultas.Atualizar(consulta);
        await _uow.Confirmar();

        return true;
    }

    public async Task<bool> Cancelar(Guid codigo)
    {
        var consulta = await _uow.Consultas.BuscarPorCodigo(codigo);

        if (consulta is null)
        {
            _coletorErros.AdicionarErro("Consulta não encontrada.");
            return false;
        }

        try
        {
            consulta.Cancelar();
        }
        catch (InvalidOperationException ex)
        {
            _coletorErros.AdicionarErro(ex.Message);
            return false;
        }

        await _uow.Consultas.Atualizar(consulta);
        await _uow.Confirmar();

        return true;
    }

    public async Task<bool> Concluir(Guid codigo)
    {
        var consulta = await _uow.Consultas.BuscarPorCodigo(codigo);

        if (consulta is null)
        {
            _coletorErros.AdicionarErro("Consulta não encontrada.");
            return false;
        }

        try
        {
            consulta.Concluir();
        }
        catch (InvalidOperationException ex)
        {
            _coletorErros.AdicionarErro(ex.Message);
            return false;
        }

        await _uow.Consultas.Atualizar(consulta);
        await _uow.Confirmar();

        return true;
    }

    private static ConsultaResponse MapearParaResponse(Consulta consulta)
    {
        return new ConsultaResponse
        {
            Id = consulta.Id,
            PacienteId = consulta.PacienteId,
            MedicoId = consulta.MedicoId,
            DataHora = consulta.DataHora,
            DataHoraFim = consulta.DataHoraFim,
            DuracaoMinutos = consulta.DuracaoMinutos,
            Status = consulta.Status
        };
    }
}