using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Request;

public class BuscarUsuarioRequest
{
    public Guid? Codigo { get; set; }
    public string? Email { get; set; }
    public bool? Ativo { get; set; }
}
