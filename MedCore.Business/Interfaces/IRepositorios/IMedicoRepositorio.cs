
using MedCore.Business.DTOs.Request;
using MedCore.Dominio;

namespace MedCore.Business.Interfaces.IRepositorios;

public interface IMedicoRepositorio
{
    Task Criar(Medico medico);

    Task<Medico?> BuscarPorCodigo(Guid codigo);

    Task<Medico?> BuscarPorUsuarioId(Guid usuarioId);

    Task<List<Medico>> Buscar(BuscarMedicoRequest filtros);

    Task Atualizar(Medico medico);

    Task<bool> ExisteCRM(string crm, string uf);
}
