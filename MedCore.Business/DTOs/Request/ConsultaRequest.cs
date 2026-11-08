namespace MedCore.Business.DTOs.Request;

public class ConsultaRequest
{
    public Guid? MedicoId { get; set; }
    public DateTime? DataHora { get; set; }
    public int? DuracaoMinutos { get; set; }
}
