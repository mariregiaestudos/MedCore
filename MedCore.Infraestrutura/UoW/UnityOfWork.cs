using MedCore.Business.Interfaces;
using MedCore.Business.Interfaces.IRepositorios;
using MedCore.Infraestrutura.Contexto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Infraestrutura.UoW;

public class UnityOfWork(MedCoreContexto _contexto, IUsuarioRepositorio _usuarioRepositorio, IPacienteRepositorio _pacienteRepositorio, IConsultaRepositorio _consultaRepositorio) : IUnityOfWork
{
    public IUsuarioRepositorio Usuarios => _usuarioRepositorio;
    public IPacienteRepositorio Pacientes => _pacienteRepositorio;
    public IConsultaRepositorio Consultas => _consultaRepositorio;
    public IMedicoRepositorio Medicos => _medicoRepositorio;


    public async Task<int> Confirmar()
    {
        return await _contexto.SaveChangesAsync();
    }
}
