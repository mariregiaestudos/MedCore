using MedCore.Dominio;

namespace MedCore.Business.Interfaces.IRepositorios;

public interface IConsultaRepositorio
{
    Task<bool> ExisteConflitoAsync(
        Guid medicoId,
        Guid pacienteId,
        DateTime dataHora,
        DateTime dataHoraFim,
        Guid? consultaIdIgnorada = null);

    Task Criar(Consulta consulta);

    Task<Consulta?> BuscarPorCodigo(Guid codigo);

    Task<List<Consulta>> Buscar();

    Task Atualizar(Consulta consulta);
}