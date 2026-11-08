using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Interfaces;

public interface IConsultaService
{
    Task<ConsultaResponse?> Agendar(
        Guid usuarioId,
        ConsultaRequest request);

    Task<ConsultaResponse?> BuscarPorCodigo(Guid codigo, Guid usuarioId);

    Task<List<ConsultaResponse>> Buscar(
        BuscarConsultaRequest filtros,
        Guid usuarioId);

    Task<ConsultaResponse?> Reagendar(
        Guid codigo,
        Guid usuarioId,
        ReagendarConsultaRequest request);

    Task<bool> Confirmar(Guid codigo);

    Task<bool> Cancelar(Guid codigo, Guid usuarioId);

    Task<bool> Concluir(Guid codigo);
}
