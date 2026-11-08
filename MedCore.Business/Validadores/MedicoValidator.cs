using FluentValidation;
using MedCore.Business.DTOs.Request;

namespace MedCore.Business.Validadores;

public class MedicoValidator : AbstractValidator<MedicoRequest>
{
    public MedicoValidator()
    {
        RuleFor(m => (UsuarioRequest)m)
            .SetValidator(new UsuarioValidator());

        RuleFor(m => m.CRM)
            .NotEmpty()
            .WithMessage("O CRM é obrigatório.")
            .MaximumLength(20)
            .WithMessage("O CRM deve ter no máximo 20 caracteres.");

        RuleFor(m => m.UF)
            .NotEmpty()
            .WithMessage("A UF é obrigatória.")
            .Must(uf => uf is not null &&
                uf.Trim().Length == 2 &&
                uf.Trim().All(char.IsLetter))
            .WithMessage("A UF deve conter exatamente duas letras.");

        RuleFor(m => m.Especialidade)
            .NotEmpty()
            .WithMessage("A especialidade é obrigatória.")
            .MaximumLength(100)
            .WithMessage("A especialidade deve ter no máximo 100 caracteres.");
    }
}
