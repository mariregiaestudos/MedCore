
using MedCore.Business.Interfaces.IRepositorios;

namespace MedCore.Business.Interfaces;

public interface IUnityOfWork
{
    IPacienteRepositorio Pacientes { get; }
    IUsuarioRepositorio Usuarios { get; }
    IConsultaRepositorio Consultas { get; }
    IMedicoRepositorio Medicos { get; }

    Task<int> Confirmar();
}
