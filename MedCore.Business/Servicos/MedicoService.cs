using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using MedCore.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Servicos;

public class MedicoService(
    IUnityOfWork _uow,
    IColetorErros _coletorErros) : IMedicoService
{
    public async Task<MedicoResponse?> Criar(MedicoRequest request)
    {
        if (!Validar(request))
            return null;

        var crm = request.CRM!.Trim();
        var uf = request.UF!.Trim().ToUpperInvariant();

        if (await _uow.Medicos.ExisteCRM(crm, uf))
        {
            _coletorErros.AdicionarErro(
                "Já existe um médico cadastrado com esse CRM e UF.");
            return null;
        }

        var medico = new Medico(
            request.Nome!.Trim(),
            crm,
            uf,
            request.Especialidade!.Trim());

        await _uow.Medicos.Criar(medico);
        await _uow.Confirmar();

        return MapearParaResponse(medico);
    }

    public async Task<MedicoResponse?> BuscarPorCodigo(Guid codigo)
    {
        var medico = await _uow.Medicos.BuscarPorCodigo(codigo);

        if (medico is null)
        {
            _coletorErros.AdicionarErro("Médico não encontrado.");
            return null;
        }

        return MapearParaResponse(medico);
    }

    public async Task<List<MedicoResponse>> Buscar(
        BuscarMedicoRequest filtros)
    {
        var medicos = await _uow.Medicos.Buscar(filtros);

        return medicos.Select(MapearParaResponse).ToList();
    }

    public async Task<MedicoResponse?> Atualizar(
        Guid codigo,
        MedicoRequest request)
    {
        if (!Validar(request))
            return null;

        var medico = await _uow.Medicos.BuscarPorCodigo(codigo);

        if (medico is null)
        {
            _coletorErros.AdicionarErro("Médico não encontrado.");
            return null;
        }

        var crm = request.CRM!.Trim();
        var uf = request.UF!.Trim().ToUpperInvariant();

        if ((medico.CRM != crm || medico.UF != uf) &&
            await _uow.Medicos.ExisteCRM(crm, uf))
        {
            _coletorErros.AdicionarErro(
                "Já existe outro médico cadastrado com esse CRM e UF.");
            return null;
        }

        medico.AlterarDados(
            request.Nome!.Trim(),
            crm,
            uf,
            request.Especialidade!.Trim());

        await _uow.Medicos.Atualizar(medico);
        await _uow.Confirmar();

        return MapearParaResponse(medico);
    }

    public async Task<bool> Desativar(Guid codigo)
    {
        var medico = await _uow.Medicos.BuscarPorCodigo(codigo);

        if (medico is null)
        {
            _coletorErros.AdicionarErro("Médico não encontrado.");
            return false;
        }

        medico.Desativar();

        await _uow.Medicos.Atualizar(medico);
        await _uow.Confirmar();

        return true;
    }

    private bool Validar(MedicoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome) ||
            string.IsNullOrWhiteSpace(request.CRM) ||
            string.IsNullOrWhiteSpace(request.UF) ||
            string.IsNullOrWhiteSpace(request.Especialidade))
        {
            _coletorErros.AdicionarErro(
                "Nome, CRM, UF e especialidade são obrigatórios.");
            return false;
        }

        if (request.UF.Trim().Length != 2)
        {
            _coletorErros.AdicionarErro(
                "A UF deve conter exatamente duas letras.");
            return false;
        }

        return true;
    }

    private static MedicoResponse MapearParaResponse(Medico medico)
    {
        return new MedicoResponse
        {
            Id = medico.Id,
            Nome = medico.Nome,
            CRM = medico.CRM,
            UF = medico.UF,
            Especialidade = medico.Especialidade
        };
    }
}