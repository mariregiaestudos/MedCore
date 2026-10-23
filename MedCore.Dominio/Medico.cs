using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Dominio;

public class Medico
{
    public Guid Codigo { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public string Crm { get; private set; } = string.Empty;
    public string Especialidade { get; private set; } = string.Empty;

    private Medico() { }

    public Medico(string nome, string crm, string especialidade)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException(
                "O nome é obrigatório.", nameof(nome));

        if (string.IsNullOrWhiteSpace(crm))
            throw new ArgumentException(
                "O CRM é obrigatório.", nameof(crm));

        if (string.IsNullOrWhiteSpace(especialidade))
            throw new ArgumentException(
                "A especialidade é obrigatória.", nameof(especialidade));

        Codigo = Guid.NewGuid();
        Nome = nome.Trim();
        Crm = crm.Trim();
        Especialidade = especialidade.Trim();
    }
}
