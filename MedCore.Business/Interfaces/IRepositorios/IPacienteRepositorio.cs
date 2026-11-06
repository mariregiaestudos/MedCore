
using MedCore.Business.DTOs.Request;
using MedCore.Dominio;

namespace MedCore.Business.Interfaces.IRepositorios;

public interface IPacienteRepositorio
{
    Task Criar(Paciente paciente);

    Task<Paciente?> BuscarPorCodigo(Guid id);

    Task<Paciente?> BuscarPorUsuarioId(Guid usuarioId);

    Task<List<Paciente>> Buscar(BuscarPacienteRequest filtros);

    Task Atualizar(Paciente paciente);
}
