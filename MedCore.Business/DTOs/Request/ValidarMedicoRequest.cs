namespace MedCore.Business.DTOs.Request;

public class ValidarMedicoRequest
{
    public Guid Codigo { get; set; }
    public AtualizarMedicoRequest? Dados { get; set; }
    public OperacaoMedico Operacao { get; set; }
}

public enum OperacaoMedico
{
    Buscar,
    Atualizar,
    Desativar
}
