namespace MedCore.Dominio;

public class Usuario
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string CPF { get; private set; } = string.Empty;
    public DateTime DataNascimento { get; private set; }

    public string SenhaHash { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }

    public Paciente? Paciente { get; private set; }
    public Medico? Medico { get; private set; }

    protected Usuario() { } // EF Core

    public Usuario(
        string nome,
        string email,
        string cpf,
        DateTime dataNascimento,
        string senhaHash)
    {
        AlterarDadosPessoais(nome, email, cpf, dataNascimento);
        AlterarSenhaHash(senhaHash);

        Ativo = true;
    }

    public void AlterarDadosPessoais(
        string nome,
        string email,
        string cpf,
        DateTime dataNascimento)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("E-mail obrigatório.", nameof(email));

        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("CPF obrigatório.", nameof(cpf));

        if (dataNascimento.Date > DateTime.Today)
            throw new ArgumentException(
                "Data de nascimento inválida.",
                nameof(dataNascimento));

        Nome = nome.Trim();
        Email = email.Trim().ToLowerInvariant();
        CPF = cpf.Trim();
        DataNascimento = dataNascimento.Date;
    }

    public void AlterarSenhaHash(string senhaHash)
    {
        if (string.IsNullOrWhiteSpace(senhaHash))
            throw new ArgumentException(
                "O hash da senha é obrigatório.",
                nameof(senhaHash));

        SenhaHash = senhaHash;
    }

    public void Desativar() => Ativo = false;

    public void Ativar() => Ativo = true;
}