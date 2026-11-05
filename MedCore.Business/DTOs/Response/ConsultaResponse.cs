using MedCore.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Response;

public class ConsultaResponse
{
    public Guid Id { get; set; }
    public Guid PacienteId { get; set; }
    public Guid MedicoId { get; set; }
    public DateTime DataHora { get; set; }
    public DateTime DataHoraFim { get; set; }
    public int DuracaoMinutos { get; set; }
    public StatusConsulta Status { get; set; }
}
