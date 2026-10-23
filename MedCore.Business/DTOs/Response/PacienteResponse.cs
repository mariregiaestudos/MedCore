using MedCore.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Response;

public class PacienteResponse
{
    public Guid? Id { get;  set; }
    public string? Nome { get;  set; }
    public string? Email { get; set; }
}
