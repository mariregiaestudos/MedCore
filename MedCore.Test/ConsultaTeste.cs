using MedCore.Dominio;
using MedCore.Dominio.Enums;

namespace MedCore.Test;

public class ConsultaTeste
{
    [Fact]
    public void CriarConsulta_ComDadosValidos_DeveAgendarConsulta()
    {
        var pacienteId = Guid.NewGuid();
        var medicoId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1);
        var duracaoMinutos = 30;

        var consulta = new Consulta(
            pacienteId,
            medicoId,
            dataHora,
            duracaoMinutos);

        Assert.NotEqual(Guid.Empty, consulta.Id);
        Assert.Equal(pacienteId, consulta.PacienteId);
        Assert.Equal(medicoId, consulta.MedicoId);
        Assert.Equal(dataHora, consulta.DataHora);
        Assert.Equal(duracaoMinutos, consulta.DuracaoMinutos);
        Assert.Equal(StatusConsulta.Agendada, consulta.Status);
    }

    [Fact]
    public void CriarConsulta_ComPacienteVazio_DeveLancarExcecao()
    {
        var acao = () => new Consulta(
            Guid.Empty,
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            30);

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CriarConsulta_ComMedicoVazio_DeveLancarExcecao()
    {
        var acao = () => new Consulta(
            Guid.NewGuid(),
            Guid.Empty,
            DateTime.Now.AddDays(1),
            30);

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CriarConsulta_ComDataPassada_DeveLancarExcecao()
    {
        var acao = () => new Consulta(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now.AddDays(-1),
            30);

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CancelarConsulta_Agendada_DeveAlterarStatus()
    {
        var consulta = new Consulta(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            30);

        consulta.Cancelar();

        Assert.Equal(StatusConsulta.Cancelada, consulta.Status);
    }

    [Fact]
    public void CancelarConsulta_JaCancelada_DeveLancarExcecao()
    {
        var consulta = new Consulta(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now.AddDays(1),
            30);

        consulta.Cancelar();

        var acao = () => consulta.Cancelar();

        Assert.Throws<InvalidOperationException>(acao);
    }
}