using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Response;

public class MedicoResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CRM { get; set; } = string.Empty;
    public string UF { get; set; } = string.Empty;
    public string Especialidade { get; set; } = string.Empty;
}