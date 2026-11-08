using FluentValidation;
using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces;

namespace MedCore.Business.Validadores;

public class AgendarConsultaValidator
    : AbstractValidator<AgendarConsultaValidationRequest>
{
    public AgendarConsultaValidator(IUnityOfWork uow)
    {
        RuleFor(request => request.Consulta.MedicoId)
            .NotNull()
            .WithMessage("O médico é obrigatório.")
            .NotEqual(Guid.Empty)
            .WithMessage("O médico é obrigatório.");

        RuleFor(request => request.Consulta.DataHora)
            .NotNull()
            .WithMessage("A data da consulta é obrigatória.")
            .GreaterThan(DateTime.Now)
            .WithMessage("A data da consulta deve ser futura.");

        RuleFor(request => request.Consulta.DuracaoMinutos)
            .NotNull()
            .WithMessage("A duração da consulta é obrigatória.")
            .GreaterThan(0)
            .WithMessage("A duração deve ser maior que zero.");

        RuleFor(request => request).CustomAsync(async (request, context, cancellationToken) =>
        {
            if (request.Consulta.MedicoId is not Guid medicoId ||
                medicoId == Guid.Empty ||
                request.Consulta.DataHora is not DateTime dataHora ||
                dataHora <= DateTime.Now ||
                request.Consulta.DuracaoMinutos is not int duracaoMinutos ||
                duracaoMinutos <= 0)
            {
                return;
            }

            var paciente = await uow.Pacientes.BuscarPorUsuarioId(request.UsuarioId);

            if (paciente is null)
            {
                context.AddFailure(
                    "UsuarioId",
                    "Somente usuários cadastrados como pacientes podem agendar consultas.");

                return;
            }

            if (!paciente.Ativo)
            {
                context.AddFailure(
                    "UsuarioId",
                    "Não é possível agendar uma consulta para um paciente inativo.");
            }

            var medico = await uow.Medicos.BuscarPorCodigo(medicoId);

            if (medico is null)
            {
                context.AddFailure("Consulta.MedicoId", "Médico não encontrado.");
                return;
            }

            if (!medico.Ativo)
            {
                context.AddFailure(
                    "Consulta.MedicoId",
                    "Não é possível agendar uma consulta com um médico inativo.");
            }

            if (paciente.UsuarioId == medico.UsuarioId)
            {
                context.AddFailure(
                    "Consulta.MedicoId",
                    "Não é permitido agendar uma consulta com o próprio médico.");
            }

            if (!paciente.Ativo ||
                !medico.Ativo ||
                paciente.UsuarioId == medico.UsuarioId)
            {
                return;
            }

            var dataHoraFim = dataHora.AddMinutes(duracaoMinutos);

            var existeConflito = await uow.Consultas.ExisteConflitoAsync(
                medicoId,
                paciente.Id,
                dataHora,
                dataHoraFim);

            if (existeConflito)
            {
                context.AddFailure(
                    "Consulta.DataHora",
                    "Já existe uma consulta nesse horário para o médico ou paciente.");
            }
        });
    }
}
