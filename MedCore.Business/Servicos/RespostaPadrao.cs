
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Business.Servicos;

public class RespostaPadrao(
    IColetorErros coletorErros) : IRespostaPadrao
{
    public async Task<ActionResult<RespostaApi<T>>> Executar<T>(
        Func<Task<T?>> acao,
        bool criado = false)
    {
        coletorErros.LimparErros();

        try
        {
            var resultado = await acao();

            if (coletorErros.PossuiErros)
            {
                return new BadRequestObjectResult(
                    new RespostaApi<object>
                    {
                        Sucesso = false,
                        Resultado = null,
                        Erros = coletorErros.GerarErros()
                    });
            }

            if (resultado is null)
            {
                return new NotFoundObjectResult(
                    new RespostaApi<object>
                    {
                        Sucesso = false,
                        Resultado = null,
                        Erros = ["Registro não encontrado."]
                    });
            }

            var resposta = new RespostaApi<T>
            {
                Sucesso = true,
                Resultado = resultado,
                Erros = null
            };

            return criado
                ? new ObjectResult(resposta) { StatusCode = 201 }
                : new OkObjectResult(resposta);
        }
        catch (Exception)
        {
            return new ObjectResult(
                new RespostaApi<object>
                {
                    Sucesso = false,
                    Resultado = null,
                    Erros = ["Ocorreu um erro interno ao processar a solicitação."]
                })
            {
                StatusCode = 500
            };
        }
    }
}
