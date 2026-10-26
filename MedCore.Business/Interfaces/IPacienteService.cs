using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Interfaces;

public interface IPacienteService
{

    Task<List<PacienteResponse>> Buscar(BuscarPacienteRequest filtros);
    Task<PacienteResponse?> Criar(PacienteRequest request);
    Task<PacienteResponse?> BuscarPorCodigo(Guid codigo);
    Task<PacienteResponse?> Atualizar(Guid codigo, PacienteRequest request);
    Task<bool> Desativar(Guid codigo);
}
