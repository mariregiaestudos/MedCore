using FluentValidation;
using MedCore.Business.DTOs.Request;

namespace MedCore.Business.Validadores;

public class UsuarioValidator : AbstractValidator<UsuarioRequest>
{
    public UsuarioValidator()
    {
        RuleFor(u => u.Nome)
            .NotEmpty()
            .WithMessage("O nome é obrigatório.")
            .MaximumLength(150)
            .WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(u => u.CPF)
            .NotEmpty()
            .WithMessage("O CPF é obrigatório.")
            .Must(CpfValido)
            .WithMessage("Informe um CPF válido.");

        RuleFor(u => u.DataNascimento)
            .NotEmpty()
            .WithMessage("A data de nascimento é obrigatória.")
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("A data de nascimento não pode ser futura.");

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

    private static bool CpfValido(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) ||
            cpf.Any(caractere =>
                !DigitoAscii(caractere) && caractere is not '.' and not '-'))
        {
            return false;
        }

        var formatoValido = cpf.Length == 11 ||
            (cpf.Length == 14 &&
             cpf[3] == '.' &&
             cpf[7] == '.' &&
             cpf[11] == '-');

        if (!formatoValido)
            return false;

        var digitos = cpf
            .Where(DigitoAscii)
            .Select(caractere => caractere - '0')
            .ToArray();

        if (digitos.Length != 11 || digitos.All(digito => digito == digitos[0]))
            return false;

        var primeiroDigito = CalcularDigitoVerificador(digitos, 9, 10);
        if (digitos[9] != primeiroDigito)
            return false;

        var segundoDigito = CalcularDigitoVerificador(digitos, 10, 11);
        return digitos[10] == segundoDigito;
    }

    private static bool DigitoAscii(char caractere) =>
        caractere is >= '0' and <= '9';

    private static int CalcularDigitoVerificador(
        int[] digitos,
        int quantidade,
        int pesoInicial)
    {
        var soma = 0;

        for (var indice = 0; indice < quantidade; indice++)
            soma += digitos[indice] * (pesoInicial - indice);

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
