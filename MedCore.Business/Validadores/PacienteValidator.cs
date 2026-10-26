using FluentValidation;
using MedCore.Business.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Validadores;

public class PacienteValidator : AbstractValidator<PacienteRequest>
{
    public PacienteValidator()
    {
        RuleFor(p => p.Nome)
            .NotEmpty()
            .WithMessage("O nome é obrigatório.")
            .MaximumLength(150)
            .WithMessage("O nome deve ter no máximo 150 caracteres.");


        RuleFor(p => p.Email)
            .NotEmpty()
                .WithMessage("O e-mail é obrigatório.")
            .EmailAddress()
                .WithMessage("Informe um e-mail válido.")
            .MaximumLength(254)
                .WithMessage("O e-mail deve ter no máximo 254 caracteres.");
    }
}
