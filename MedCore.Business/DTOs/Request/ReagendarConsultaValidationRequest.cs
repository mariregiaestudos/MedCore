namespace MedCore.Business.DTOs.Request;

public class ReagendarConsultaValidationRequest
{
    public Guid Codigo { get; set; }
    public Guid UsuarioId { get; set; }
    public ReagendarConsultaRequest Consulta { get; set; } = new();
}
