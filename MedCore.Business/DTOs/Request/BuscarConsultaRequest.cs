using MedCore.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Request;

public class BuscarConsultaRequest
{
    public Guid? PacienteId { get; set; }
    public Guid? MedicoId { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public StatusConsulta? Status { get; set; }
}