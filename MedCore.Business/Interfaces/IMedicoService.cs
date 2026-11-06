
using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;

namespace MedCore.Business.Interfaces;

public interface IMedicoService
{
    Task<MedicoResponse?> BuscarPorCodigo(Guid codigo);

    Task<List<MedicoResponse>> Buscar(
        BuscarMedicoRequest filtros);

    Task<MedicoResponse?> Atualizar(
        Guid codigo,
        MedicoRequest request);

    Task<bool> Desativar(Guid codigo);
}
