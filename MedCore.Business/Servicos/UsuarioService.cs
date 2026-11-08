
using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using MedCore.Business.Validadores;
using MedCore.Dominio;
using MedCore.Dominio.Enums;
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
    IConfiguration _config) : IUsuarioService
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public async Task<PacienteResponse?> CadastrarPaciente(
        UsuarioRequest request)
    {
        var usuario = await CadastrarPerfil(
            request,
            TipoCadastro.Paciente);

        if (usuario is null)
            return null;

        var paciente = await _uow.Pacientes.BuscarPorUsuarioId(usuario.Id);

        if (paciente is null)
        {
            throw new InvalidOperationException(
                "Não foi possível carregar o cadastro do paciente criado.");
        }

        return new PacienteResponse
        {
            Id = paciente.Id,
            Nome = paciente.Usuario.Nome,
            Email = paciente.Usuario.Email
        };
    }

    public async Task<MedicoResponse?> CadastrarMedico(
        MedicoRequest request)
    {
        var usuario = await CadastrarPerfil(
            request,
            TipoCadastro.Medico,
            request);

        if (usuario is null)
            return null;

        var medico = await _uow.Medicos.BuscarPorUsuarioId(usuario.Id);

        if (medico is null)
        {
            throw new InvalidOperationException(
                "Não foi possível carregar o cadastro do médico criado.");
        }

        return new MedicoResponse
        {
            Id = medico.Id,
            Nome = medico.Usuario.Nome,
            CRM = medico.CRM,
            UF = medico.UF,
            Especialidade = medico.Especialidade,
            Ativo = medico.Ativo
        };
    }

    private async Task<UsuarioResponse?> CadastrarPerfil(
        UsuarioRequest request,
        TipoCadastro tipoCadastro,
        MedicoRequest? medicoRequest = null)
    {
        if (!await ValidarRequestCadastro(request, tipoCadastro, medicoRequest))
            return null;

        var validacaoCadastro = new ValidarCadastroPerfilRequest
        {
            Usuario = request,
            TipoCadastro = tipoCadastro,
            Medico = medicoRequest
        };

        if (!await _validador.ValidarAsync(validacaoCadastro))
            return null;

        var usuario = await BuscarOuCriarUsuario(request);
        await CriarPerfil(usuario, tipoCadastro, medicoRequest);
        await _uow.Confirmar();

        return MapearParaResponse(usuario);
    }

    private Task<bool> ValidarRequestCadastro(
        UsuarioRequest request,
        TipoCadastro tipoCadastro,
        MedicoRequest? medicoRequest) =>
        tipoCadastro switch
        {
            TipoCadastro.Paciente =>
                _validador.ValidarAsync<UsuarioRequest>(request),
            TipoCadastro.Medico when medicoRequest is not null =>
                _validador.ValidarAsync<MedicoRequest>(medicoRequest),
            _ => throw new ArgumentOutOfRangeException(nameof(tipoCadastro))
        };

    private async Task<Usuario> BuscarOuCriarUsuario(UsuarioRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var usuario = await _uow.Usuarios.Buscar(
            new BuscarUsuarioRequest { Email = email });

        if (usuario is not null)
            return usuario;

        usuario = new Usuario(
            request.Nome,
            email,
            request.CPF.Trim(),
            request.DataNascimento,
            "hash-temporario");

        var senhaHash = _passwordHasher.HashPassword(usuario, request.Senha);
        usuario.AlterarSenhaHash(senhaHash);
        await _uow.Usuarios.Criar(usuario);

        return usuario;
    }

    private async Task CriarPerfil(
        Usuario usuario,
        TipoCadastro tipoCadastro,
        MedicoRequest? medicoRequest)
    {
        switch (tipoCadastro)
        {
            case TipoCadastro.Paciente:
                await _uow.Pacientes.Criar(new Paciente(usuario.Id));
                break;

            case TipoCadastro.Medico:
                if (medicoRequest is null)
                    throw new InvalidOperationException(
                        "Os dados profissionais do médico são obrigatórios.");

                var medico = new Medico(
                    usuario.Id,
                    medicoRequest.CRM!.Trim(),
                    medicoRequest.UF!.Trim().ToUpperInvariant(),
                    medicoRequest.Especialidade!.Trim());

                await _uow.Medicos.Criar(medico);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(tipoCadastro));
        }
    }

    public async Task<LoginResponse?> Login(LoginRequest request)
    {
        if (!await _validador.ValidarAsync(request))
            return null;

        var email = request.Email!.Trim().ToLowerInvariant();

        var usuario = await _uow.Usuarios.Buscar(
            new BuscarUsuarioRequest { Email = email })
            ?? throw new InvalidOperationException(
                "O usuário autenticado não foi encontrado.");

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

    private static UsuarioResponse MapearParaResponse(
        Usuario usuario)
    {
        return new UsuarioResponse
        {
            Id = usuario.Id,
            Email = usuario.Email,
            Ativo = usuario.Ativo
        };
    }
}
