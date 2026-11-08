namespace MedCore.Business.DTOs.Request;

public class AgendarConsultaValidationRequest
{
    public Guid UsuarioId { get; set; }
    public ConsultaRequest Consulta { get; set; } = new();
}
