using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Response;

public class UsuarioResponse
{
    public Guid Codigo { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public Guid? CodigoPaciente { get; set; }
}
