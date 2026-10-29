using MedCore.Business.Interfaces.IRepositorios;
using MedCore.Dominio;
using MedCore.Infraestrutura.Contexto;
using Microsoft.EntityFrameworkCore;

namespace MedCore.Infraestrutura.Repositorios;

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly MedCoreContexto _contexto;

    public UsuarioRepositorio(MedCoreContexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<Usuario?> BuscarPorId(Guid id)
    {
        return await _contexto.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<Usuario?> BuscarPorEmail(string email)
    {
        return await _contexto.Usuarios
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExisteEmail(string email)
    {
        return await _contexto.Usuarios
            .AnyAsync(u => u.Email == email);
    }

    public async Task Adicionar(Usuario usuario)
    {
        await _contexto.Usuarios.AddAsync(usuario);
    }
}
