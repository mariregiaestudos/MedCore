namespace MedCore.Business.DTOs.Request;

public class ValidarPacienteRequest
{
    public Guid Codigo { get; set; }
    public OperacaoPaciente Operacao { get; set; }
}

public enum OperacaoPaciente
{
    Buscar,
    Desativar
}
