using MedCore.Business.Interfaces;
using MedCore.Infraestrutura.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Infraestrutura.UoW;

public class UnityOfWork(MedCoreContexto _contexto, IPacienteRepositorio _pacienteRepositorio) : IUnityOfWork
{
    public IPacienteRepositorio Pacientes => _pacienteRepositorio;

    public async Task<int> Confirmar()
    {
        return await _contexto.SaveChangesAsync();
    }
}
