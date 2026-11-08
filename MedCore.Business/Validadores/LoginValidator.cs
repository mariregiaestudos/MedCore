using FluentValidation;
using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces;
using MedCore.Dominio;
using Microsoft.AspNetCore.Identity;

namespace MedCore.Business.Validadores;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    private readonly IUnityOfWork _uow;
    private readonly PasswordHasher<Usuario> _passwordHasher = new();

    public LoginRequestValidator(IUnityOfWork uow)
    {
        _uow = uow;
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O e-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("Informe um e-mail válido.");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage("A senha é obrigatória.");

        RuleFor(request => request).CustomAsync(ValidarCredenciaisAsync);
    }

    private async Task ValidarCredenciaisAsync(
        LoginRequest request,
        ValidationContext<LoginRequest> context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Senha))
        {
            return;
        }

        var usuario = await _uow.Usuarios.Buscar(
            new BuscarUsuarioRequest
            {
                Email = request.Email.Trim().ToLowerInvariant()
            });

        if (usuario is null || !usuario.Ativo ||
            _passwordHasher.VerifyHashedPassword(
                usuario,
                usuario.SenhaHash,
                request.Senha) == PasswordVerificationResult.Failed)
        {
            context.AddFailure("Email", "E-mail ou senha inválidos.");
        }
    }
}
