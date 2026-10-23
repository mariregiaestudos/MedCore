using MedCore.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Request;

public class PacienteRequest
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
}
