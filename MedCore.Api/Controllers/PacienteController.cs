
using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PacientesController(
    IPacienteService _servico,
    IRespostaPadrao _resposta) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RespostaApi<PacienteResponse>>> Criar(
        [FromBody] PacienteRequest request)
    {
        return await _resposta.Executar(
            () => _servico.Criar(request),
            criado: true);
    }

    [HttpGet("{codigo:guid}")]
    public async Task<ActionResult<RespostaApi<PacienteResponse>>> BuscarPorCodigo(
        Guid codigo)
    {
        return await _resposta.Executar(
            () => _servico.BuscarPorCodigo(codigo));
    }

    [HttpGet]
    public async Task<ActionResult<RespostaApi<List<PacienteResponse>>>> Buscar(
        [FromQuery] BuscarPacienteRequest filtros)
    {
        return await _resposta.Executar(
            async () => await _servico.Buscar(filtros));
    }

    [HttpPut("{codigo:guid}")]
    public async Task<ActionResult<RespostaApi<PacienteResponse>>> Atualizar(
        Guid codigo,
        [FromBody] PacienteRequest request)
    {
        return await _resposta.Executar(
            () => _servico.Atualizar(codigo, request));
    }

    [HttpPatch("{codigo:guid}/desativar")]
    public async Task<ActionResult<RespostaApi<bool>>> Desativar(
        Guid codigo)
    {
        return await _resposta.Executar(
            () => _servico.Desativar(codigo));
    }
}
