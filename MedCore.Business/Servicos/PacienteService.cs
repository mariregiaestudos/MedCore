
using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using MedCore.Business.Validadores;
using MedCore.Dominio;

namespace MedCore.Business.Servicos;

public class PacienteService(
    IUnityOfWork _uow,
    ValidadorService _validador) : IPacienteService
{
    public async Task<PacienteResponse?> BuscarPorCodigo(Guid codigo)
    {
        if (!await ValidarPaciente(codigo, OperacaoPaciente.Buscar))
            return null;

        var paciente = await _uow.Pacientes.BuscarPorCodigo(codigo)
            ?? throw new InvalidOperationException(
                "O paciente validado não foi encontrado.");

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
        if (!await ValidarPaciente(codigo, OperacaoPaciente.Desativar))
            return false;

        var paciente = await _uow.Pacientes.BuscarPorCodigo(codigo)
            ?? throw new InvalidOperationException(
                "O paciente validado não foi encontrado.");

        paciente.Desativar();

        await _uow.Pacientes.Atualizar(paciente);
        await _uow.Confirmar();

        return true;
    }

    private Task<bool> ValidarPaciente(Guid codigo, OperacaoPaciente operacao) =>
        _validador.ValidarAsync(new ValidarPacienteRequest
        {
            Codigo = codigo,
            Operacao = operacao
        });

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
