using FluentValidation;
using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces;

namespace MedCore.Business.Validadores;

public class PacienteOperacaoValidator
    : AbstractValidator<ValidarPacienteRequest>
{
    public PacienteOperacaoValidator(IUnityOfWork uow)
    {
        RuleFor(request => request.Codigo)
            .MustAsync(async (codigo, cancellationToken) =>
                await uow.Pacientes.BuscarPorCodigo(codigo) is not null)
            .WithMessage("Paciente não encontrado.");
    }
}
