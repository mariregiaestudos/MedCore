using MedCore.Business.DTOs.Request;
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
    public async Task<Usuario?> Buscar(BuscarUsuarioRequest request)
    {
        var query = _contexto.Usuarios.AsQueryable();

        if (request.Id.HasValue)
            query = query.Where(u => u.Id == request.Id.Value);

        if (!string.IsNullOrWhiteSpace(request.Nome))
        {
            var nome = request.Nome.Trim();

            query = query.Where(u => u.Nome.Contains(nome));
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToLowerInvariant();

            query = query.Where(u => u.Email == email);
        }

        if (!string.IsNullOrWhiteSpace(request.CPF))
        {
            var cpf = request.CPF.Trim();

            query = query.Where(u => u.CPF == cpf);
        }

        return await query.SingleOrDefaultAsync();
    }

    public async Task<Usuario?> BuscarPorId(Guid id)
    {
        return await _contexto.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<bool> ExisteEmail(string email)
    {
        return await _contexto.Usuarios
            .AnyAsync(u => u.Email == email);
    }

    public async Task Criar(Usuario usuario)
    {
        await _contexto.Usuarios.AddAsync(usuario);
    }

    public async Task Atualizar(Usuario usuario)
    {
        _contexto.Usuarios.Update(usuario);
    }   
}
