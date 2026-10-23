using MedCore.Dominio;
using MedCore.Dominio.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Test;

public class ConsultaTeste
{
    [Fact]
    public void CriarConsulta_ComDadosValidos_DeveAgendarConsulta()
    {
        // Arrange
        var pacienteCodigo = Guid.NewGuid();
        var medicoCodigo = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1);

        // Act
        var consulta = new Consulta(
            pacienteCodigo,
            medicoCodigo,
            dataHora);

        // Assert
        Assert.NotEqual(Guid.Empty, consulta.Codigo);
        Assert.Equal(pacienteCodigo, consulta.PacienteCodigo);
        Assert.Equal(medicoCodigo, consulta.MedicoCodigo);
        Assert.Equal(dataHora, consulta.DataHora);
        Assert.Equal(StatusConsulta.Agendada, consulta.Status);
    }

    [Fact]
    public void CriarConsulta_ComPacienteVazio_DeveLancarExcecao()
    {
        // Arrange
        var medicoCodigo = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1);

        // Act
        var acao = () => new Consulta(
            Guid.Empty,
            medicoCodigo,
            dataHora);

        // Assert
        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CriarConsulta_ComMedicoVazio_DeveLancarExcecao()
    {
        // Arrange
        var pacienteCodigo = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(1);

        // Act
        var acao = () => new Consulta(
            pacienteCodigo,
            Guid.Empty,
            dataHora);

        // Assert
        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CriarConsulta_ComDataPassada_DeveLancarExcecao()
    {
        // Arrange
        var pacienteCodigo = Guid.NewGuid();
        var medicoCodigo = Guid.NewGuid();
        var dataHora = DateTime.Now.AddDays(-1);

        // Act
        var acao = () => new Consulta(
            pacienteCodigo,
            medicoCodigo,
            dataHora);

        // Assert
        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CancelarConsulta_Agendada_DeveAlterarStatus()
    {
        // Arrange
        var consulta = new Consulta(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now.AddDays(1));

        // Act
        consulta.Cancelar();

        // Assert
        Assert.Equal(StatusConsulta.Cancelada, consulta.Status);
    }

    [Fact]
    public void CancelarConsulta_JaCancelada_DeveLancarExcecao()
    {
        // Arrange
        var consulta = new Consulta(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now.AddDays(1));

        consulta.Cancelar();

        // Act
        var acao = () => consulta.Cancelar();

        // Assert
        Assert.Throws<InvalidOperationException>(acao);
    }
}
