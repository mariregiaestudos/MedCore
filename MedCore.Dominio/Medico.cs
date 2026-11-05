namespace MedCore.Dominio;

public class Medico
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid UsuarioId { get; private set; }
    public Usuario Usuario { get; private set; } = null!;

    public string CRM { get; private set; } = string.Empty;
    public string UF { get; private set; } = string.Empty;
    public string Especialidade { get; private set; } = string.Empty;

    public bool Ativo { get; private set; }

    protected Medico() { } // EF Core

    public Medico(
        Guid usuarioId,
        string crm,
        string uf,
        string especialidade)
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException(
                "Usuário inválido.",
                nameof(usuarioId));

        UsuarioId = usuarioId;
        AlterarDados(crm, uf, especialidade);
        Ativo = true;
    }

    public void AlterarDados(
        string crm,
        string uf,
        string especialidade)
    {
        crm = crm?.Trim() ?? string.Empty;
        uf = uf?.Trim().ToUpperInvariant() ?? string.Empty;
        especialidade = especialidade?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(crm))
            throw new ArgumentException("CRM obrigatório.", nameof(crm));

        if (uf.Length != 2)
            throw new ArgumentException("UF inválida.", nameof(uf));

        if (string.IsNullOrWhiteSpace(especialidade))
            throw new ArgumentException(
                "Especialidade obrigatória.",
                nameof(especialidade));

        CRM = crm;
        UF = uf;
        Especialidade = especialidade;
    }

    public void Desativar() => Ativo = false;

    public void Ativar() => Ativo = true;
}