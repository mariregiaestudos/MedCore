namespace MedCore.Business.DTOs.Request;

public class MedicoRequest : UsuarioRequest
{
    public string? CRM { get; set; }
    public string? UF { get; set; }
    public string? Especialidade { get; set; }
}
