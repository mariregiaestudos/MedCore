using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Response;

public class ApiResposta
{
    public bool Sucesso { get; init; }

    public string Mensagem { get; init; } = string.Empty;

    public List<string>? Erros { get; init; }
}
