
namespace MedCore.Business.DTOs.Request;

public class PacienteRequest
{
    public string? Nome { get; set; }
    public string? Email { get; set; }
    public string? Cpf { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? Senha { get; set; }
}
