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
    [HttpPost]
    public async Task<ActionResult<RespostaApi<UsuarioResponse?>>> Cadastrar(
        [FromBody] UsuarioRequest request)
    {
        return await _resposta.Executar(
            () => _servico.Cadastrar(request),
            criado: true);
    }

    [HttpPost("login")]
    public async Task<ActionResult<RespostaApi<LoginResponse>>> Login(
    [FromBody] LoginRequest request)
    {
        return await _resposta.Executar(
            () => _servico.Login(request));
    }
}

