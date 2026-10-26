using Microsoft.AspNetCore.Mvc;
using MedCore.Business.DTOs.Response;

namespace MedCore.Business.Interfaces;

public interface IRespostaPadrao
{
    Task<ActionResult<RespostaApi<T>>> Executar<T>(
        Func<Task<T?>> acao,
        bool criado = false);
}
