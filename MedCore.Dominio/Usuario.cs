
namespace MedCore.Dominio;

public class Usuario
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Email { get; private set; } = string.Empty;

    public string SenhaHash { get; private set; } = string.Empty;

    public bool Ativo { get; private set; } = true;

    // Relacionamento opcional com Paciente
    public Paciente? Paciente { get; private set; }

    protected Usuario() { }

    public Usuario(string email, string senhaHash)
    {
        Email = email.Trim();
        SenhaHash = senhaHash;
    }

    public void AtualizarSenha(string senhaHash)
    {
        SenhaHash = senhaHash;
    }

    public void Desativar()
    {
        Ativo = false;
    }
}
