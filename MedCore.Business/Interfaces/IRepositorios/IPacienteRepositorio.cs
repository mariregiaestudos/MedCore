using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Interfaces;

public interface IPacienteRepositorio
{
    Task Criar(Paciente paciente);
    Task<Paciente?> BuscarPorCodigo(Guid codigo);
    Task<List<Paciente>> Buscar(BuscarPacienteRequest filtros);
    Task Atualizar(Paciente paciente);
    Task<bool> ExisteEmail(string email);
}
