using FluentValidation;
using MedCore.Business.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Validadores;

public class UsuarioValidator : AbstractValidator<UsuarioRequest>
{
    public UsuarioValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty()
            .WithMessage("O e-mail é obrigatório.")
            .EmailAddress()
            .WithMessage("Informe um e-mail válido.")
            .MaximumLength(256)
            .WithMessage("O e-mail deve ter no máximo 256 caracteres.");

        RuleFor(u => u.Senha)
            .NotEmpty()
            .WithMessage("A senha é obrigatória.")
            .MinimumLength(8)
            .WithMessage("A senha deve ter no mínimo 8 caracteres.")
            .MaximumLength(128)
            .WithMessage("A senha deve ter no máximo 128 caracteres.");
    }
}