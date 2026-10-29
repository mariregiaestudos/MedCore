using MedCore.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Interfaces.IRepositorios;

public interface IUsuarioRepositorio
{
    Task<Usuario?> BuscarPorId(Guid id);

    Task<Usuario?> BuscarPorEmail(string email);

    Task<bool> ExisteEmail(string email);

    Task Adicionar(Usuario usuario);
}
