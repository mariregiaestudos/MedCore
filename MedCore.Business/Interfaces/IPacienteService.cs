
using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;

namespace MedCore.Business.Interfaces;

public interface IPacienteService
{
    Task<PacienteResponse?> BuscarPorCodigo(Guid codigo);

    Task<List<PacienteResponse>> Buscar(
        BuscarPacienteRequest filtros);

    Task<bool> Desativar(Guid codigo);
}
