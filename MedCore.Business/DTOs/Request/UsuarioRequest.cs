using System;

namespace MedCore.Business.DTOs.Request;

public class UsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Senha { get; set; } = string.Empty;
}
