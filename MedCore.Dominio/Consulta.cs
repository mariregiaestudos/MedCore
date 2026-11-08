using MedCore.Dominio.Enums;

namespace MedCore.Dominio;

public class Consulta
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid PacienteId { get; private set; }
    public Paciente Paciente { get; private set; } = null!;

    public Guid MedicoId { get; private set; }
    public Medico Medico { get; private set; } = null!;

    public DateTime DataHora { get; private set; }
    public int DuracaoMinutos { get; private set; }

    public StatusConsulta Status { get; private set; }

    public DateTime DataHoraFim =>
        DataHora.AddMinutes(DuracaoMinutos);

    protected Consulta() { } // EF Core

    public Consulta(
        Guid pacienteId,
        Guid medicoId,
        DateTime dataHora,
        int duracaoMinutos)
    {
        if (pacienteId == Guid.Empty)
            throw new ArgumentException("Paciente inválido.");

        if (medicoId == Guid.Empty)
            throw new ArgumentException("Médico inválido.");

        if (duracaoMinutos <= 0)
            throw new ArgumentException(
                "Duração deve ser maior que zero.");

        if (dataHora <= DateTime.Now)
            throw new ArgumentException(
                "A data da consulta deve ser futura.",
                nameof(dataHora));

        PacienteId = pacienteId;
        MedicoId = medicoId;
        DataHora = dataHora;
        DuracaoMinutos = duracaoMinutos;
        Status = StatusConsulta.Agendada;
    }

    public void Confirmar()
    {
        if (Status != StatusConsulta.Agendada)
            throw new InvalidOperationException(
                "Somente consultas agendadas podem ser confirmadas.");

        Status = StatusConsulta.Confirmada;
    }

    public void Cancelar()
    {
        if (Status == StatusConsulta.Concluida)
            throw new InvalidOperationException(
                "Uma consulta concluída não pode ser cancelada.");

        if (Status == StatusConsulta.Cancelada)
            throw new InvalidOperationException(
                "A consulta já está cancelada.");

        Status = StatusConsulta.Cancelada;
    }

    public void Concluir()
    {
        if (Status != StatusConsulta.Confirmada)
            throw new InvalidOperationException(
                "Somente consultas confirmadas podem ser concluídas.");

        Status = StatusConsulta.Concluida;
    }

    public void Reagendar(
        Guid novoMedicoId,
        DateTime novaDataHora,
        int novaDuracaoMinutos)
    {
        if (Status == StatusConsulta.Cancelada ||
            Status == StatusConsulta.Concluida)
        {
            throw new InvalidOperationException(
                "Esta consulta não pode ser reagendada.");
        }
        if (novoMedicoId == Guid.Empty)
            throw new ArgumentException("Médico inválido.", nameof(novoMedicoId));

        if (novaDataHora <= DateTime.Now)
            throw new ArgumentException(
                "A nova data da consulta deve ser futura.",
                nameof(novaDataHora));
        if (novaDuracaoMinutos <= 0)
            throw new ArgumentException(
                "Duração deve ser maior que zero.");

        MedicoId = novoMedicoId;
        DataHora = novaDataHora;
        DuracaoMinutos = novaDuracaoMinutos;
        Status = StatusConsulta.Agendada;
    }
}