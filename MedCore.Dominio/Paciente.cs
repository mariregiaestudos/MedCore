namespace MedCore.Dominio;

public class Paciente
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = null!;

    public bool Ativo { get; private set; }

    protected Paciente() { } // EF Core

    public Paciente(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException(
                "Usuário inválido.",
                nameof(usuarioId));

        UsuarioId = usuarioId;
        Ativo = true;
    }

    public void Desativar() => Ativo = false;

    public void Ativar() => Ativo = true;
}