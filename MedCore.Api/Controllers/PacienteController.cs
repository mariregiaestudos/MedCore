
using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers;

[Authorize]
[Controller]
[Route("api/[controller]")]
public class PacientesController(
    IPacienteService _servico,
    IUsuarioService _usuarios,
    IRespostaPadrao _resposta) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<RespostaApi<PacienteResponse>>> Cadastrar(
        [FromBody] UsuarioRequest request)
    {
        return await _resposta.Executar(
            () => _usuarios.CadastrarPaciente(request),
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

    [HttpPatch("{codigo:guid}/desativar")]
    public async Task<ActionResult<RespostaApi<bool>>> Desativar(
        Guid codigo)
    {
        return await _resposta.Executar(
            () => _servico.Desativar(codigo));
    }
}
