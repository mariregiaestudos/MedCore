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
    IColetorErros _coletorErros,
    IConfiguration _config) : IUsuarioService
{
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public async Task<UsuarioResponse?> Cadastrar(
        UsuarioRequest request)
    {
        if (!await _validador.ValidarAsync(request))
            return null;

        if (!Enum.IsDefined(typeof(TipoCadastro), request.TipoCadastro))
        {
            _coletorErros.AdicionarErro(
                "Tipo de cadastro inválido.");

            return null;
        }

        var email = request.Email!.Trim().ToLowerInvariant();
        var cpf = request.CPF!.Trim();

        var usuarioPorEmail = await _uow.Usuarios.Buscar(
            new BuscarUsuarioRequest
            {
                Email = email
            });

        var usuarioPorCPF = await _uow.Usuarios.Buscar(
            new BuscarUsuarioRequest
            {
                CPF = cpf
            });

        // O e-mail e o CPF não podem apontar para usuários diferentes.
        if (usuarioPorEmail is not null &&
            usuarioPorCPF is not null &&
            usuarioPorEmail.Id != usuarioPorCPF.Id)
        {
            _coletorErros.AdicionarErro(
                "E-mail e CPF pertencem a usuários diferentes.");

            return null;
        }

        var usuario = usuarioPorEmail ?? usuarioPorCPF;

        if (usuario is not null)
        {
            // Exige a senha da conta existente para adicionar outro papel.
            var resultadoSenha =
                _passwordHasher.VerifyHashedPassword(
                    usuario,
                    usuario.SenhaHash,
                    request.Senha!);

            if (resultadoSenha == PasswordVerificationResult.Failed)
            {
                _coletorErros.AdicionarErro(
                    "Não foi possível validar o usuário existente.");

                return null;
            }

            if (!usuario.Ativo)
            {
                _coletorErros.AdicionarErro(
                    "Este usuário está inativo.");

                return null;
            }

            // Não altera os dados pessoais nem a senha do usuário existente.
        }
        else
        {
            // Cria um usuário novo.
            usuario = new Usuario(
                request.Nome!,
                email,
                cpf,
                request.DataNascimento,
                "hash-temporario");

            var senhaHash = _passwordHasher.HashPassword(
                usuario,
                request.Senha!);

            usuario.AlterarSenhaHash(senhaHash);

            await _uow.Usuarios.Criar(usuario);
        }

        switch (request.TipoCadastro)
        {
            case TipoCadastro.Paciente:
                {
                    var pacienteExistente =
                        await _uow.Pacientes.BuscarPorCodigo(usuario.Id);

                    if (pacienteExistente is not null)
                    {
                        _coletorErros.AdicionarErro(
                            "Este usuário já possui cadastro de paciente.");

                        return null;
                    }

                    var paciente = new Paciente(usuario.Id);

                    await _uow.Pacientes.Criar(paciente);
                    break;
                }

            case TipoCadastro.Medico:
                {
                    if (string.IsNullOrWhiteSpace(request.CRM) ||
                        string.IsNullOrWhiteSpace(request.UF) ||
                        string.IsNullOrWhiteSpace(request.Especialidade))
                    {
                        _coletorErros.AdicionarErro(
                            "CRM, UF e especialidade são obrigatórios para médicos.");

                        return null;
                    }

                    var crm = request.CRM.Trim();
                    var uf = request.UF.Trim().ToUpperInvariant();

                    if (await _uow.Medicos.ExisteCRM(crm, uf))
                    {
                        _coletorErros.AdicionarErro(
                            "Já existe um médico cadastrado com este CRM e UF.");

                        return null;
                    }

                    var medicoExistente =
                        await _uow.Medicos.BuscarPorCodigo(usuario.Id);

                    if (medicoExistente is not null)
                    {
                        _coletorErros.AdicionarErro(
                            "Este usuário já possui cadastro de médico.");

                        return null;
                    }

                    var medico = new Medico(
                        usuario.Id,
                        crm,
                        uf,
                        request.Especialidade);

                    await _uow.Medicos.Criar(medico);
                    break;
                }
        }

        await _uow.Confirmar();

        return MapearParaResponse(usuario);
    }

    public async Task<LoginResponse?> Login(LoginRequest request)
    {
        if (!await _validador.ValidarAsync(request))
            return null;

        var email = request.Email!.Trim().ToLowerInvariant();

        var usuario = await _uow.Usuarios.Buscar(new BuscarUsuarioRequest
        {
            Email = email
        });

        if (usuario is null || !usuario.Ativo)
        {
            _coletorErros.AdicionarErro(
                "E-mail ou senha inválidos.");

            return null;
        }

        var resultado = _passwordHasher.VerifyHashedPassword(
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