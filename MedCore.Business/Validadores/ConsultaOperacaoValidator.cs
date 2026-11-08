using FluentValidation;
using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces;
using MedCore.Dominio.Enums;

namespace MedCore.Business.Validadores;

public class ConsultaOperacaoValidator
    : AbstractValidator<ValidarConsultaRequest>
{
    public ConsultaOperacaoValidator(IUnityOfWork uow)
    {
        RuleFor(request => request).CustomAsync(async (request, context, cancellationToken) =>
        {
            var paciente = request.UsuarioId.HasValue
                ? await uow.Pacientes.BuscarPorUsuarioId(request.UsuarioId.Value)
                : null;

            if (request.UsuarioId.HasValue && paciente is null)
            {
                context.AddFailure(
                    "UsuarioId",
                    "Somente usuários cadastrados como pacientes podem acessar estas consultas.");
                return;
            }

            if (request.Operacao == OperacaoConsulta.Listar)
                return;

            var consulta = await uow.Consultas.BuscarPorCodigo(request.Codigo);

            if (consulta is null ||
                (paciente is not null && consulta.PacienteId != paciente.Id))
            {
                context.AddFailure("Codigo", "Consulta não encontrada.");
                return;
            }

            var statusRequerido = request.Operacao switch
            {
                OperacaoConsulta.Confirmar => StatusConsulta.Agendada,
                OperacaoConsulta.Concluir => StatusConsulta.Confirmada,
                _ => (StatusConsulta?)null
            };

            if (statusRequerido.HasValue && consulta.Status != statusRequerido)
            {
                var mensagem = request.Operacao == OperacaoConsulta.Confirmar
                    ? "Somente consultas agendadas podem ser confirmadas."
                    : "Somente consultas confirmadas podem ser concluídas.";

                context.AddFailure("Codigo", mensagem);
            }

            if (request.Operacao == OperacaoConsulta.Cancelar &&
                consulta.Status is StatusConsulta.Cancelada or StatusConsulta.Concluida)
            {
                var mensagem = consulta.Status == StatusConsulta.Cancelada
                    ? "A consulta já está cancelada."
                    : "Uma consulta concluída não pode ser cancelada.";

                context.AddFailure("Codigo", mensagem);
            }
        });
    }
}
