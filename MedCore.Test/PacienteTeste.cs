using MedCore.Dominio;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Test;

public class PacienteTeste
{
    [Fact]
    public void CriarPaciente_ComDadosValidos_DeveCriarPaciente()
    {
        // Arrange
        var nome = "Maria Silva";
        var email = "maria@email.com";

        // Act
        var paciente = new Paciente(nome, email);

        // Assert
        Assert.NotEqual(Guid.Empty, paciente.Id);
        Assert.Equal(nome, paciente.Nome);
        Assert.Equal(email, paciente.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarPaciente_SemNome_DeveLancarExcecao(
        string? nome)
    {
        // Arrange
        var email = "maria@email.com";

        // Act
        var acao = () => new Paciente(nome!, email);

        // Assert
        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarPaciente_SemEmail_DeveLancarExcecao(
        string? email)
    {
        // Arrange
        var nome = "Maria Silva";

        // Act
        var acao = () => new Paciente(nome, email!);

        // Assert
        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CriarPaciente_ComEspacosNasExtremidades_DeveRemoverEspacos()
    {
        // Arrange
        var nome = " Maria Silva ";
        var email = " maria@email.com ";

        // Act
        var paciente = new Paciente(nome, email);

        // Assert
        Assert.Equal("Maria Silva", paciente.Nome);
        Assert.Equal("maria@email.com", paciente.Email);
    }
}
