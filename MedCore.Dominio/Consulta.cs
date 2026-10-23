using MedCore.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Dominio;

public class Consulta
{
    public Guid Codigo { get; private set; }
    public Guid PacienteCodigo { get; private set; }
    public Guid MedicoCodigo { get; private set; }
    public DateTime DataHora { get; private set; }
    public StatusConsulta Status { get; private set; }

    private Consulta() { }

    public Consulta(
        Guid pacienteCodigo,
        Guid medicoCodigo,
        DateTime dataHora)
    {
        if (pacienteCodigo == Guid.Empty)
            throw new ArgumentException(
                "O paciente é obrigatório.", nameof(pacienteCodigo));

        if (medicoCodigo == Guid.Empty)
            throw new ArgumentException(
                "O médico é obrigatório.", nameof(medicoCodigo));

        if (dataHora <= DateTime.Now)
            throw new ArgumentException(
                "A consulta deve ser agendada para o futuro.", nameof(dataHora));

        Codigo = Guid.NewGuid();
        PacienteCodigo = pacienteCodigo;
        MedicoCodigo = medicoCodigo;
        DataHora = dataHora;
        Status = StatusConsulta.Agendada;
    }

    public void Cancelar()
    {
        if (Status == StatusConsulta.Cancelada)
            throw new InvalidOperationException(
                "A consulta já está cancelada.");

        Status = StatusConsulta.Cancelada;
    }
}
