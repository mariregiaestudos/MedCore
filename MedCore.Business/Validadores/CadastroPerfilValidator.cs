using FluentValidation;
using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces;
using MedCore.Dominio;
using MedCore.Dominio.Enums;
using Microsoft.AspNetCore.Identity;

namespace MedCore.Business.Validadores;

public class CadastroPerfilValidator
    : AbstractValidator<ValidarCadastroPerfilRequest>
{
    private readonly IUnityOfWork _uow;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public CadastroPerfilValidator(IUnityOfWork uow)
    {
        _uow = uow;
        RuleFor(request => request).CustomAsync(ValidarCadastroAsync);
    }

    private async Task ValidarCadastroAsync(
        ValidarCadastroPerfilRequest request,
        ValidationContext<ValidarCadastroPerfilRequest> context,
        CancellationToken cancellationToken)
    {
        var email = request.Usuario.Email.Trim().ToLowerInvariant();
        var cpf = request.Usuario.CPF.Trim();

        var usuarioPorEmail = await _uow.Usuarios.Buscar(
            new BuscarUsuarioRequest { Email = email });
        var usuarioPorCpf = await _uow.Usuarios.Buscar(
            new BuscarUsuarioRequest { CPF = cpf });

        if ((usuarioPorEmail is null) != (usuarioPorCpf is null))
        {
            context.AddFailure(
                "Usuario.Email",
                "O e-mail e o CPF informados não correspondem à mesma conta.");

            return;
        }

        if (usuarioPorEmail is not null &&
            usuarioPorCpf is not null &&
            usuarioPorEmail.Id != usuarioPorCpf.Id)
        {
            context.AddFailure(
                "Usuario.Email",
                "E-mail e CPF pertencem a usuários diferentes.");

            return;
        }

        var usuario = usuarioPorEmail ?? usuarioPorCpf;

        if (usuario is null)
        {
            await ValidarCRMAsync(request, context);
            return;
        }

        var resultadoSenha = _passwordHasher.VerifyHashedPassword(
            usuario,
            usuario.SenhaHash,
            request.Usuario.Senha);

        if (resultadoSenha == PasswordVerificationResult.Failed)
        {
            context.AddFailure(
                "Usuario.Senha",
                "Não foi possível validar o usuário existente.");

            return;
        }

        if (!usuario.Ativo)
        {
            context.AddFailure(
                "Usuario.Email",
                "Este usuário está inativo.");

            return;
        }

        if (request.TipoCadastro == TipoCadastro.Paciente)
        {
            var paciente = await _uow.Pacientes.BuscarPorUsuarioId(usuario.Id);

            if (paciente is not null)
            {
                context.AddFailure(
                    "Usuario.Email",
                    "Este usuário já possui cadastro de paciente.");
            }

            return;
        }

        var medico = await _uow.Medicos.BuscarPorUsuarioId(usuario.Id);

        if (medico is not null)
        {
            context.AddFailure(
                "Usuario.Email",
                "Este usuário já possui cadastro de médico.");

            return;
        }

        await ValidarCRMAsync(request, context);
    }

    private async Task ValidarCRMAsync(
        ValidarCadastroPerfilRequest request,
        ValidationContext<ValidarCadastroPerfilRequest> context)
    {
        if (request.TipoCadastro != TipoCadastro.Medico ||
            request.Medico is null ||
            string.IsNullOrWhiteSpace(request.Medico.CRM) ||
            string.IsNullOrWhiteSpace(request.Medico.UF))
        {
            return;
        }

        var existeCrm = await _uow.Medicos.ExisteCRM(
            request.Medico.CRM.Trim(),
            request.Medico.UF.Trim().ToUpperInvariant());

        if (existeCrm)
        {
            context.AddFailure(
                "Medico.CRM",
                "Já existe um médico cadastrado com este CRM e UF.");
        }
    }
}
