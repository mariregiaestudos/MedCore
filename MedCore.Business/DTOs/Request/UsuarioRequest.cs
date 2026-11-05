using MedCore.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MedCore.Business.DTOs.Request;

public class UsuarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Senha { get; set; } = string.Empty;

    public TipoCadastro TipoCadastro { get; set; }

    // Obrigatórios somente para cadastro de médico
    public string? CRM { get; set; }
    public string? UF { get; set; }
    public string? Especialidade { get; set; }
}
