using FluentValidation;
using MedCore.Business.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Validadores;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("O e-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("Informe um e-mail válido.");

        RuleFor(x => x.Senha)
            .NotEmpty()
            .WithMessage("A senha é obrigatória.");
    }
}
