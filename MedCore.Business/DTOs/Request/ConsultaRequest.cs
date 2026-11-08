using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.DTOs.Request;

public class ConsultaRequest
{
    public Guid MedicoId { get; set; }
    public DateTime DataHora { get; set; }
    public int DuracaoMinutos { get; set; }
}
