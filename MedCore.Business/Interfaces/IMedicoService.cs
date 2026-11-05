using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Interfaces;

public interface IMedicoService
{
    Task<MedicoResponse?> Criar(MedicoRequest request);
    Task<MedicoResponse?> BuscarPorCodigo(Guid codigo);
    Task<List<MedicoResponse>> Buscar(BuscarMedicoRequest filtros);
    Task<MedicoResponse?> Atualizar(Guid codigo, MedicoRequest request);
    Task<bool> Desativar(Guid codigo);
}
