using FluentValidation;
using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces;

namespace MedCore.Business.Validadores;

public class MedicoOperacaoValidator
    : AbstractValidator<ValidarMedicoRequest>
{
    public MedicoOperacaoValidator(IUnityOfWork uow)
    {
        RuleFor(request => request).CustomAsync(async (request, context, cancellationToken) =>
        {
            var medico = await uow.Medicos.BuscarPorCodigo(request.Codigo);

            if (medico is null)
            {
                context.AddFailure("Codigo", "Médico não encontrado.");
                return;
            }

            if (request.Operacao != OperacaoMedico.Atualizar)
                return;

            if (request.Dados is null)
            {
                context.AddFailure("Dados", "Os dados profissionais são obrigatórios.");
                return;
            }

            var dados = request.Dados;

            if (string.IsNullOrWhiteSpace(dados.CRM) ||
                string.IsNullOrWhiteSpace(dados.UF) ||
                string.IsNullOrWhiteSpace(dados.Especialidade))
            {
                context.AddFailure(
                    "Dados",
                    "CRM, UF e especialidade são obrigatórios.");
                return;
            }

            var uf = dados.UF.Trim();
            if (uf.Length != 2 || !uf.All(char.IsLetter))
            {
                context.AddFailure(
                    "Dados.UF",
                    "A UF deve conter exatamente duas letras.");
                return;
            }

            var crm = dados.CRM.Trim();
            var ufNormalizada = uf.ToUpperInvariant();
            if ((medico.CRM != crm || medico.UF != ufNormalizada) &&
                await uow.Medicos.ExisteCRM(crm, ufNormalizada))
            {
                context.AddFailure(
                    "Dados.CRM",
                    "Já existe outro médico cadastrado com esse CRM e UF.");
            }
        });
    }
}
