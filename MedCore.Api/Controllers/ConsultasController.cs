using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MedCore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ConsultasController(
    IConsultaService _servico,
    IRespostaPadrao _resposta) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RespostaApi<ConsultaResponse>>> Agendar(
        [FromBody] ConsultaRequest request)
    {
        if (!TentarObterUsuarioId(out var usuarioId))
            return Unauthorized();

        return await _resposta.Executar(
            () => _servico.Agendar(usuarioId, request),
            criado: true);
    }

    [HttpGet("{codigo:guid}")]
    public async Task<ActionResult<RespostaApi<ConsultaResponse>>> BuscarPorCodigo(
        Guid codigo)
    {
        if (!TentarObterUsuarioId(out var usuarioId))
            return Unauthorized();

        return await _resposta.Executar(
            () => _servico.BuscarPorCodigo(codigo, usuarioId));
    }

    [HttpGet]
    public async Task<ActionResult<RespostaApi<List<ConsultaResponse>>>> Buscar(
        [FromQuery] BuscarConsultaRequest filtros)
    {
        if (!TentarObterUsuarioId(out var usuarioId))
            return Unauthorized();

        return await _resposta.Executar(
            async () => await _servico.Buscar(filtros, usuarioId));
    }

    [HttpPut("{codigo:guid}")]
    public async Task<ActionResult<RespostaApi<ConsultaResponse>>> Reagendar(
        Guid codigo,
        [FromBody] ReagendarConsultaRequest request)
    {
        if (!TentarObterUsuarioId(out var usuarioId))
            return Unauthorized();

        return await _resposta.Executar(
            () => _servico.Reagendar(codigo, usuarioId, request));
    }

    [HttpPatch("{codigo:guid}/cancelar")]
    public async Task<ActionResult<RespostaApi<bool>>> Cancelar(Guid codigo)
    {
        if (!TentarObterUsuarioId(out var usuarioId))
            return Unauthorized();

        return await _resposta.Executar(
            () => _servico.Cancelar(codigo, usuarioId));
    }

    private bool TentarObterUsuarioId(out Guid usuarioId) =>
        Guid.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out usuarioId);
}
