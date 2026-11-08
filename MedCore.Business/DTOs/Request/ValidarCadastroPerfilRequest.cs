using MedCore.Dominio.Enums;

namespace MedCore.Business.DTOs.Request;

public class ValidarCadastroPerfilRequest
{
    public UsuarioRequest Usuario { get; set; } = new();
    public TipoCadastro TipoCadastro { get; set; }
    public MedicoRequest? Medico { get; set; }
}
