using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Response;

public class UsuarioResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}
