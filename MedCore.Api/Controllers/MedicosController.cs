using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MedicosController(
    IMedicoService _servico,
    IUsuarioService _usuarios,
    IRespostaPadrao _resposta) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<RespostaApi<MedicoResponse>>> Cadastrar(
        [FromBody] MedicoRequest request)
    {
        return await _resposta.Executar(
            () => _usuarios.CadastrarMedico(request),
            criado: true);
    }

    [HttpGet("{codigo:guid}")]
    public async Task<ActionResult<RespostaApi<MedicoResponse>>> BuscarPorCodigo(
        Guid codigo)
    {
        return await _resposta.Executar(
            () => _servico.BuscarPorCodigo(codigo));
    }

    [HttpGet]
    public async Task<ActionResult<RespostaApi<List<MedicoResponse>>>> Buscar(
        [FromQuery] BuscarMedicoRequest filtros)
    {
        return await _resposta.Executar(
            async () => await _servico.Buscar(filtros));
    }

    [HttpPut("{codigo:guid}")]
    public async Task<ActionResult<RespostaApi<MedicoResponse>>> Atualizar(
        Guid codigo,
        [FromBody] AtualizarMedicoRequest request)
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
