using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Request;

public class BuscarMedicoRequest
{
    public string? Nome { get; set; }
    public string? CRM { get; set; }
    public string? UF { get; set; }
    public string? Especialidade { get; set; }
    public bool? Ativo { get; set; }
}
