
using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using MedCore.Dominio;

namespace MedCore.Business.Servicos;

public class MedicoService(
    IUnityOfWork _uow,
    IColetorErros _coletorErros) : IMedicoService
{
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

        return medicos
            .Select(MapearParaResponse)
            .ToList();
    }

    public async Task<MedicoResponse?> Atualizar(
        Guid codigo,
        MedicoRequest request)
    {
        if (!ValidarDadosMedicos(request))
            return null;

        var medico = await _uow.Medicos.BuscarPorCodigo(codigo);

        if (medico is null)
        {
            _coletorErros.AdicionarErro("Médico não encontrado.");
            return null;
        }

        var crm = request.CRM!.Trim();
        var uf = request.UF!.Trim().ToUpperInvariant();
        var especialidade = request.Especialidade!.Trim();

        // Verifica se outro médico já utiliza o CRM e a UF.
        if ((medico.CRM != crm || medico.UF != uf) &&
            await _uow.Medicos.ExisteCRM(crm, uf))
        {
            _coletorErros.AdicionarErro(
                "Já existe outro médico cadastrado com esse CRM e UF.");

            return null;
        }

        medico.AlterarDados(crm, uf, especialidade);

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

    private bool ValidarDadosMedicos(MedicoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CRM) ||
            string.IsNullOrWhiteSpace(request.UF) ||
            string.IsNullOrWhiteSpace(request.Especialidade))
        {
            _coletorErros.AdicionarErro(
                "CRM, UF e especialidade são obrigatórios.");

            return false;
        }

        var uf = request.UF.Trim();

        if (uf.Length != 2 || !uf.All(char.IsLetter))
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
            Nome = medico.Usuario.Nome,
            CRM = medico.CRM,
            UF = medico.UF,
            Especialidade = medico.Especialidade
        };
    }
}
