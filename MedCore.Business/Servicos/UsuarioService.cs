
using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using MedCore.Business.Validadores;
using MedCore.Dominio;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MedCore.Business.Servicos;

public class UsuarioService(
    IUnityOfWork _uow,
    ValidadorService _validador,
    IColetorErros _coletorErros,
    IConfiguration _config) : IUsuarioService
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public async Task<UsuarioResponse?> Cadastrar(UsuarioRequest request)
    {
        if (!await _validador.ValidarAsync(request))
            return null;

        var email = request.Email!.Trim();

        if (await _uow.Usuarios.ExisteEmail(email))
        {
            _coletorErros.AdicionarErro(
                "Já existe um usuário cadastrado com esse e-mail.");

            return null;
        }

        Usuario usuario = new Usuario(email, string.Empty);

        var senhaHash = _passwordHasher.HashPassword(
            usuario,
            request.Senha!);

        usuario.AtualizarSenha(senhaHash);

        await _uow.Usuarios.Adicionar(usuario);
        await _uow.Confirmar();

        return MapearParaResponse(usuario);
    }

    public async Task<LoginResponse?> Login(LoginRequest request)
    {
        if (!await _validador.ValidarAsync(request))
            return null;

        var email = request.Email!.Trim();

        var usuario = await _uow.Usuarios.BuscarPorEmail(email);

        if (usuario is null || !usuario.Ativo)
        {
            _coletorErros.AdicionarErro(
                "E-mail ou senha inválidos.");

            return null;
        }

        var passwordHasher = new PasswordHasher<Usuario>();

        var resultado = passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            request.Senha!);

        if (resultado == PasswordVerificationResult.Failed)
        {
            _coletorErros.AdicionarErro(
                "E-mail ou senha inválidos.");

            return null;
        }

        var chave = _config["Jwt:Key"]!;
        var emissor = _config["Jwt:Issuer"]!;
        var audiencia = _config["Jwt:Audience"]!;

        var minutos = int.Parse(
            _config["Jwt:ExpirationMinutes"] ?? "60");

        var agora = DateTime.UtcNow;
        var expiracao = agora.AddMinutes(minutos);

        var claims = new[]
        {
        new Claim(
            JwtRegisteredClaimNames.Sub,
            usuario.Id.ToString()),

        new Claim(
            JwtRegisteredClaimNames.Email,
            usuario.Email),

        new Claim(
            ClaimTypes.NameIdentifier,
            usuario.Id.ToString())
    };

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(chave)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: emissor,
            audience: audiencia,
            claims: claims,
            notBefore: agora,
            expires: expiracao,
            signingCredentials: credenciais);

        return new LoginResponse
        {
            AccessToken = new JwtSecurityTokenHandler()
                .WriteToken(token),

            ExpiraEm = expiracao
        };
    }


    private static UsuarioResponse MapearParaResponse(Usuario usuario)
    {
        return new UsuarioResponse
        {
            Codigo = usuario.Id,
            Email = usuario.Email,
            Ativo = usuario.Ativo        };
    }
}
