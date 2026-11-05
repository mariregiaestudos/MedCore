using MedCore.Business.DTOs.Request;
using MedCore.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Interfaces.IRepositorios;

public interface IUsuarioRepositorio
{
    Task<Usuario?> BuscarPorId(Guid id);
    Task<Usuario?> Buscar(BuscarUsuarioRequest request);
    Task<bool> ExisteEmail(string email);
    Task Criar(Usuario usuario);
    Task Atualizar(Usuario usuario);
}
