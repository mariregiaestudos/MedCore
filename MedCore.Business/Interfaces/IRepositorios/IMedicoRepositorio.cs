using MedCore.Business.DTOs.Request;
using MedCore.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Interfaces.IRepositorios;

public interface IMedicoRepositorio
{
    Task Criar(Medico medico);
    Task<Medico?> BuscarPorCodigo(Guid codigo);
    Task<List<Medico>> Buscar(BuscarMedicoRequest filtros);
    Task Atualizar(Medico medico);
    Task<bool> ExisteCRM(string crm, string uf);
}
