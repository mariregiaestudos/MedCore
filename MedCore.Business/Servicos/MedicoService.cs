
using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using MedCore.Business.Validadores;
using MedCore.Dominio;

namespace MedCore.Business.Servicos;

public class MedicoService(
    IUnityOfWork _uow,
    ValidadorService _validador) : IMedicoService
{
    public async Task<MedicoResponse?> BuscarPorCodigo(Guid codigo)
    {
        if (!await ValidarMedico(codigo, OperacaoMedico.Buscar))
            return null;

        var medico = await _uow.Medicos.BuscarPorCodigo(codigo)
            ?? throw new InvalidOperationException(
                "O médico validado não foi encontrado.");

        return MapearParaResponse(medico);
    }

    public async Task<List<MedicoResponse>> Buscar(
        BuscarMedicoRequest filtros)
    {
        var medicos = await _uow.Medicos.Buscar(filtros);

        return medicos
            .Select(MapearParaResponse)
            .ToList();
    }

    public async Task<MedicoResponse?> Atualizar(
        Guid codigo,
        AtualizarMedicoRequest request)
    {
        if (!await ValidarMedico(codigo, OperacaoMedico.Atualizar, request))
            return null;

        var medico = await _uow.Medicos.BuscarPorCodigo(codigo)
            ?? throw new InvalidOperationException(
                "O médico validado não foi encontrado.");

        medico.AlterarDados(
            request.CRM!.Trim(),
            request.UF!.Trim().ToUpperInvariant(),
            request.Especialidade!.Trim());

        await _uow.Medicos.Atualizar(medico);
        await _uow.Confirmar();

        return MapearParaResponse(medico);
    }

    public async Task<bool> Desativar(Guid codigo)
    {
        if (!await ValidarMedico(codigo, OperacaoMedico.Desativar))
            return false;

        var medico = await _uow.Medicos.BuscarPorCodigo(codigo)
            ?? throw new InvalidOperationException(
                "O médico validado não foi encontrado.");

        medico.Desativar();

        await _uow.Medicos.Atualizar(medico);
        await _uow.Confirmar();

        return true;
    }

    private Task<bool> ValidarMedico(
        Guid codigo,
        OperacaoMedico operacao,
        AtualizarMedicoRequest? dados = null) =>
        _validador.ValidarAsync(new ValidarMedicoRequest
        {
            Codigo = codigo,
            Operacao = operacao,
            Dados = dados
        });

    private static MedicoResponse MapearParaResponse(Medico medico)
    {
        return new MedicoResponse
        {
            Id = medico.Id,
            Nome = medico.Usuario.Nome,
            CRM = medico.CRM,
            UF = medico.UF,
            Especialidade = medico.Especialidade,
            Ativo = medico.Ativo
        };
    }
}
