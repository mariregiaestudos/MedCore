using FluentValidation;
using MedCore.Business.Interfaces;

namespace MedCore.Business.Validadores;

public class ValidadorService(
    IServiceProvider serviceProvider,
    IColetorErros coletorErros)
{
    public async Task<bool> ValidarAsync<T>(
        T request,
        CancellationToken cancellationToken = default)
    {
        var validator = serviceProvider
            .GetService(typeof(IValidator<T>))
            as IValidator<T>;
        if (validator is null)
        {
            coletorErros.AdicionarErro(
                $"Validador não registrado para {typeof(T).Name}.");

            return false;
        }

        var resultado = await validator.ValidateAsync(
            request,
            cancellationToken);

        foreach (var erro in resultado.Errors)
        {
            coletorErros.AdicionarErro(erro.ErrorMessage);
        }

        return resultado.IsValid;
    }
}
