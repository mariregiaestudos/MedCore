using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Response;

public class RespostaApi<T>
{
    public bool Sucesso { get; init; }
    public T? Resultado { get; init; }
    public List<string>? Erros { get; init; }
}
