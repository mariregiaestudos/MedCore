using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteService _servico;

    public PacientesController(IPacienteService servico)
    {
        _servico = servico;
    }

    [HttpPost]
    public async Task<ActionResult<PacienteResponse>> Criar(
        [FromBody] PacienteRequest request)
    {
        try
        {
            var paciente = await _servico.Criar(request);

            return CreatedAtAction(
                nameof(BuscarPorCodigo),
                new { codigo = paciente.Id },
                paciente);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("{codigo:guid}")]
    public async Task<ActionResult<PacienteResponse>> BuscarPorCodigo(
        Guid codigo)
    {
        try
        {
            return Ok(await _servico.BuscarPorCodigo(codigo));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<PacienteResponse>>> Buscar(
        [FromQuery] BuscarPacienteRequest filtros)
    {
        var pacientes = await _servico.Buscar(filtros);

        return Ok(pacientes);
    }

    [HttpPut("{codigo:guid}")]
    public async Task<ActionResult<PacienteResponse>> Atualizar(
        Guid codigo,
        [FromBody] PacienteRequest request)
    {
        try
        {
            var paciente = await _servico.Atualizar(codigo, request);

            return Ok(paciente);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("{codigo:guid}/desativar")]
    public async Task<IActionResult> Desativar(Guid codigo)
    {
        try
        {
            await _servico.Desativar(codigo);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}
