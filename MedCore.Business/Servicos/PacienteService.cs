
using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using MedCore.Business.Validadores;
using MedCore.Dominio;

namespace MedCore.Business.Servicos;

public class PacienteService(
    IUnityOfWork _uow,
    ValidadorService _validador,
    IColetorErros _coletorErros) : IPacienteService
{
    public async Task<PacienteResponse?> Criar(PacienteRequest request)
    {
        if (!await _validador.ValidarAsync(request))
            return null;

        var email = request.Email!.Trim();

        if (await _uow.Pacientes.ExisteEmail(email))
        {
            _coletorErros.AdicionarErro(
                "Já existe um paciente cadastrado com esse e-mail.");

            return null;
        }

        var paciente = new Paciente(request.Nome!, email);

        await _uow.Pacientes.Criar(paciente);
        await _uow.Confirmar();

        return MapearParaResponse(paciente);
    }

    public async Task<PacienteResponse?> BuscarPorCodigo(Guid codigo)
    {
        var paciente = await _uow.Pacientes.BuscarPorCodigo(codigo);

        if (paciente is null)
        {
            _coletorErros.AdicionarErro("Paciente não encontrado.");
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

    public async Task<PacienteResponse?> Atualizar(
        Guid codigo,
        PacienteRequest request)
    {
        if (!await _validador.ValidarAsync(request))
            return null;

        var paciente = await _uow.Pacientes.BuscarPorCodigo(codigo);

        if (paciente is null)
        {
            _coletorErros.AdicionarErro("Paciente não encontrado.");
            return null;
        }

        var email = request.Email!.Trim();

        if (!string.Equals(
                paciente.Email,
                email,
                StringComparison.OrdinalIgnoreCase)
            && await _uow.Pacientes.ExisteEmail(email))
        {
            _coletorErros.AdicionarErro(
                "Já existe outro paciente cadastrado com esse e-mail.");

            return null;
        }

        paciente.AtualizarDados(request.Nome!, email);

        await _uow.Pacientes.Atualizar(paciente);
        await _uow.Confirmar();

        return MapearParaResponse(paciente);
    }

    public async Task<bool> Desativar(Guid codigo)
    {
        var paciente = await _uow.Pacientes.BuscarPorCodigo(codigo);

        if (paciente is null)
        {
            _coletorErros.AdicionarErro("Paciente não encontrado.");
            return false;
        }

        paciente.Desativar();

        await _uow.Pacientes.Atualizar(paciente);
        await _uow.Confirmar();

        return true;
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
