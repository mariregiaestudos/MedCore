using MedCore.Dominio;
using MedCore.Dominio.Enums;

namespace MedCore.Test;

public class ConsultaTeste
{
    [Fact]
    public void CriarConsulta_ComDadosValidos_IniciaAgendadaECalculaFim()
    {
        var pacienteId = Guid.NewGuid();
        var medicoId = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1);
        var consulta = new Consulta(pacienteId, medicoId, dataHora, 30);

        Assert.NotEqual(Guid.Empty, consulta.Id);
        Assert.Equal(pacienteId, consulta.PacienteId);
        Assert.Equal(medicoId, consulta.MedicoId);
        Assert.Equal(dataHora, consulta.DataHora);
        Assert.Equal(dataHora.AddMinutes(30), consulta.DataHoraFim);
        Assert.Equal(30, consulta.DuracaoMinutos);
        Assert.Equal(StatusConsulta.Agendada, consulta.Status);
    }

    [Fact]
    public void CriarConsulta_ComPacienteVazio_LancaExcecao()
    {
        var acao = () => new Consulta(
            Guid.Empty, Guid.NewGuid(), DateTime.Now.AddDays(1), 30);

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CriarConsulta_ComMedicoVazio_LancaExcecao()
    {
        var acao = () => new Consulta(
            Guid.NewGuid(), Guid.Empty, DateTime.Now.AddDays(1), 30);

        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CriarConsulta_ComDuracaoNaoPositiva_LancaExcecao(int duracao)
    {
        var acao = () => new Consulta(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Now.AddDays(1), duracao);

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CriarConsulta_ComDataNaoFutura_LancaExcecao()
    {
        var acao = () => new Consulta(
            Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, 30);

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void Confirmar_ConsultaAgendada_AtualizaStatus()
    {
        var consulta = CriarConsulta();

        consulta.Confirmar();

        Assert.Equal(StatusConsulta.Confirmada, consulta.Status);
    }

    [Fact]
    public void Confirmar_ConsultaNaoAgendada_LancaExcecao()
    {
        var consulta = CriarConsulta();
        consulta.Cancelar();

        var acao = () => consulta.Confirmar();

        Assert.Throws<InvalidOperationException>(acao);
    }

    [Fact]
    public void Cancelar_ConsultaAgendada_AtualizaStatus()
    {
        var consulta = CriarConsulta();

        consulta.Cancelar();

        Assert.Equal(StatusConsulta.Cancelada, consulta.Status);
    }

    [Fact]
    public void Cancelar_ConsultaJaCancelada_LancaExcecao()
    {
        var consulta = CriarConsulta();
        consulta.Cancelar();

        var acao = () => consulta.Cancelar();

        Assert.Throws<InvalidOperationException>(acao);
    }

    [Fact]
    public void Cancelar_ConsultaConcluida_LancaExcecao()
    {
        var consulta = CriarConsulta();
        consulta.Confirmar();
        consulta.Concluir();

        var acao = () => consulta.Cancelar();

        Assert.Throws<InvalidOperationException>(acao);
    }

    [Fact]
    public void Concluir_ConsultaConfirmada_AtualizaStatus()
    {
        var consulta = CriarConsulta();
        consulta.Confirmar();

        consulta.Concluir();

        Assert.Equal(StatusConsulta.Concluida, consulta.Status);
    }

    [Theory]
    [InlineData(StatusConsulta.Agendada)]
    [InlineData(StatusConsulta.Cancelada)]
    public void Concluir_ConsultaNaoConfirmada_LancaExcecao(StatusConsulta status)
    {
        var consulta = CriarConsulta();
        if (status == StatusConsulta.Cancelada)
            consulta.Cancelar();

        var acao = () => consulta.Concluir();

        Assert.Throws<InvalidOperationException>(acao);
    }

    [Fact]
    public void Reagendar_ConsultaConfirmada_AtualizaDadosEVoltaAAgendada()
    {
        var consulta = CriarConsulta();
        consulta.Confirmar();
        var novoMedicoId = Guid.NewGuid();
        var novaDataHora = DateTime.Now.AddDays(2);

        consulta.Reagendar(novoMedicoId, novaDataHora, 45);

        Assert.Equal(novoMedicoId, consulta.MedicoId);
        Assert.Equal(novaDataHora, consulta.DataHora);
        Assert.Equal(novaDataHora.AddMinutes(45), consulta.DataHoraFim);
        Assert.Equal(45, consulta.DuracaoMinutos);
        Assert.Equal(StatusConsulta.Agendada, consulta.Status);
    }

    [Theory]
    [InlineData(StatusConsulta.Cancelada)]
    [InlineData(StatusConsulta.Concluida)]
    public void Reagendar_ConsultaEmEstadoFinal_LancaExcecao(StatusConsulta status)
    {
        var consulta = CriarConsulta();
        if (status == StatusConsulta.Cancelada)
        {
            consulta.Cancelar();
        }
        else
        {
            consulta.Confirmar();
            consulta.Concluir();
        }

        var acao = () => consulta.Reagendar(
            Guid.NewGuid(), DateTime.Now.AddDays(2), 30);

        Assert.Throws<InvalidOperationException>(acao);
    }

    [Fact]
    public void Reagendar_ComMedicoVazio_LancaExcecao()
    {
        var consulta = CriarConsulta();

        var acao = () => consulta.Reagendar(
            Guid.Empty, DateTime.Now.AddDays(2), 30);

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void Reagendar_ComDataPassada_LancaExcecao()
    {
        var consulta = CriarConsulta();

        var acao = () => consulta.Reagendar(
            Guid.NewGuid(), DateTime.Now, 30);

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void Reagendar_ComDuracaoNaoPositiva_LancaExcecao()
    {
        var consulta = CriarConsulta();

        var acao = () => consulta.Reagendar(
            Guid.NewGuid(), DateTime.Now.AddDays(2), 0);

        Assert.Throws<ArgumentException>(acao);
    }

    private static Consulta CriarConsulta() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        DateTime.Now.AddDays(1),
        30);
}
