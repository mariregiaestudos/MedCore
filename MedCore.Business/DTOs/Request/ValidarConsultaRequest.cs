using MedCore.Dominio.Enums;

namespace MedCore.Business.DTOs.Request;

public class ValidarConsultaRequest
{
    public Guid Codigo { get; set; }
    public Guid? UsuarioId { get; set; }
    public OperacaoConsulta Operacao { get; set; }
}

public enum OperacaoConsulta
{
    Buscar,
    Listar,
    Confirmar,
    Cancelar,
    Concluir
}
