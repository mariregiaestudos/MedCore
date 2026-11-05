using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Request;

public class BuscarUsuarioRequest
{
    public Guid? Id { get; set; }
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? CPF { get; set; }
}
