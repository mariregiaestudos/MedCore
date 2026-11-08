using FluentValidation;
using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces;
using MedCore.Dominio.Enums;

namespace MedCore.Business.Validadores;

public class ReagendarConsultaValidator
    : AbstractValidator<ReagendarConsultaValidationRequest>
{
    public ReagendarConsultaValidator(IUnityOfWork uow)
    {
        RuleFor(request => request).CustomAsync(async (request, context, cancellationToken) =>
        {
            var paciente = await uow.Pacientes.BuscarPorUsuarioId(request.UsuarioId);
            var consulta = await uow.Consultas.BuscarPorCodigo(request.Codigo);

            if (paciente is null || consulta is null ||
                consulta.PacienteId != paciente.Id)
            {
                context.AddFailure("Codigo", "Consulta não encontrada.");
                return;
            }

            if (!paciente.Ativo)
            {
                context.AddFailure(
                    "UsuarioId",
                    "Não é possível reagendar uma consulta para um paciente inativo.");
            }

            if (consulta.Status is StatusConsulta.Cancelada or StatusConsulta.Concluida)
            {
                context.AddFailure(
                    "Codigo",
                    "Esta consulta não pode ser reagendada.");
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
                    "Não é possível reagendar uma consulta com um médico inativo.");
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
                    "A nova data da consulta deve ser futura.");
            }

            if (request.Consulta.DuracaoMinutos <= 0)
            {
                context.AddFailure(
                    "Consulta.DuracaoMinutos",
                    "A duração deve ser maior que zero.");
            }

            if (!paciente.Ativo ||
                consulta.Status is StatusConsulta.Cancelada or StatusConsulta.Concluida ||
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
                dataHoraFim,
                consulta.Id);

            if (existeConflito)
            {
                context.AddFailure(
                    "Consulta.DataHora",
                    "Já existe uma consulta nesse horário para o médico ou paciente.");
            }
        });
    }
}
