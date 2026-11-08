using FluentValidation;
using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces;

namespace MedCore.Business.Validadores;

public class AgendarConsultaValidator
    : AbstractValidator<AgendarConsultaValidationRequest>
{
    public AgendarConsultaValidator(IUnityOfWork uow)
    {
        RuleFor(request => request).CustomAsync(async (request, context, cancellationToken) =>
        {
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

            var medico = await uow.Medicos.BuscarPorCodigo(request.Consulta.MedicoId);

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

            if (request.Consulta.DataHora <= DateTime.Now)
            {
                context.AddFailure(
                    "Consulta.DataHora",
                    "A data da consulta deve ser futura.");
            }

            if (request.Consulta.DuracaoMinutos <= 0)
            {
                context.AddFailure(
                    "Consulta.DuracaoMinutos",
                    "A duração deve ser maior que zero.");
            }

            if (!paciente.Ativo ||
                !medico.Ativo ||
                paciente.UsuarioId == medico.UsuarioId ||
                request.Consulta.DataHora <= DateTime.Now ||
                request.Consulta.DuracaoMinutos <= 0)
            {
                return;
            }

            var dataHoraFim = request.Consulta.DataHora.AddMinutes(
                request.Consulta.DuracaoMinutos);

            var existeConflito = await uow.Consultas.ExisteConflitoAsync(
                request.Consulta.MedicoId,
                paciente.Id,
                request.Consulta.DataHora,
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
