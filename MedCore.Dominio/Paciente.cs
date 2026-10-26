using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MedCore.Dominio;

public class Paciente
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public Guid? IdUsuario { get; private set; }
    public Usuario? Usuario { get; private set; }
    public bool Ativo { get; private set; }

    private Paciente()
    {
    }

    public Paciente(string nome, string email)
    {
        ValidarDados(nome, email);

        Id = Guid.NewGuid();
        Nome = nome.Trim();
        Email = email.Trim();
        Ativo = true;
    }

    public void AtualizarDados(string nome, string email)
    {
        ValidarDados(nome, email);

        Nome = nome.Trim();
        Email = email.Trim();
    }

    public void Desativar()
    {
        Ativo = false;
    }

    public void AssociarUsuario(Guid idUsuario)
    {
        IdUsuario = idUsuario;
    }

    private static void ValidarDados(string nome, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("O e-mail é obrigatório.", nameof(email));
    }


}
