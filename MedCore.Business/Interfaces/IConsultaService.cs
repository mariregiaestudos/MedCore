using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Interfaces;

public interface IConsultaService
{
    Task<ConsultaResponse?> Agendar(ConsultaRequest request);

    Task<ConsultaResponse?> BuscarPorCodigo(Guid codigo);

    Task<List<ConsultaResponse>> Buscar(BuscarConsultaRequest filtros);

    Task<ConsultaResponse?> Reagendar(
        Guid codigo,
        ReagendarConsultaRequest request);

    Task<bool> Confirmar(Guid codigo);

    Task<bool> Cancelar(Guid codigo);

    Task<bool> Concluir(Guid codigo);
}
