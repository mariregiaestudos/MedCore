
using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using MedCore.Dominio;

namespace MedCore.Business.Servicos;

public class PacienteService(
    IUnityOfWork _uow,
    IColetorErros _coletorErros) : IPacienteService
{
    public async Task<PacienteResponse?> BuscarPorCodigo(Guid codigo)
    {
        var paciente = await _uow.Pacientes.BuscarPorCodigo(codigo);

        if (paciente is null)
        {
            _coletorErros.AdicionarErro(
                "Paciente não encontrado.");

            return null;
        }

        return MapearParaResponse(paciente);
    }

    public async Task<List<PacienteResponse>> Buscar(
        BuscarPacienteRequest filtros)
    {
        var pacientes = await _uow.Pacientes.Buscar(filtros);

        return pacientes
            .Select(MapearParaResponse)
            .ToList();
    }

    public async Task<bool> Desativar(Guid codigo)
    {
        var paciente = await _uow.Pacientes.BuscarPorCodigo(codigo);

        if (paciente is null)
        {
            _coletorErros.AdicionarErro(
                "Paciente não encontrado.");

            return false;
        }

        paciente.Desativar();

        await _uow.Pacientes.Atualizar(paciente);
        await _uow.Confirmar();

        return true;
    }

    private static PacienteResponse MapearParaResponse(
        Paciente paciente)
    {
        return new PacienteResponse
        {
            Id = paciente.Id,
            Nome = paciente.Usuario.Nome,
            Email = paciente.Usuario.Email
        };
    }
}
