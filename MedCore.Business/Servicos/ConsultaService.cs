using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using MedCore.Business.Validadores;
using MedCore.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Servicos;

public class ConsultaService(
    IUnityOfWork _uow,
    ValidadorService _validador) : IConsultaService
{
    public async Task<ConsultaResponse?> Agendar(
        Guid usuarioId,
        ConsultaRequest request)
    {
        var validacao = new AgendarConsultaValidationRequest
        {
            UsuarioId = usuarioId,
            Consulta = request
        };

        if (!await _validador.ValidarAsync(validacao))
            return null;

        var paciente = await _uow.Pacientes.BuscarPorUsuarioId(usuarioId)
            ?? throw new InvalidOperationException(
                "O perfil de paciente validado não foi encontrado.");

        var consulta = new Consulta(
            paciente.Id,
            request.MedicoId,
            request.DataHora,
            request.DuracaoMinutos);

        await _uow.Consultas.Criar(consulta);
        await _uow.Confirmar();

        return MapearParaResponse(consulta);
    }

    public async Task<ConsultaResponse?> BuscarPorCodigo(
        Guid codigo,
        Guid usuarioId)
    {
        if (!await ValidarConsulta(codigo, usuarioId, OperacaoConsulta.Buscar))
            return null;

        var consulta = await _uow.Consultas.BuscarPorCodigo(codigo)
            ?? throw new InvalidOperationException(
                "A consulta validada não foi encontrada.");

        return MapearParaResponse(consulta);
    }

    public async Task<List<ConsultaResponse>> Buscar(
        BuscarConsultaRequest filtros,
        Guid usuarioId)
    {
        if (!await ValidarConsulta(Guid.Empty, usuarioId, OperacaoConsulta.Listar))
            return [];

        var paciente = await _uow.Pacientes.BuscarPorUsuarioId(usuarioId)
            ?? throw new InvalidOperationException(
                "O perfil de paciente validado não foi encontrado.");

        var consultas = await _uow.Consultas.Buscar(new BuscarConsultaRequest
        {
            PacienteId = paciente.Id,
            MedicoId = filtros.MedicoId,
            DataInicio = filtros.DataInicio,
            DataFim = filtros.DataFim,
            Status = filtros.Status
        });

        return consultas.Select(MapearParaResponse).ToList();
    }

    public async Task<ConsultaResponse?> Reagendar(
        Guid codigo,
        Guid usuarioId,
        ReagendarConsultaRequest request)
    {
        var validacao = new ReagendarConsultaValidationRequest
        {
            Codigo = codigo,
            UsuarioId = usuarioId,
            Consulta = request
        };

        if (!await _validador.ValidarAsync(validacao))
            return null;

        var consulta = await _uow.Consultas.BuscarPorCodigo(codigo)
            ?? throw new InvalidOperationException(
                "A consulta validada não foi encontrada.");

        consulta.Reagendar(
            request.MedicoId,
            request.DataHora,
            request.DuracaoMinutos);

        await _uow.Consultas.Atualizar(consulta);
        await _uow.Confirmar();

        return MapearParaResponse(consulta);
    }

    public async Task<bool> Confirmar(Guid codigo)
    {
        if (!await ValidarConsulta(codigo, null, OperacaoConsulta.Confirmar))
            return false;

        var consulta = await _uow.Consultas.BuscarPorCodigo(codigo)
            ?? throw new InvalidOperationException(
                "A consulta validada não foi encontrada.");

        consulta.Confirmar();
        await _uow.Consultas.Atualizar(consulta);
        await _uow.Confirmar();

        return true;
    }

    public async Task<bool> Cancelar(Guid codigo, Guid usuarioId)
    {
        if (!await ValidarConsulta(codigo, usuarioId, OperacaoConsulta.Cancelar))
            return false;

        var consulta = await _uow.Consultas.BuscarPorCodigo(codigo)
            ?? throw new InvalidOperationException(
                "A consulta validada não foi encontrada.");

        consulta.Cancelar();
        await _uow.Consultas.Atualizar(consulta);
        await _uow.Confirmar();

        return true;
    }

    public async Task<bool> Concluir(Guid codigo)
    {
        if (!await ValidarConsulta(codigo, null, OperacaoConsulta.Concluir))
            return false;

        var consulta = await _uow.Consultas.BuscarPorCodigo(codigo)
            ?? throw new InvalidOperationException(
                "A consulta validada não foi encontrada.");

        consulta.Concluir();
        await _uow.Consultas.Atualizar(consulta);
        await _uow.Confirmar();

        return true;
    }

    private Task<bool> ValidarConsulta(
        Guid codigo,
        Guid? usuarioId,
        OperacaoConsulta operacao) =>
        _validador.ValidarAsync(new ValidarConsultaRequest
        {
            Codigo = codigo,
            UsuarioId = usuarioId,
            Operacao = operacao
        });

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