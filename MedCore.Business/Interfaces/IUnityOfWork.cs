
using MedCore.Business.Interfaces.IRepositorios;

namespace MedCore.Business.Interfaces;

public interface IUnityOfWork
{
    IPacienteRepositorio Pacientes { get; }
    IUsuarioRepositorio Usuarios { get; }

    Task<int> Confirmar();
}
