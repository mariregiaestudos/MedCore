using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using MedCore.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedCore.Api.Controllers;

[Controller]
[Route("api/[controller]")]
public class UsuarioController(
    IUsuarioService _servico,
    IRespostaPadrao _resposta) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<RespostaApi<LoginResponse>>> Login(
    [FromBody] LoginRequest request)
    {
        return await _resposta.Executar(
            () => _servico.Login(request));
    }
}

