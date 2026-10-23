using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Interfaces;

public interface IUnityOfWork
{
    IPacienteRepositorio Pacientes { get; }

    //O Task<int> representa a quantidade de entradas afetadas pelo SaveChangesAsync do EF Core.
    Task<int> Confirmar();
}
